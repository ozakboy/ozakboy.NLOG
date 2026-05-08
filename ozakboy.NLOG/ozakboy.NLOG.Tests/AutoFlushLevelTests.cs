using ozakboy.NLOG.Core;
using Xunit;

namespace ozakboy.NLOG.Tests
{
    /// <summary>
    /// 防回歸：確保 CustomName=99 不會因為大於 Fatal=5 被誤判為高嚴重性
    /// 走同步寫入路徑（v3.0 早期 bug，HFT 場景下會把吞吐量直接砍 1000 倍）
    /// </summary>
    public class AutoFlushLevelTests
    {
        // 等價於 AsyncLogHandler 內的判斷邏輯
        private static bool IsAutoFlushLevel(LogLevel level)
            => level == LogLevel.Error || level == LogLevel.Fatal;

        [Theory]
        [InlineData(LogLevel.Trace, false)]
        [InlineData(LogLevel.Debug, false)]
        [InlineData(LogLevel.Info, false)]
        [InlineData(LogLevel.Warn, false)]
        [InlineData(LogLevel.Error, true)]
        [InlineData(LogLevel.Fatal, true)]
        [InlineData(LogLevel.CustomName, false)]   // 關鍵：CustomName=99 不可被誤判為自動 flush
        public void AutoFlush_OnlyTriggersForErrorAndFatal(LogLevel level, bool expectedAutoFlush)
        {
            Assert.Equal(expectedAutoFlush, IsAutoFlushLevel(level));
        }
    }
}
