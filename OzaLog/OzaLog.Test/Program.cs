using OzaLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

// Windows cmd 預設 OEM encoding 會把中文輸出成亂碼，強制改 UTF-8
Console.OutputEncoding = Encoding.UTF8;

// ═══════════════════════════════════════════════════════════════════════════
// Ozakboy.NLOG v3.0 — Console smoke / 壓測 程式
// 涵蓋：所有 LogLevel、格式化、物件序列化、異常記錄、CustomName、
//      HFT 多執行緒多商品壓測、LRU 觸發、queue 飽和 drop、immediateFlush、
//      Sync 模式 sanity check
// ═══════════════════════════════════════════════════════════════════════════

Header("Ozakboy.NLOG v3.0 console smoke test");
Console.WriteLine($"PID:          {Environment.ProcessId}");
Console.WriteLine($"BaseDir:      {AppContext.BaseDirectory}");
Console.WriteLine($"Runtime:      {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
Console.WriteLine();

// ─── 配置 ────────────────────────────────────────────────────────────────
// HFT 取向：高 batch、短 flush、適度 queue
long droppedCount = 0;
LOG.Configure(o =>
{
    o.KeepDays = -7;
    o.SetFileSizeInMB(10);
    o.EnableAsyncLogging = true;
    o.EnableConsoleOutput = false;          // console 輸出大量 spam 會干擾報告
    o.MaxOpenFileStreams = 50;              // 故意低於後續 100 商品數，測 LRU
    o.DiskFlushIntervalMs = 100;
    o.EnableGlobalExceptionCapture = false; // 預設 OFF（演示用）
    o.OnDropped = () => Interlocked.Increment(ref droppedCount);
    o.ConfigureAsync(a =>
    {
        a.MaxBatchSize = 500;
        a.MaxQueueSize = 5000;              // 故意小，方便壓測 drop
        a.FlushIntervalMs = 100;
    });
});

// ─── 1. 各 LogLevel 基本寫入 ─────────────────────────────────────────────
Header("1. 各 LogLevel 基本寫入");
LOG.Trace_Log("trace 級別測試");
LOG.Debug_Log("debug 級別測試");
LOG.Info_Log("info  級別測試");
LOG.Warn_Log("warn  級別測試");
LOG.Error_Log("error 級別測試");
LOG.Fatal_Log("fatal 級別測試");
Tick("已寫入 6 種 LogLevel");

// ─── 2. 格式化參數 ───────────────────────────────────────────────────────
Header("2. 格式化參數");
LOG.Info_Log("user {0} performed {1}", new[] { "alice", "login" });
LOG.Warn_Log("retry {0}/{1} failed: {2}", new[] { "3", "5", "timeout" });
Tick("含 {N} placeholder 的訊息已格式化寫入");

// ─── 3. 物件序列化（JSON）─────────────────────────────────────────────
Header("3. 物件序列化（JSON）");
var quote = new { Symbol = "BTC/USDT", Price = 67890.12m, Volume = 1234, Best5 = new[] { 67889m, 67888m, 67887m } };
LOG.Info_Log(quote);
LOG.Info_Log("含訊息的物件 log", quote);
Tick("匿名物件已序列化為 JSON");

// ─── 4. 異常記錄 ─────────────────────────────────────────────────────────
Header("4. 異常記錄");

// (a) 一般異常
try { throw new InvalidOperationException("scenario 4a 一般異常 demo"); }
catch (Exception ex) { LOG.Error_Log(ex); }

// (b) 自訂異常
try { throw new ErrorMessageException("scenario 4b 自訂異常 demo"); }
catch (ErrorMessageException ex) { LOG.Warn_Log(ex); }

// (c) 巢狀異常
try
{
    try { throw new InvalidOperationException("最內層"); }
    catch (Exception inner) { throw new ErrorMessageException("中層包裝", inner); }
}
catch (ErrorMessageException ex)
{
    LOG.Error_Log("scenario 4c 巢狀異常 demo", ex);
}

// (d) ArgumentNullException + Fatal
try { throw new ArgumentNullException("priceFeed", "報價串流為 null"); }
catch (ArgumentNullException ex) { LOG.Fatal_Log(ex); }

Tick("4 種異常情境已記錄為 JSON 結構");

// ─── 5. CustomName 寫入（HFT 預設用法）────────────────────────────────
Header("5. CustomName（每商品獨立檔案）");
LOG.CustomName_Log("BTC", "scenario 5: BTC tick 67890.12");
LOG.CustomName_Log("ETH", "scenario 5: ETH tick  3567.45");
LOG.CustomName_Log("SOL", "scenario 5: SOL tick   234.78");
LOG.CustomName_Log("API", "scenario 5: 外部呼叫 GET /price");
Tick("4 個 CustomName log 已寫入各自檔案");

// ─── 6. HFT 多執行緒 × 多商品壓測 ─────────────────────────────────────
Header("6. HFT 多執行緒 × 多商品壓測");
const int productCount = 100;
const int threadCount = 8;
const int logsPerThread = 2000;
var products = Enumerable.Range(0, productCount).Select(i => $"PROD{i:D3}").ToArray();
var totalLogs = 0L;

Console.WriteLine($"  配置: {threadCount} threads × {logsPerThread} logs each, {productCount} 商品輪寫");
Console.WriteLine($"  目標總筆數: {threadCount * logsPerThread:N0}");
Console.WriteLine($"  MaxOpenFileStreams = 50 → 預期會看到 LRU 關閉冷檔");

var sw = Stopwatch.StartNew();
var threads = new List<Thread>();
for (int t = 0; t < threadCount; t++)
{
    var threadIdx = t;
    var th = new Thread(() =>
    {
        var rng = new Random(threadIdx * 1000 + 7);
        for (int i = 0; i < logsPerThread; i++)
        {
            var p = products[rng.Next(products.Length)];
            LOG.CustomName_Log(p, $"t{threadIdx} i{i} bid:{rng.NextDouble() * 100:F4} ask:{rng.NextDouble() * 100:F4}");
            Interlocked.Increment(ref totalLogs);
        }
    })
    { IsBackground = true, Name = $"hft-{t}" };
    threads.Add(th);
    th.Start();
}
foreach (var th in threads) th.Join();
sw.Stop();

var rate = totalLogs * 1000.0 / Math.Max(1, sw.ElapsedMilliseconds);
Console.WriteLine($"  實際寫入: {totalLogs:N0} 筆");
Console.WriteLine($"  耗時:     {sw.ElapsedMilliseconds:N0} ms");
Console.WriteLine($"  吞吐:     {rate:N0} logs/sec（呼叫端入隊速率）");
Console.WriteLine($"  drop 計數: {droppedCount:N0}（queue 飽和時 drop oldest）");

// ─── 7. queue 飽和 drop oldest 壓測 ─────────────────────────────────────
Header("7. queue 飽和 drop oldest 壓測");
var dropBefore = droppedCount;
Console.WriteLine($"  queue 容量 5000；連續 fire 50000 筆，不等 dispatcher");

sw.Restart();
for (int i = 0; i < 50_000; i++)
    LOG.Info_Log($"burst {i}");
sw.Stop();
var enqueueRate = 50_000 * 1000.0 / Math.Max(1, sw.ElapsedMilliseconds);
Console.WriteLine($"  純 enqueue 50K 筆耗時: {sw.ElapsedMilliseconds} ms（{enqueueRate:N0} logs/sec 入隊）");
Thread.Sleep(800);  // 等 dispatcher 抽完
var dropAfter = droppedCount;
Console.WriteLine($"  本批 drop 數:        {dropAfter - dropBefore:N0}");
Console.WriteLine($"  累計 drop 數:        {dropAfter:N0}");
Console.WriteLine("  ✓ 預期：超出 queue 部分被 drop oldest，呼叫端不會 block");

// ─── 8. immediateFlush 路徑 ──────────────────────────────────────────────
Header("8. immediateFlush 路徑");
LOG.Error_Log("scenario 8: Error 自動 immediate flush（>= Error 級別）");
LOG.Info_Log("scenario 8: 手動 immediate flush", new[] { "manual" }, true, true);
LOG.CustomName_Log("CRITICAL", "scenario 8: CustomName immediate", new[] { "true" }, true, true);
Tick("3 筆 immediateFlush log 已立即寫入並 flush 到磁碟");

// ─── 9. Sync 模式 sanity check ──────────────────────────────────────────
Header("9. Sync 模式 sanity check");
Console.WriteLine("  LogConfiguration 不可重入（v3.0 維持 v2.x 行為），無法在執行中切到 sync 模式");
Console.WriteLine("  Sync 路徑由 xUnit 測試 BasicLoggingTests 涵蓋（dotnet test OzaLog.Tests）");

// ─── 等 dispatcher 處理完 ────────────────────────────────────────────────
Header("等待 dispatcher flush 最後 batch");
Thread.Sleep(2000);

// ─── 終端報告 ────────────────────────────────────────────────────────────
Header("最終統計");
var logsRoot = Path.Combine(AppContext.BaseDirectory, LOG.GetCurrentOptions().LogPath);
if (Directory.Exists(logsRoot))
{
    var dateDirs = Directory.GetDirectories(logsRoot);
    var allFiles = dateDirs.SelectMany(d => Directory.GetFiles(d, "*.txt", SearchOption.AllDirectories)).ToArray();
    long totalSize = 0;
    foreach (var f in allFiles)
    {
        try { totalSize += new FileInfo(f).Length; } catch { }
    }

    Console.WriteLine($"  log 根目錄:      {logsRoot}");
    Console.WriteLine($"  日期子目錄數:    {dateDirs.Length}");
    Console.WriteLine($"  生成檔案總數:    {allFiles.Length}");
    Console.WriteLine($"  總大小:          {totalSize / 1024.0:N2} KB ({totalSize / 1024.0 / 1024.0:N2} MB)");
    Console.WriteLine($"  drop 累計:       {droppedCount:N0}");
    Console.WriteLine();

    // 列出最大檔（檢查是否有觸發 part 分割）
    var topFiles = allFiles
        .Select(f => new { Path = f, Size = new FileInfo(f).Length })
        .OrderByDescending(x => x.Size)
        .Take(5)
        .ToArray();
    if (topFiles.Length > 0)
    {
        Console.WriteLine("  Top 5 大檔：");
        foreach (var f in topFiles)
            Console.WriteLine($"    {f.Size,12:N0} bytes  {Path.GetRelativePath(logsRoot, f.Path)}");
    }

    // 抽樣顯示前幾筆（驗證格式）
    var sampleFile = allFiles.FirstOrDefault();
    if (sampleFile != null)
    {
        Console.WriteLine();
        Console.WriteLine($"  抽樣檔案 ({Path.GetRelativePath(logsRoot, sampleFile)})：");
        try
        {
            using var fs = new FileStream(sampleFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            for (int i = 0; i < 3; i++)
            {
                var line = sr.ReadLine();
                if (line == null) break;
                Console.WriteLine($"    {line}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    (無法讀取：{ex.Message})");
        }
    }
}
else
{
    Console.WriteLine("  ⚠ logs 目錄未生成！");
}

Console.WriteLine();
Console.WriteLine("按 Enter 結束（讓 dispatcher 處理完所有未寫入 log）...");
Console.ReadLine();

// ═══════════════════════════════════════════════════════════════════════════

[MethodImpl(MethodImplOptions.NoInlining)]
static void Header(string title)
{
    Console.WriteLine();
    var bar = new string('-', Math.Max(40, title.Length + 6));
    Console.WriteLine(bar);
    Console.WriteLine($"  {title}");
    Console.WriteLine(bar);
}

[MethodImpl(MethodImplOptions.NoInlining)]
static void Tick(string note) => Console.WriteLine($"  v {note}");
