using System;
using System.Threading;
using System.Threading.Tasks;

namespace OzaLog.Core
{
    /// <summary>
    /// 全域意外攔截器 - 訂閱 AppDomain.UnhandledException 與 TaskScheduler.UnobservedTaskException，
    /// 將任何未被攔截的例外以 Fatal 等級同步寫入 log（含 immediate flush 確保 crash 前落盤）。
    /// Global exception capture - logs any unhandled exception synchronously with immediate flush.
    /// </summary>
    /// <remarks>
    /// v3.0 引入：使用者透過 <c>LogOptions.EnableGlobalExceptionCapture = true</c> 啟用，預設 false。
    /// 函式庫不主動接管全域例外處理，避免與宿主應用既有 handler 衝突；採明確 opt-in 設計。
    /// 不涵蓋 WPF DispatcherUnhandledException / WinForms ThreadException / ASP.NET Core 等 UI/middleware 路徑，
    /// 因為函式庫層級無法取得這些 framework 物件，需由宿主應用自行 hook。
    /// </remarks>
    internal static class GlobalExceptionCapture
    {
        private const string LogName = "Unhandled";
        private static int _enabled;

        /// <summary>
        /// 啟用全域意外攔截（idempotent，重複呼叫無副作用）。
        /// </summary>
        public static void Enable()
        {
            if (Interlocked.CompareExchange(ref _enabled, 1, 0) != 0) return;

            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        /// <summary>
        /// 停用全域意外攔截。
        /// </summary>
        public static void Disable()
        {
            if (Interlocked.CompareExchange(ref _enabled, 0, 1) != 1) return;

            AppDomain.CurrentDomain.UnhandledException -= OnAppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        }

        public static bool IsEnabled => Interlocked.CompareExchange(ref _enabled, 0, 0) == 1;

        private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                var message = "[Unhandled in AppDomain"
                    + (e.IsTerminating ? " (terminating)" : string.Empty)
                    + "]";
                WriteFatal(message, ex);
            }
            catch
            {
                // crash 路徑中不能再丟例外
            }
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                WriteFatal("[Unobserved Task Exception]", e.Exception);
                // 不呼叫 e.SetObserved()：保留宿主應用既有的觀察行為，
                // 由使用者自行決定是否吞掉例外。
            }
            catch
            {
            }
        }

        private static void WriteFatal(string headerMessage, Exception ex)
        {
            // 走 LogObject 等價路徑：序列化異常為 JSON 並同步 + immediate flush 寫入
            string body;
            if (ex != null)
            {
                try { body = LogSerializer.SerializeException(ex); }
                catch (Exception serEx) { body = ExceptionHandler.HandleSerializationException(serEx); }
            }
            else
            {
                body = "(no exception object)";
            }

            var currentThread = Thread.CurrentThread;
            var item = new LogItem(
                level: LogLevel.Fatal,
                name: LogName,
                message: headerMessage + "\n" + body,
                args: null,
                timestampTicks: TimestampCache.GetCurrentTicks(),
                threadId: currentThread.ManagedThreadId,
                threadName: currentThread.Name,
                requireImmediateFlush: true);

            // 直接走 sync 路徑（不入隊），確保 crash 前已落盤
            LogText.Write(in item);
            FileStreamPool.Flush(LogLevel.Fatal, LogName);
        }
    }
}
