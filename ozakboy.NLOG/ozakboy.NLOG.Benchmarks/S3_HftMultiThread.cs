using System.Threading;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace ozakboy.NLOG.Benchmarks;

/// <summary>
/// S3：HFT 多執行緒多商品 — 8 thread × 50 商品 × 2000 logs each = 800K 筆。
/// 量級調整為 800K（而非規格寫的 8M）以保持單次 iteration 在合理時間內。
/// 測整體 throughput（時間越短越好）+ 分配/GC counts。
/// </summary>
[Config(typeof(BenchConfig))]
[MemoryDiagnoser]
public class S3_HftMultiThread
{
    private const int ThreadCount = 8;
    private const int ProductCount = 50;
    private const int LogsPerThread = 2000;
    private string[] _products;

    [GlobalSetup]
    public void Setup()
    {
        LoggerSetups.EnsureDirs();
        LoggerSetups.SetupOzak();
        LoggerSetups.SetupZLogger();
        LoggerSetups.SetupZeroLog();
        LoggerSetups.SetupSerilog();
        _products = new string[ProductCount];
        for (int i = 0; i < ProductCount; i++) _products[i] = $"PROD{i:D3}";
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        LoggerSetups.TeardownZLogger();
        LoggerSetups.TeardownSerilog();
    }

    [Benchmark(Baseline = true)]
    public void Ozakboy_NLOG() => RunMultiThread(static (ti, pi, _) =>
    {
        var name = $"PROD{pi:D3}";
        LOG.CustomName_Log(name, $"t{ti} tick:{pi}");
    });

    [Benchmark]
    public void ZLogger_Lib() => RunMultiThread(static (ti, pi, _) =>
    {
        LoggerSetups.ZLoggerInstance.ZLogInformation($"PROD{pi:D3} t{ti} tick:{pi}");
    });

    [Benchmark]
    public void ZeroLog_Lib() => RunMultiThread(static (ti, pi, _) =>
    {
        LoggerSetups.ZeroLogInstance.Info().Append("PROD").Append(pi).Append(" t").Append(ti).Append(" tick:").Append(pi).Log();
    });

    [Benchmark]
    public void Serilog_Lib() => RunMultiThread(static (ti, pi, _) =>
    {
        LoggerSetups.SerilogInstance.Information("{Product} t{Thread} tick:{Tick}", $"PROD{pi:D3}", ti, pi);
    });

    private static void RunMultiThread(System.Action<int, int, int> logAction)
    {
        var threads = new Thread[ThreadCount];
        for (int t = 0; t < ThreadCount; t++)
        {
            int threadIdx = t;
            threads[t] = new Thread(() =>
            {
                int seed = threadIdx * 1000 + 7;
                for (int i = 0; i < LogsPerThread; i++)
                {
                    int productIdx = (seed + i) % ProductCount;
                    logAction(threadIdx, productIdx, i);
                }
            }) { IsBackground = true, Name = $"bench-{t}" };
            threads[t].Start();
        }
        foreach (var th in threads) th.Join();
    }
}
