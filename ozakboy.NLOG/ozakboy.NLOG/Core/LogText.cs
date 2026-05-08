using System;
using System.Threading;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌寫入入口 - v3.0 改為 FileStreamPool 的薄包裝，
    /// 不再持有單一全域 lock，不再每筆 open/close 檔案。
    /// Log file writer - thin wrapper over FileStreamPool in v3.0.
    /// </summary>
    internal static class LogText
    {
        /// <summary>
        /// 同步寫入單筆 log。v3.0 由 dispatcher 執行緒呼叫，或在 EnableAsyncLogging=false 時由呼叫端直接呼叫。
        /// </summary>
        internal static void Write(in LogItem item)
        {
            try
            {
                var line = LogFormatter.Format(in item);
                FileStreamPool.AppendLine(item.Level, item.Name, line);
                if (item.RequireImmediateFlush)
                    FileStreamPool.Flush(item.Level, item.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}>>LogText.Write 錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// v2.x 相容入口 - 同步寫入路徑（EnableAsyncLogging=false 時呼叫）。
        /// 內部會建構 LogItem 並呼叫 <see cref="Write(in LogItem)"/>。
        /// </summary>
        internal static void Add_LogText(LogLevel level, string name, string message, object[] args)
        {
            var item = new LogItem(
                level: level,
                name: name ?? string.Empty,
                message: message,
                args: (args != null && args.Length > 0) ? args : null,
                timestampTicks: TimestampCache.GetCurrentTicks(),
                threadId: Thread.CurrentThread.ManagedThreadId,
                requireImmediateFlush: false);
            Write(in item);
        }
    }
}
