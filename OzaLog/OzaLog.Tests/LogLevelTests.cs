using OzaLog.Core;
using Xunit;

namespace OzaLog.Tests
{
    public class LogLevelTests
    {
        [Fact]
        public void CustomName_TypoIsFixed_AndValueIs99()
        {
            // v3.0 修正：CostomName → CustomName，但保留枚舉值 99 以維持 wire compatibility
            Assert.Equal(99, (int)LogLevel.CustomName);
        }

        [Theory]
        [InlineData(LogLevel.Trace, 0)]
        [InlineData(LogLevel.Debug, 1)]
        [InlineData(LogLevel.Info, 2)]
        [InlineData(LogLevel.Warn, 3)]
        [InlineData(LogLevel.Error, 4)]
        [InlineData(LogLevel.Fatal, 5)]
        [InlineData(LogLevel.CustomName, 99)]
        public void EnumValues_StableAcrossVersions(LogLevel level, int expectedValue)
        {
            Assert.Equal(expectedValue, (int)level);
        }
    }
}
