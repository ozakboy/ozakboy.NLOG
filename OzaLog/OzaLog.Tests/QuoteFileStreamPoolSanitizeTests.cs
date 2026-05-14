using OzaLog.Core;
using Xunit;

namespace OzaLog.Tests
{
    /// <summary>
    /// v3.1+ QuoteFileStreamPool.Sanitize 驗證:把檔系統非法字元換成 '-'
    /// </summary>
    public class QuoteFileStreamPoolSanitizeTests
    {
        [Theory]
        [InlineData("BTCUSDT", "BTCUSDT")]                          // 純 alphanumeric 不動
        [InlineData("BTC-USDT-SWAP", "BTC-USDT-SWAP")]              // 連字號合法
        [InlineData("BTC/USDT", "BTC-USDT")]                        // / 換成 -
        [InlineData("BTC\\USDT", "BTC-USDT")]                       // \ 換成 -
        [InlineData("BTC:USDT", "BTC-USDT")]                        // : 換成 -
        [InlineData("BTC*USDT", "BTC-USDT")]                        // * 換成 -
        [InlineData("BTC?USDT", "BTC-USDT")]                        // ? 換成 -
        [InlineData("BTC|USDT", "BTC-USDT")]                        // | 換成 -
        [InlineData("BTC<USDT>", "BTC-USDT-")]                      // < > 都換成 -
        [InlineData("BTC\"USDT", "BTC-USDT")]                       // 雙引號 換成 -
        [InlineData("BTC/USDT:PERP", "BTC-USDT-PERP")]              // 多個非法字元
        public void Sanitize_ReplacesInvalidCharsWithDash(string input, string expected)
        {
            Assert.Equal(expected, QuoteFileStreamPool.Sanitize(input));
        }

        [Fact]
        public void Sanitize_EmptyOrNullPassesThrough()
        {
            Assert.Equal("", QuoteFileStreamPool.Sanitize(""));
            Assert.Null(QuoteFileStreamPool.Sanitize(null));
        }
    }
}
