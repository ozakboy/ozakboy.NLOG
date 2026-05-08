# 版本更新記錄

[English](CHANGELOG.md) | [繁體中文](CHANGELOG_zh-TW.md)

本檔案記錄 **OzaLog**（前身為 **Ozakboy.NLOG**）套件的所有重要變更。
版本號遵循 [語意化版本（SemVer）](https://semver.org/lang/zh-TW/)。

---

## [3.0.0] - 2026-05-09

### 破壞性變更
- **套件改名**：`Ozakboy.NLOG` → `OzaLog`。NuGet 上原套件標 deprecated 並指向此處。升級指南請見 `MIGRATION.md`。
- **命名空間改名**：`ozakboy.LOG` → `OzaLog`。使用端程式碼的 `using` 須同步更新。
- **移除 TargetFramework**：砍掉 `.NET Framework 4.6.2`、`net6.0`、`net7.0`（皆 EOL）。現支援 `netstandard2.0`、`netstandard2.1`、`net8.0`、`net9.0`、`net10.0`。
- **Enum 拼字修正**：`LogLevel.CostomName` → `LogLevel.CustomName`。公開方法 `LOG.CustomName_Log(...)` 原本就拼對,僅修正底層 enum 值名稱。

### 新增功能
- HFT 級異步管線:`ConcurrentQueue<struct LogItem>` + 持久化 FileStream 池 + 1ms 快取時間戳 + drop-oldest 壓力控制。
- `LogOptions.EnableGlobalExceptionCapture`(預設 `false`)— 可選訂閱 `AppDomain.UnhandledException` 與 `TaskScheduler.UnobservedTaskException`,以 Fatal 級別同步 + 立即 flush 寫入。
- `LogOptions.MaxOpenFileStreams`(預設 `100`)— LRU 上限;超出時關閉最久未寫入的 stream。
- `LogOptions.DiskFlushIntervalMs`(預設 `100`)— 持久化 FileStream 定期 `Flush()` 落盤間隔。
- `LogOptions.OnDropped`(預設 `null`)— 異步隊列在背壓下丟棄最舊項目時觸發的 callback。
- `OzaLog.Tests/` — xUnit 測試專案,涵蓋並發、LRU、換日、backpressure、GlobalExceptionCapture 切換、格式正確性。
- `OzaLog.Benchmarks/` — BenchmarkDotNet 專案,對 ZLogger / ZeroLog / Serilog 做比較。
- `MIGRATION.md` — 自 `Ozakboy.NLOG` v2.x 的升級指南。

### 功能優化
- `net8.0` / `net9.0` / `net10.0` 零 NuGet 依賴(System.Text.Json 已內建於 BCL)。
- 格式化工作移出呼叫執行緒——呼叫端只入隊原始 `(level, name, message, args, ticks, threadId)`,hot path 不做 `string.Format`。
- 持久化 FileStream 消除每批次開關,syscall 成本趨近於零。
- 換日處理移至 dispatcher 內聯(比對快取 ticks 日期與 stream 日期)。
- 過期 log 清理移至背景定時任務(v2.x 在 hot path 上)。

### 問題修正
- `LOG.cs` 中 `Console.WriteLine(formattedMessage, args)` 雙重格式化 bug(若格式化後訊息恰好含 `{0}` 等 token 會丟 `FormatException`)。
- 自動 flush 級別判定 bug:`Error` 與 `Fatal` 現正確觸發立即 flush,不依賴呼叫端 `immediateFlush` 參數。

### 技術改進
- 內部 `LogItem` 改為 `readonly struct`(原為 class)— hot path 零 GC。
- 新增 `Core/TimestampCache.cs` — 背景定時器每 1ms 更新 `volatile long _currentTicks`,呼叫端只讀。
- 新增 `Core/FileStreamPool.cs` — 以 `(level, name)` 為 key 的持久化 FileStream 池,含 LRU 淘汰。
- 新增 `Core/LogRetentionCleaner.cs` — 背景過期 log 清理,脫離 hot path。
- 新增 `Core/GlobalExceptionCapture.cs` — 可選的全域意外攔截。
- 跨 5 個 TargetFrameworks 的建置驗證:0 警告 / 0 錯誤。

---

## [2.1.0] - 2024

### 新增功能
- 新增 .NET 8.0 支援
- 新增異步日誌寫入機制（含可配置的批次處理）
- 新增不同日誌級別的可自訂目錄結構
- 新增自訂日誌類型支援（`CustomName_Log`）
- 新增主控台輸出開關

### 功能優化
- 強化檔案管理，加入自動分割大檔機制
- 強化異常處理與序列化
- 強化配置系統，提供更多選項與便捷方法
- 改善跨作業系統的檔案路徑處理

### 技術改進
- 改善線程安全與整體效能
- 智慧型檔案大小管理

---

> 早期版本（< 2.1.0）的歷史記錄未完整保留。詳細變更可參考 git 歷史與 NuGet 套件頁面。
