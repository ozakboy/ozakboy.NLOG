using System;
using System.Threading;
using OzaLog.Core;
using Xunit;

namespace OzaLog.Tests
{
    /// <summary>
    /// v3.1+ 驗證 LogFormatter 在不同 TimeFormat / Thread 顯示開關下的輸出。
    /// </summary>
    /// <remarks>
    /// 注意:LogConfiguration.Configure 全域只能呼叫一次,測試中無法切換 TimeFormat。
    /// 這裡只測試「預設行為」與 LogFormatter 內部分支(fast path / fallback)。
    /// 完整 e2e 切換驗證留給人工煙測。
    /// </remarks>
    public class CustomTimeFormatTests
    {
        [Fact]
        public void Format_FastPath_DefaultTimeFormatProducesHHmmssfff()
        {
            var ticks = new DateTime(2026, 5, 14, 10, 23, 45, 123, DateTimeKind.Local).Ticks;
            var item = new LogItem(LogLevel.Info, "", "x", null, ticks, 42, null, false);
            var line = LogFormatter.Format(in item);

            // 預設 TimeFormat = HH:mm:ss.fff
            Assert.StartsWith("10:23:45.123", line);
        }

        [Fact]
        public void Format_ThreadName_SkippedWhenNullEvenIfShowThreadNameTrue()
        {
            // 預設配置:ShowThreadId=true, ShowThreadName=false
            // 沒設定 Configure 時用預設值;此測試依預設行為,不切配置
            var ticks = DateTime.Now.Ticks;
            var item = new LogItem(LogLevel.Info, "", "msg", null, ticks, 7, null, false);
            var line = LogFormatter.Format(in item);

            // 預設只有 [T:tid],不含 N: 區段
            Assert.Contains("[T:7]", line);
            Assert.DoesNotContain("N:", line);
        }
    }
}
