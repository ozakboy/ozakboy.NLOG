using System;
using System.Collections.Generic;
using OzaLog;
using OzaLog.Core;
using Xunit;

namespace OzaLog.Tests
{
    /// <summary>
    /// v3.1+ QuoteFormatter 三種輸出格式 + 驗證邏輯測試
    /// </summary>
    public class QuoteFormatterTests
    {
        private static long FixedTicks => new DateTime(2026, 5, 14, 10, 23, 45, 123, DateTimeKind.Local).Ticks;

        [Fact]
        public void Format_Txt_OutputsHumanReadableKvLine()
        {
            var rec = new QuoteRecord(
                symbol: "BTCUSDT",
                bucket: "binance_spot",
                ticks: FixedTicks,
                last: 60123.5m,
                bid: 60123.0m,
                ask: 60124.0m);

            var line = QuoteFormatter.Format(in rec, QuoteOutputFormat.Txt);

            Assert.Contains("[2026-05-14 10:23:45.123]", line);
            Assert.Contains("binance_spot BTCUSDT", line);
            Assert.Contains("last=60123.5", line);
            Assert.Contains("bid=60123.0", line);
            Assert.Contains("ask=60124.0", line);
        }

        [Fact]
        public void Format_Txt_SkipsNullOptionalFields()
        {
            var rec = new QuoteRecord(
                symbol: "ETHUSDT",
                bucket: "binance_spot",
                ticks: FixedTicks,
                last: 3000m);

            var line = QuoteFormatter.Format(in rec, QuoteOutputFormat.Txt);

            Assert.Contains("last=3000", line);
            Assert.DoesNotContain("bid=", line);
            Assert.DoesNotContain("ask=", line);
            Assert.DoesNotContain("open=", line);
            Assert.DoesNotContain("volume=", line);
        }

        [Fact]
        public void Format_Json_OutputsNDJSONWithEpochMs()
        {
            var rec = new QuoteRecord(
                symbol: "BTCUSDT",
                bucket: "binance_spot",
                ticks: FixedTicks,
                last: 60123.5m,
                bid: 60123.0m,
                bidQty: 0.5m,
                ask: 60124.0m,
                askQty: 1.2m);

            var line = QuoteFormatter.Format(in rec, QuoteOutputFormat.Json);

            Assert.StartsWith("{", line);
            Assert.EndsWith("}", line);
            Assert.Contains("\"symbol\":\"BTCUSDT\"", line);
            Assert.Contains("\"bucket\":\"binance_spot\"", line);
            Assert.Contains("\"last\":60123.5", line);
            Assert.Contains("\"bid\":60123.0", line);
            Assert.Contains("\"bidQty\":0.5", line);
            Assert.Contains("\"ask\":60124.0", line);
            Assert.Contains("\"askQty\":1.2", line);

            // ts 應為 epoch_ms 整數
            Assert.Contains("\"ts\":", line);
            // 不含 thread 資訊(Quote 不寫 tid/tn)
            Assert.DoesNotContain("\"tid\"", line);
            Assert.DoesNotContain("\"tn\"", line);
        }

        [Fact]
        public void Format_Json_SkipsNullOptionalFields()
        {
            var rec = new QuoteRecord(
                symbol: "ETHUSDT",
                bucket: "binance_spot",
                ticks: FixedTicks,
                last: 3000m);

            var line = QuoteFormatter.Format(in rec, QuoteOutputFormat.Json);

            Assert.Contains("\"last\":3000", line);
            Assert.DoesNotContain("\"bid\"", line);
            Assert.DoesNotContain("\"ask\"", line);
            Assert.DoesNotContain("\"open\"", line);
        }

        [Fact]
        public void Format_Json_ExtrasNestedInExtrasObject()
        {
            var extras = new Dictionary<string, object>
            {
                ["funding"] = 0.0001m,
                ["mark"] = 60100m,
            };
            var rec = new QuoteRecord(
                symbol: "BTCUSDT",
                bucket: "binance_perp",
                ticks: FixedTicks,
                last: 60123.5m,
                extras: extras);

            var line = QuoteFormatter.Format(in rec, QuoteOutputFormat.Json);

            // Extras 應該 nested 在 "extras":{...} 而不是 top level
            Assert.Contains("\"extras\":{", line);
            Assert.Contains("\"funding\":0.0001", line);
            Assert.Contains("\"mark\":60100", line);
        }

        [Fact]
        public void Format_ThrowsWhenExtrasAndExtrasJsonBothSet()
        {
            var rec = new QuoteRecord(
                symbol: "BTCUSDT",
                bucket: "x",
                ticks: FixedTicks,
                last: 1m,
                extras: new Dictionary<string, object> { ["a"] = 1 },
                extrasJson: "{\"b\":2}");

            var ex = Assert.Throws<ArgumentException>(() => QuoteFormatter.Format(in rec, QuoteOutputFormat.Txt));
            Assert.Contains("不可同時設定", ex.Message);
        }

        [Fact]
        public void Format_ThrowsWhenExtrasKeyCollidesWithReservedKey()
        {
            var rec = new QuoteRecord(
                symbol: "BTCUSDT",
                bucket: "x",
                ticks: FixedTicks,
                last: 1m,
                extras: new Dictionary<string, object> { ["bid"] = 999m });

            Assert.Throws<ArgumentException>(() => QuoteFormatter.Format(in rec, QuoteOutputFormat.Json));
        }

        [Fact]
        public void Format_Log_SameContentAsTxt()
        {
            var rec = new QuoteRecord(
                symbol: "BTCUSDT",
                bucket: "binance_spot",
                ticks: FixedTicks,
                last: 60123.5m);

            var txtLine = QuoteFormatter.Format(in rec, QuoteOutputFormat.Txt);
            var logLine = QuoteFormatter.Format(in rec, QuoteOutputFormat.Log);

            Assert.Equal(txtLine, logLine);
        }
    }
}
