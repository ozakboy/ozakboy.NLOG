using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace OzaLog.Core
{
    /// <summary>
    /// 報價 pipeline 的非同步處理器。獨立於主 logger 的 AsyncLogHandler。
    /// </summary>
    /// <remarks>
    /// v3.1+ 引入。
    /// • 預設關閉(QuoteOptions.Enable=false),使用者需在 Configure 內明確 opt-in
    /// • 啟動後建立獨立的 dispatcher Task + 獨立 disk flush timer
    /// • Backpressure:drop oldest,觸發 OnDropped callback(若有設定)
    /// • 收尾:ProcessExit 時並行於主 logger 的 flush(兩條 pipeline 互不阻塞)
    /// </remarks>
    internal static class QuoteLogHandler
    {
        private static readonly ConcurrentQueue<QuoteRecord> _queue = new ConcurrentQueue<QuoteRecord>();
        private static readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private static readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private static Task _processTask;
        private static int _initialized;
        private static Timer _diskFlushTimer;

        private static long _droppedCount;
        private static long _droppedCountAtLastCallback;

        public static long DroppedCount => Interlocked.Read(ref _droppedCount);

        /// <summary>
        /// 入隊。第一次呼叫時自動啟動 dispatcher(若 QuoteOptions.Enable=true)。
        /// 佇列滿時 drop oldest 並觸發 callback(若有設定)。
        /// </summary>
        public static void Enqueue(in QuoteRecord rec)
        {
            // 若使用者未 opt-in,不啟動 dispatcher,呼叫端視為 no-op(避免意外建立背景執行緒)
            if (!LogConfiguration.Current.QuoteOptions.Enable) return;

            if (Interlocked.CompareExchange(ref _initialized, 0, 0) == 0)
                Initialize();

            var quoteOpts = LogConfiguration.Current.QuoteOptions;
            if (_queue.Count >= quoteOpts.MaxQueueSize)
            {
                if (_queue.TryDequeue(out _))
                {
                    var dropped = Interlocked.Increment(ref _droppedCount);
                    var cb = quoteOpts.OnDropped;
                    if (cb != null)
                    {
                        try
                        {
                            var newlyDropped = dropped - Interlocked.Exchange(ref _droppedCountAtLastCallback, dropped);
                            cb(newlyDropped);
                        }
                        catch
                        {
                            // callback body 出錯不能拖累生產者
                        }
                    }
                }
            }

            _queue.Enqueue(rec);

            try { _signal.Release(); } catch (SemaphoreFullException) { }
        }

        private static void Initialize()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0) return;

            // 觸發其他背景元件
            _ = TimestampCache.GetCurrentTicks();

            _processTask = Task.Run(ProcessQueueAsync);

            var flushMs = LogConfiguration.Current.DiskFlushIntervalMs;
            _diskFlushTimer = new Timer(static _ => QuoteFileStreamPool.FlushAll(), null, flushMs, flushMs);

            AppDomain.CurrentDomain.ProcessExit += static (s, e) => ShutdownGracefully();
            AppDomain.CurrentDomain.UnhandledException += static (s, e) => ShutdownGracefully();
        }

        private static async Task ProcessQueueAsync()
        {
            var token = _cts.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var intervalMs = LogConfiguration.Current.QuoteOptions.FlushIntervalMs;
                    await _signal.WaitAsync(TimeSpan.FromMilliseconds(intervalMs)).ConfigureAwait(false);
                    DrainBatch();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"QuoteLogHandler.ProcessQueueAsync 錯誤: {ex.Message}");
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
            DrainBatch();
        }

        private static void DrainBatch()
        {
            var quoteOpts = LogConfiguration.Current.QuoteOptions;
            var batchLimit = quoteOpts.MaxBatchSize;
            var format = quoteOpts.OutputFormat;
            var processed = 0;

            while (processed < batchLimit && _queue.TryDequeue(out var rec))
            {
                try
                {
                    var line = QuoteFormatter.Format(in rec, format);
                    QuoteFileStreamPool.AppendLine(rec.Bucket, rec.Symbol, line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"QuoteLogHandler.DrainBatch 寫入錯誤: {ex.Message}");
                }
                processed++;
            }
        }

        public static async Task ShutdownAsync()
        {
            if (Interlocked.CompareExchange(ref _initialized, 0, 1) != 1) return;

            try
            {
                DrainBatch();
                _cts.Cancel();
                try { _signal.Release(); } catch (SemaphoreFullException) { }

                if (_processTask != null)
                    await Task.WhenAny(_processTask, Task.Delay(2000)).ConfigureAwait(false);

                QuoteFileStreamPool.FlushAll();
                QuoteFileStreamPool.Shutdown();

                var t = Interlocked.Exchange(ref _diskFlushTimer, null);
                t?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"QuoteLogHandler.ShutdownAsync 錯誤: {ex.Message}");
            }
        }

        private static void ShutdownGracefully()
        {
            try
            {
                DrainBatch();
                QuoteFileStreamPool.FlushAll();
            }
            catch
            {
                // 收尾時的錯誤吞掉
            }
        }
    }
}
