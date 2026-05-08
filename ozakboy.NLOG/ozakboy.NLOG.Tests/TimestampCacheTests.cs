using System;
using System.Threading;
using ozakboy.NLOG.Core;
using Xunit;

namespace ozakboy.NLOG.Tests
{
    public class TimestampCacheTests
    {
        [Fact]
        public void GetCurrentTicks_ReturnsRecentTimestamp()
        {
            // 給快取至少 5ms 來啟動背景 timer
            var t0 = TimestampCache.GetCurrentTicks();
            Thread.Sleep(20);
            var t1 = TimestampCache.GetCurrentTicks();

            Assert.True(t1 >= t0, "ticks should be monotonically non-decreasing");

            var diff = new DateTime(t1) - new DateTime(t0);
            Assert.True(diff.TotalMilliseconds < 1000, $"diff should be < 1s but was {diff.TotalMilliseconds}ms");
        }

        [Fact]
        public void GetCurrentDateTime_NearCurrentLocalTime()
        {
            var actual = DateTime.Now;
            var cached = TimestampCache.GetCurrentDateTime();

            // 快取最多落後 ~1ms（背景 tick 間隔）+ 一些 jitter；給 200ms 容忍
            var diff = actual - cached;
            Assert.True(Math.Abs(diff.TotalMilliseconds) < 200,
                $"cached ts {cached:HH:mm:ss.fff} differs too much from actual {actual:HH:mm:ss.fff}");
        }
    }
}
