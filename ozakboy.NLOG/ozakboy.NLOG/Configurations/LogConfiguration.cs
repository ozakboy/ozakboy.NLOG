using ozakboy.NLOG.Core;
using System;
using static ozakboy.NLOG.LogConfiguration;

namespace ozakboy.NLOG
{
    /// <summary>
    /// LOG配置類別
    /// </summary>
    public static class LogConfiguration
    {

        private static readonly LogOptions _currentOptions = new LogOptions();
        private static bool _isInitialized = false;
        public static ILogOptions Current => new ReadOnlyLogOptions(_currentOptions);
        public interface ILogOptions
        {
            int KeepDays { get; }
            long MaxFileSize { get; }
            string LogPath { get; }
            ILogTypeDirectories TypeDirectories { get; }
            bool EnableAsyncLogging { get; }
            bool EnableCompression { get; }
            bool EnableConsoleOutput { get; }
            IAsyncLogOptions AsyncOptions { get; }
        }

        private class ReadOnlyLogOptions : ILogOptions
        {
            private readonly LogOptions _options;

            public ReadOnlyLogOptions(LogOptions options)
            {
                _options = options;
            }

            public int KeepDays => _options.KeepDays;
            public long MaxFileSize => _options.MaxFileSize;
            public string LogPath => _options.LogPath;
            public bool EnableAsyncLogging => _options.EnableAsyncLogging;
            public bool EnableConsoleOutput => _options.EnableConsoleOutput;
            public ILogTypeDirectories TypeDirectories =>new ReadOnlyLogTypeDirectories(_options.TypeDirectories);
            public IAsyncLogOptions AsyncOptions => new ReadOnlyAsyncLogOptions(_options.AsyncOptions);
        }

        public interface ILogTypeDirectories
        {
           string  DirectoryPath { get; }
           string TracePath { get; }
           string DebugPath { get; }
           string InfoPath { get; }
           string WarnPath { get; }
           string ErrorPath { get; }
           string FatalPath { get; }
           string CustomPath { get; }
            string GetPathForType(LogLevel logLevel);
        }

        private class ReadOnlyLogTypeDirectories : ILogTypeDirectories
        {
            private readonly LogTypeDirectories _TypeDirectories;
            public ReadOnlyLogTypeDirectories(LogTypeDirectories TypeDirectories)
            {
                _TypeDirectories = TypeDirectories;
            }
            public string DirectoryPath => _TypeDirectories.DirectoryPath;
            public string TracePath =>  _TypeDirectories.TracePath;
            public string DebugPath =>  _TypeDirectories.DebugPath;
            public string InfoPath =>  _TypeDirectories.InfoPath;
            public string WarnPath =>  _TypeDirectories.WarnPath;
            public string ErrorPath =>  _TypeDirectories.ErrorPath;
            public string FatalPath =>  _TypeDirectories.FatalPath;
            public string CustomPath =>  _TypeDirectories.CustomPath;

            /// <summary>
            /// 取得指定日誌類型的目錄路徑
            /// </summary>
            /// <param name="logLevel">日誌類型</param>
            /// <returns>目錄路徑</returns>
            public string GetPathForType(LogLevel logLevel)
            {

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        return TracePath ?? DirectoryPath;
                    case LogLevel.Debug:
                        return DebugPath ?? DirectoryPath;
                    case LogLevel.Info:
                        return InfoPath ?? DirectoryPath;
                    case LogLevel.Warn:
                        return WarnPath ?? DirectoryPath;
                    case LogLevel.Error:
                        return ErrorPath ?? DirectoryPath;
                    case LogLevel.Fatal:
                        return FatalPath ?? DirectoryPath;
                    case LogLevel.CostomName:
                        return CustomPath ?? DirectoryPath;
                    default:
                        return CustomPath ?? DirectoryPath;
                }
            }
        }

        /// <summary>
        /// 異步日誌配置接口
        /// </summary>
        public interface IAsyncLogOptions
        {
            /// <summary>
            /// 批次處理的最大日誌數量
            /// </summary>
            int MaxBatchSize { get; }

            /// <summary>
            /// 隊列的最大容量
            /// </summary>
            int MaxQueueSize { get; }

            /// <summary>
            /// 定期寫入的時間間隔（毫秒）
            /// </summary>
            int FlushIntervalMs { get; }
        }

        private class ReadOnlyAsyncLogOptions : IAsyncLogOptions
        {
            private readonly AsyncLogOptions _options;

            public ReadOnlyAsyncLogOptions(AsyncLogOptions options)
            {
                _options = options;
            }

            public int MaxBatchSize => _options.MaxBatchSize;
            public int MaxQueueSize => _options.MaxQueueSize;
            public int FlushIntervalMs => _options.FlushIntervalMs;
        }

        /// <summary>
        /// 異步日誌配置選項
        /// </summary>
        public class AsyncLogOptions
        {
            private int _maxBatchSize = 100;
            private int _maxQueueSize = 10000;
            private int _flushIntervalMs = 1000;

            /// <summary>
            /// 批次處理的最大日誌數量
            /// 默認值：100
            /// 最小值：1，最大值：1000
            /// </summary>
            public int MaxBatchSize
            {
                get => _maxBatchSize;
                set => _maxBatchSize = Math.Max(1, Math.Min(1000, value));
            }

            /// <summary>
            /// 隊列的最大容量
            /// 默認值：10000
            /// 最小值：1000，最大值：100000
            /// </summary>
            public int MaxQueueSize
            {
                get => _maxQueueSize;
                set => _maxQueueSize = Math.Max(1000, Math.Min(100000, value));
            }

            /// <summary>
            /// 定期寫入的時間間隔（毫秒）
            /// 默認值：1000ms
            /// 最小值：100ms，最大值：10000ms
            /// </summary>
            public int FlushIntervalMs
            {
                get => _flushIntervalMs;
                set => _flushIntervalMs = Math.Max(100, Math.Min(10000, value));
            }
        }


        /// <summary>
        /// 日誌配置選項
        /// </summary>
        public class LogOptions
        {
            /// <summary>
            /// 日誌檔案保存天數設定，預設為 -3 天（保存最近3天的日誌）
            /// 請設定為負數，例如 -7 表示保存最近 7 天的日誌
            /// （請使用負數）
            /// </summary>
            public int KeepDays { get; set; } = -3;

            /// <summary>
            /// 單個日誌檔案的最大大小限制，超過此大小將自動分割檔案
            /// 預設為 50MB (50 * 1024 * 1024 bytes)
            /// </summary>
            public long MaxFileSize { get; set; } = 50 * 1024 * 1024;

            /// <summary>
            /// 日誌檔案根目錄
            /// 默認為 logs
            /// </summary>
            public string LogPath { get; set; } = "logs";

            /// <summary>
            /// 日誌類型目錄設定
            /// 可為每種日誌類型配置不同的子目錄
            /// </summary>
            public LogTypeDirectories TypeDirectories { get; set; } = new LogTypeDirectories();

            /// <summary>
            /// 是否啟用異步寫入
            /// </summary>
            public bool EnableAsyncLogging { get; set; } = true;

            /// <summary>
            /// 是否在控制台輸出
            /// </summary>
            public bool EnableConsoleOutput { get; set; } = true;

            /// <summary>
            /// 設定檔案大小的便捷方法（以 MB 為單位）
            /// </summary>
            /// <param name="megabytes">檔案大小（MB）</param>
            public void SetFileSizeInMB(int megabytes)
            {
                MaxFileSize = megabytes * 1024L * 1024L;
            }

            /// <summary>
            /// 異步日誌配置
            /// </summary>
            public AsyncLogOptions AsyncOptions { get; set; } = new AsyncLogOptions();

            /// <summary>
            /// 設定異步日誌配置
            /// </summary>
            /// <param name="configure">配置動作</param>
            public void ConfigureAsync(Action<AsyncLogOptions> configure)
            {
                configure?.Invoke(AsyncOptions);
            }
        }

        /// <summary>
        /// 日誌類型目錄配置
        /// 用於配置不同類型日誌的存放目錄
        /// </summary>
        public class LogTypeDirectories
        {
            /// <summary>
            /// 所有日誌類型的預設目錄
            /// 預設為 LogFiles
            /// </summary>
            public string DirectoryPath { get; set; } = "LogFiles";

            /// <summary>
            /// 追蹤日誌目錄，若為空則使用 DirectoryPath
            /// </summary>
            public string TracePath { get; set; }

            /// <summary>
            /// 除錯日誌目錄，若為空則使用 DirectoryPath
            /// </summary>
            public string DebugPath { get; set; }

            /// <summary>
            /// 一般資訊日誌目錄，若為空則使用 DirectoryPath
            /// </summary>
            public string InfoPath { get; set; }

            /// <summary>
            /// 警告日誌目錄，若為空則使用 DirectoryPath
            /// </summary>
            public string WarnPath { get; set; }

            /// <summary>
            /// 錯誤日誌目錄，若為空則使用 DirectoryPath
            /// </summary>
            public string ErrorPath { get; set; }

            /// <summary>
            /// 致命錯誤日誌目錄，若為空則使用 DirectoryPath
            /// </summary>
            public string FatalPath { get; set; }

            /// <summary>
            /// 自定義日誌目錄，若為空則使用 DirectoryPath
            /// </summary>
            public string CustomPath { get; set; }

      
        }

        /// <summary>
        /// 配置日誌系統
        /// </summary>
        /// <param name="configure">配置動作</param>
        public static void Initialize(Action<LogOptions> configure)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("ozakboy.LOG已初始化");
            }
            configure?.Invoke(_currentOptions);
            _isInitialized = true;
        }

        /// <summary>
        /// 取得當前配置
        /// </summary>
        public static ILogOptions GetCurrentOptions()
        {
            return Current;
        }

        /// <summary>
        /// 確保配置已初始化
        /// </summary>
        internal static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                Initialize(_ => { }); // 使用預設配置
            }
        }
    }
}