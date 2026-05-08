using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using MELILogger = Microsoft.Extensions.Logging.ILogger;
using Serilog;
using Serilog.Core;
using ZeroLog;
using ZeroLog.Appenders;
using ZeroLog.Configuration;
using ZeroLog.Formatting;
using ZLogger;

namespace OzaLog.Benchmarks;

/// <summary>
/// 共用 BenchmarkConfig：MemoryDiagnoser + 較短迭代次數（避免 file I/O benchmark 跑太久）
/// </summary>
public class BenchConfig : ManualConfig
{
    public BenchConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.Default
            .WithLaunchCount(1)
            .WithWarmupCount(1)
            .WithIterationCount(3));
    }
}

/// <summary>
/// 各 logger 的 setup / teardown 共用程式
/// </summary>
internal static class LoggerSetups
{
    private const string LogRoot = "logs-bench";

    public static string OzakDir => Path.Combine(LogRoot, "ozak");
    public static string ZLoggerPath => Path.Combine(LogRoot, "zlogger", "z.log");
    public static string ZeroLogDir => Path.Combine(LogRoot, "zerolog");
    public static string SerilogPath => Path.Combine(LogRoot, "serilog", "s.log");

    public static void EnsureDirs()
    {
        Directory.CreateDirectory(Path.Combine(LogRoot, "ozak"));
        Directory.CreateDirectory(Path.Combine(LogRoot, "zlogger"));
        Directory.CreateDirectory(Path.Combine(LogRoot, "zerolog"));
        Directory.CreateDirectory(Path.Combine(LogRoot, "serilog"));
    }

    // ── OzaLog ───────────────────────────────────────────────
    private static bool _ozakConfigured;
    public static void SetupOzak()
    {
        if (_ozakConfigured) return;
        _ozakConfigured = true;
        LOG.Configure(o =>
        {
            o.LogPath = OzakDir;
            o.SetFileSizeInMB(50);
            o.EnableAsyncLogging = true;
            o.EnableConsoleOutput = false;
            o.MaxOpenFileStreams = 200;
            o.DiskFlushIntervalMs = 100;
            o.ConfigureAsync(a =>
            {
                a.MaxBatchSize = 1000;
                a.MaxQueueSize = 100_000;   // 高 queue 容量避免 benchmark drop
                a.FlushIntervalMs = 50;
            });
        });
    }

    // ── ZLogger ───────────────────────────────────────────────────
    private static ILoggerFactory _zLoggerFactory;
    public static MELILogger ZLoggerInstance { get; private set; }

    public static void SetupZLogger()
    {
        if (_zLoggerFactory != null) return;
        // 已存在檔案則先刪掉，避免重跑時 append 累積影響
        try { if (File.Exists(ZLoggerPath)) File.Delete(ZLoggerPath); } catch { }

        _zLoggerFactory = LoggerFactory.Create(b =>
        {
            b.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            b.AddZLoggerFile(ZLoggerPath);
        });
        ZLoggerInstance = _zLoggerFactory.CreateLogger("Bench");
    }

    public static void TeardownZLogger()
    {
        _zLoggerFactory?.Dispose();
        _zLoggerFactory = null;
        ZLoggerInstance = null;
    }

    // ── ZeroLog ───────────────────────────────────────────────────
    public static ZeroLog.Log ZeroLogInstance { get; private set; }
    private static bool _zeroLogConfigured;
    public static void SetupZeroLog()
    {
        if (_zeroLogConfigured) return;
        _zeroLogConfigured = true;

        var appender = new DateAndSizeRollingFileAppender(ZeroLogDir);
        var config = new ZeroLogConfiguration
        {
            RootLogger =
            {
                Appenders = { appender },
                Level = ZeroLog.LogLevel.Trace,
            }
        };
        LogManager.Initialize(config);
        ZeroLogInstance = LogManager.GetLogger("Bench");
    }

    // ── Serilog ───────────────────────────────────────────────────
    public static Logger SerilogInstance { get; private set; }
    public static void SetupSerilog()
    {
        if (SerilogInstance != null) return;
        try { if (File.Exists(SerilogPath)) File.Delete(SerilogPath); } catch { }
        SerilogInstance = new Serilog.LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(SerilogPath, buffered: true, flushToDiskInterval: TimeSpan.FromMilliseconds(100))
            .CreateLogger();
    }

    public static void TeardownSerilog()
    {
        SerilogInstance?.Dispose();
        SerilogInstance = null;
    }
}
