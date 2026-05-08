using System;
using System.Threading;

namespace OzaLog.Core
{
    /// <summary>
    /// 時間戳快取 - 由背景執行緒以 1ms 間隔更新 volatile ticks，
    /// 高頻 log 呼叫端只需讀 long 值，避免每筆 log 都打 DateTime.Now 的 syscall。
    /// Timestamp cache - background thread updates a volatile ticks value at 1ms,
    /// hot log paths read the cached long instead of calling DateTime.Now.
    /// </summary>
    /// <remarks>
    /// 對 HFT 類場景至關重要：DateTime.Now 在 Windows 上會呼叫 GetSystemTimeAsFileTime
    /// 並轉時區，連續百萬次呼叫成本顯著。此快取以 1ms 精度為代價，把每筆 log 的時間取得
    /// 成本壓到一次 volatile read（~奈秒等級）。
    /// </remarks>
    internal static class TimestampCache
    {
        // volatile long 在 32-bit 平台上不是原子讀寫，但本快取允許短暫不一致（撕裂讀取最差也只是 1ms 內的時間值）
        // 為避免 32-bit 撕裂，所有讀寫透過 Interlocked。
        private static long _currentTicks = DateTime.Now.Ticks;
        private static Timer _timer;
        private static int _initialized;

        /// <summary>
        /// 取得當前快取的本地時間 ticks。第一次呼叫時會自動啟動背景更新計時器。
        /// </summary>
        public static long GetCurrentTicks()
        {
            EnsureInitialized();
            return Interlocked.Read(ref _currentTicks);
        }

        /// <summary>
        /// 取得當前快取對應的 DateTime（本地時間）。
        /// </summary>
        public static DateTime GetCurrentDateTime()
        {
            return new DateTime(GetCurrentTicks(), DateTimeKind.Local);
        }

        private static void EnsureInitialized()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
                return;

            // 立即更新一次以確保啟動精度
            Interlocked.Exchange(ref _currentTicks, DateTime.Now.Ticks);

            // 1ms 週期更新；Timer 用 thread pool，不額外建專屬 thread
            _timer = new Timer(static _ =>
            {
                Interlocked.Exchange(ref _currentTicks, DateTime.Now.Ticks);
            }, null, dueTime: 1, period: 1);
        }

        /// <summary>
        /// 停止快取計時器（用於 shutdown / 測試）
        /// </summary>
        internal static void Shutdown()
        {
            var timer = Interlocked.Exchange(ref _timer, null);
            timer?.Dispose();
            Interlocked.Exchange(ref _initialized, 0);
        }
    }
}
