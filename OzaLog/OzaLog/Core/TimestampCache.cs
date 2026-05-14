using System;
using System.Diagnostics;
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
    ///
    /// v3.1+ 新增 HighPrecision 模式:每次更新快取時同步記錄當下的 Stopwatch.GetTimestamp(),
    /// 呼叫端讀取時用 Stopwatch 差值補回真實 µs 精度。代價是讀 ticks 從 ~5ns 增加到 ~30ns。
    /// 切換由 <c>LogConfiguration.Current.HighPrecisionTimestamp</c> 控制。
    /// </remarks>
    internal static class TimestampCache
    {
        // volatile long 在 32-bit 平台上不是原子讀寫，但本快取允許短暫不一致（撕裂讀取最差也只是 1ms 內的時間值）
        // 為避免 32-bit 撕裂，所有讀寫透過 Interlocked。
        private static long _currentTicks = DateTime.Now.Ticks;

        // v3.1: HighPrecision 模式下記錄 cache 更新瞬間的 Stopwatch 計數,
        // 呼叫端讀取時用 (now_sw - _swAtCacheUpdate) * (TimeSpan.TicksPerSecond / Stopwatch.Frequency) 補回真實精度
        private static long _swAtCacheUpdate;

        // Stopwatch.Frequency 每 tick 對應的 DateTime ticks(100ns)
        // 預先計算以避免每次讀都做除法
        private static readonly double _ticksPerSwTick = (double)TimeSpan.TicksPerSecond / Stopwatch.Frequency;

        private static Timer _timer;
        private static int _initialized;

        /// <summary>
        /// 取得當前快取的本地時間 ticks。第一次呼叫時會自動啟動背景更新計時器。
        /// HighPrecisionTimestamp = true 時走 Stopwatch hybrid 路徑提供 µs 級精度。
        /// </summary>
        public static long GetCurrentTicks()
        {
            EnsureInitialized();

            // 只有在 high-precision 模式下才做 Stopwatch 補正
            // 注意:這裡刻意避免讀取 LogConfiguration.Current 太頻繁(每筆 log 都讀)
            // 改在 cache 更新 timer 內讀,但這需要 stateful flag。
            // 折衷:每次都讀 — 但 LogConfiguration.Current 是 wrapper struct,實際讀取只是欄位存取
            if (LogConfiguration.Current.HighPrecisionTimestamp)
            {
                var baseTicks = Interlocked.Read(ref _currentTicks);
                var swBase = Interlocked.Read(ref _swAtCacheUpdate);
                if (swBase == 0) return baseTicks;

                var deltaSw = Stopwatch.GetTimestamp() - swBase;
                if (deltaSw < 0) return baseTicks; // 防 wraparound

                var deltaDtTicks = (long)(deltaSw * _ticksPerSwTick);
                return baseTicks + deltaDtTicks;
            }

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
            Interlocked.Exchange(ref _swAtCacheUpdate, Stopwatch.GetTimestamp());

            // 1ms 週期更新；Timer 用 thread pool，不額外建專屬 thread
            _timer = new Timer(static _ =>
            {
                // 兩個欄位一起更新,確保 hybrid 計算的基準點同步
                // 不需嚴格原子(允許 ~1ns 內讀到舊 sw 配新 ticks,誤差遠小於 1ms 快取精度本身)
                Interlocked.Exchange(ref _currentTicks, DateTime.Now.Ticks);
                Interlocked.Exchange(ref _swAtCacheUpdate, Stopwatch.GetTimestamp());
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
