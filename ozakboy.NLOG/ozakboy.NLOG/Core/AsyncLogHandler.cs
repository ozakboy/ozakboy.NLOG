using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 異步日誌處理器，負責管理日誌的異步寫入操作
    /// </summary>
    internal static class AsyncLogHandler
    {
        // 核心變數
        /// <summary>
        /// 日誌隊列，用於存儲待處理的日誌項目
        /// 使用 ConcurrentQueue 確保線程安全的入隊和出隊操作
        /// </summary>
        private static readonly ConcurrentQueue<LogItem> _logQueue = new ConcurrentQueue<LogItem>();

        /// <summary>
        /// 信號量，用於通知處理線程有新的日誌需要處理
        /// 初始計數為0，每當有新日誌加入時會釋放一個信號
        /// </summary>
        private static readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// 取消令牌源，用於控制處理線程的生命週期
        /// 當需要停止處理線程時，可以通過此令牌發出取消信號
        /// </summary>
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 日誌處理任務，負責實際的日誌寫入操作
        /// </summary>
        private static Task _processTask;

        /// <summary>
        /// 標記處理器是否已初始化
        /// </summary>
        private static bool _isInitialized;

        /// <summary>
        /// 用於初始化同步的鎖對象
        /// </summary>
        private static readonly object _lockObj = new object();

        /// <summary>
        /// 獲取當前的異步配置
        /// </summary>
        private static LogConfiguration.IAsyncLogOptions CurrentAsyncOptions
            => LogConfiguration.Current.AsyncOptions;

        /// <summary>
        /// 上次寫入的時間
        /// </summary>
        private static DateTime _lastFlushTime = DateTime.Now;

        /// <summary>
        /// 初始化異步日誌處理器
        /// 確保處理線程只被創建一次
        /// </summary>
        public static void Initialize()
        {
            // 如果已經初始化，直接返回
            if (_isInitialized) return;

            // 使用鎖確保線程安全的初始化
            lock (_lockObj)
            {
                if (_isInitialized) return;

                // 啟動處理線程
                _processTask = Task.Run(ProcessLogQueueAsync);
                _isInitialized = true;
                // 註冊應用程序域卸載事件

                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    FlushAll().GetAwaiter().GetResult();
                };

                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    FlushAll().GetAwaiter().GetResult();
                };
            }
        }

        /// <summary>
        /// 將日誌項目加入處理隊列
        /// </summary>
        /// <param name="level">日誌級別</param>
        /// <param name="name">日誌名稱</param>
        /// <param name="message">日誌消息</param>
        /// <param name="args">日誌參數</param>
        /// <param name="immediateFlush">是否需要立即寫入</param>
        public static void EnqueueLog(LogLevel level, string name, string message, object[] args, bool immediateFlush = false)
        {
            // 確保處理器已初始化
            if (!_isInitialized)
                Initialize();

            // 如果隊列已滿，進行同步寫入
            if (_logQueue.Count >= CurrentAsyncOptions.MaxQueueSize)
            {
                LogText.Add_LogText(level, name, message, args);
                return;
            }

            // 創建日誌項目
            var logItem = new LogItem
            {
                Level = level,
                Name = name,
                Message = message,
                Args = args,
                RequireImmediateFlush = immediateFlush
            };

            // 將日誌項目加入隊列
            _logQueue.Enqueue(logItem);
            // 釋放信號，通知處理線程
            _signal.Release();

            // 對於重要日誌，立即寫入
            if (immediateFlush || level >= LogLevel.Error)
            {
                FlushAll().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// 異步處理日誌隊列中的項目
        /// 這是處理線程的主要邏輯
        /// </summary>
        private static async Task ProcessLogQueueAsync()
        {
            // 持續處理，直到收到取消信號
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // 等待新的日誌或定時器觸發
                    await _signal.WaitAsync(TimeSpan.FromMilliseconds(CurrentAsyncOptions.FlushIntervalMs));

                    // 檢查是否需要定期寫入
                    if ((DateTime.Now - _lastFlushTime).TotalMilliseconds >= CurrentAsyncOptions.FlushIntervalMs)
                    {
                        await FlushAll();
                    }
                    else
                    {
                        // 處理單個日誌
                        await ProcessSingleLog();
                    }                  
                }
                catch (OperationCanceledException)
                {
                    // 處理取消操作
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"日誌處理任務發生錯誤: {ex.Message}");
                    // 發生錯誤時暫停一下，避免過度消耗資源
                    await Task.Delay(100);
                }
            }
            // 確保所有剩餘的日誌都被處理
            await FlushAll();
        }

        /// <summary>
        /// 處理單個日誌項
        /// </summary>
        private static async Task ProcessSingleLog()
        {
            if (_logQueue.TryDequeue(out LogItem logItem))
            {
                try
                {
                    LogText.Add_LogText(
                        logItem.Level,
                        logItem.Name,
                        logItem.Message,
                        logItem.Args);

                    if (logItem.RequireImmediateFlush)
                    {
                        await FlushAll();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"處理日誌時發生錯誤: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 強制寫入所有待處理的日誌
        /// </summary>
        private static async Task FlushAll()
        {
            try
            {
                int batchCount = 0;
                while (_logQueue.TryDequeue(out LogItem logItem) && batchCount < CurrentAsyncOptions.MaxBatchSize)
                {
                    try
                    {
                        LogText.Add_LogText(
                            logItem.Level,
                            logItem.Name,
                            logItem.Message,
                            logItem.Args);
                        batchCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"批次處理日誌時發生錯誤: {ex.Message}");
                    }
                }

                _lastFlushTime = DateTime.Now;

                // 如果還有剩餘的日誌，釋放信號繼續處理
                if (!_logQueue.IsEmpty)
                {
                    _signal.Release();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"強制寫入日誌時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 關閉異步日誌處理器
        /// 確保所有待處理的日誌都被寫入
        /// </summary>
        public static async Task ShutdownAsync()
        {
            if (!_isInitialized) return;

            try
            {
                // 處理所有剩餘的日誌
                await FlushAll();

                _cancellationTokenSource.Cancel();
                _signal.Release();

                if (_processTask != null)
                {
                    await Task.WhenAny(_processTask, Task.Delay(2000)); // 最多等待2秒
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"關閉日誌處理器時發生錯誤: {ex.Message}");
            }
            finally
            {
                _isInitialized = false;
            }
        }
    }
}