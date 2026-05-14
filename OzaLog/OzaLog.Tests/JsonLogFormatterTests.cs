using System;
using OzaLog.Core;
using Xunit;

namespace OzaLog.Tests
{
    /// <summary>
    /// v3.1+ JsonLogFormatter NDJSON 輸出驗證
    /// </summary>
    public class JsonLogFormatterTests
    {
        [Fact]
        public void Format_ProducesValidNdjsonWithRequiredFields()
        {
            var ticks = new DateTime(2026, 5, 14, 10, 23, 45, 123, DateTimeKind.Local).Ticks;
            var item = new LogItem(LogLevel.Info, "BTC", "hello", null, ticks, 42, "Dispatcher", false);

            var line = JsonLogFormatter.Format(in item);

            Assert.StartsWith("{", line);
            Assert.EndsWith("}", line);
            Assert.Contains("\"ts\":", line);
            Assert.Contains("\"lv\":\"Info\"", line);
            Assert.Contains("\"nm\":\"BTC\"", line);
            Assert.Contains("\"msg\":\"hello\"", line);
            // 一行不應有換行(NDJSON 由 StreamWriter.WriteLine 補)
            Assert.DoesNotContain("\n", line);
        }

        [Fact]
        public void Format_AllLevelsProduceCorrectLvString()
        {
            var ticks = DateTime.Now.Ticks;
            foreach (var (level, expected) in new[]
            {
                (LogLevel.Trace, "Trace"),
                (LogLevel.Debug, "Debug"),
                (LogLevel.Info, "Info"),
                (LogLevel.Warn, "Warn"),
                (LogLevel.Error, "Error"),
                (LogLevel.Fatal, "Fatal"),
                (LogLevel.CustomName, "CustomName"),
            })
            {
                var item = new LogItem(level, "n", "m", null, ticks, 1, null, false);
                var line = JsonLogFormatter.Format(in item);
                Assert.Contains($"\"lv\":\"{expected}\"", line);
            }
        }

        [Fact]
        public void Format_AppliesArgsToMsgField()
        {
            var ticks = DateTime.Now.Ticks;
            var item = new LogItem(LogLevel.Info, "", "user {0} did {1}",
                new object[] { "alice", "login" }, ticks, 1, null, false);

            var line = JsonLogFormatter.Format(in item);

            Assert.Contains("\"msg\":\"user alice did login\"", line);
        }
    }
}
