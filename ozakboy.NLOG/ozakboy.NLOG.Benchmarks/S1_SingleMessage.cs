using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace ozakboy.NLOG.Benchmarks;

/// <summary>
/// S1：單筆短訊息（無格式參數）— 測呼叫端 enqueue 開銷
/// 4 個 logger 各自跑同一句 LogInfo。BenchmarkDotNet 自動測 ns/op + 分配 bytes
/// </summary>
[Config(typeof(BenchConfig))]
[MemoryDiagnoser]
public class S1_SingleMessage
{
    private const string Msg = "hello bench";

    [GlobalSetup]
    public void Setup()
    {
        LoggerSetups.EnsureDirs();
        LoggerSetups.SetupOzak();
        LoggerSetups.SetupZLogger();
        LoggerSetups.SetupZeroLog();
        LoggerSetups.SetupSerilog();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        LoggerSetups.TeardownZLogger();
        LoggerSetups.TeardownSerilog();
    }

    [Benchmark(Baseline = true)]
    public void Ozakboy_NLOG()
    {
        LOG.Info_Log(Msg);
    }

    [Benchmark]
    public void ZLogger_Lib()
    {
        LoggerSetups.ZLoggerInstance.ZLogInformation($"hello bench");
    }

    [Benchmark]
    public void ZeroLog_Lib()
    {
        LoggerSetups.ZeroLogInstance.Info(Msg);
    }

    [Benchmark]
    public void Serilog_Lib()
    {
        LoggerSetups.SerilogInstance.Information(Msg);
    }
}
