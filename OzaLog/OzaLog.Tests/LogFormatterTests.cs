using System;
using OzaLog.Core;
using Xunit;

namespace OzaLog.Tests
{
    public class LogFormatterTests
    {
        [Fact]
        public void Format_ProducesExpectedTimestampLayout()
        {
            var ticks = new DateTime(2026, 5, 8, 9, 7, 5, 123, DateTimeKind.Local).Ticks;
            var item = new LogItem(LogLevel.Info, "", "hello", null, ticks, 42, false);
            var line = LogFormatter.Format(in item);

            Assert.StartsWith("09:07:05.123[T:42] ", line);
            Assert.EndsWith("hello", line);
        }

        [Fact]
        public void Format_AppliesArgsFormatting()
        {
            var ticks = DateTime.Now.Ticks;
            var item = new LogItem(LogLevel.Info, "", "user {0} did {1}", new object[] { "alice", "login" }, ticks, 1, false);
            var line = LogFormatter.Format(in item);

            Assert.Contains("user alice did login", line);
        }

        [Fact]
        public void Format_GracefullyHandlesBadFormatString()
        {
            var ticks = DateTime.Now.Ticks;
            // {2} 超出 args 範圍 → 應 fallback 為原字串而非拋例外
            var item = new LogItem(LogLevel.Info, "", "broken {2}", new object[] { "x" }, ticks, 1, false);
            var line = LogFormatter.Format(in item);

            Assert.Contains("broken {2}", line);
        }

        [Fact]
        public void EscapeMessage_LeavesNumericPlaceholdersAlone()
        {
            Assert.Equal("user {0} did {1}", LogFormatter.EscapeMessage("user {0} did {1}"));
        }

        [Fact]
        public void EscapeMessage_DoublesUnpairedBraces()
        {
            Assert.Equal("plain {{text}}", LogFormatter.EscapeMessage("plain {text}"));
        }
    }
}
