---
title: 版本歷史
description: OzaLog 所有重要變更紀錄。
---

# 版本更新記錄

本檔案記錄 **OzaLog**（前身為 **Ozakboy.NLOG**）套件的所有重要變更。
版本號遵循 [語意化版本（SemVer）](https://semver.org/lang/zh-TW/)。

---

## [3.0.1] - 2026-05-09

> 元資料與 repo 改善版本。**函式庫程式碼無變更** —— OzaLog 組件與 v3.0.0 二進位等同 (Deterministic build)。

### 功能優化
- **NuGet 套件元資料翻新**: `Description` 更精煉 (突出 `LOG.Info_Log("...")` API + HFT pipeline + 零依賴 + 加密貨幣報價串流場景)、更新 `PackageTags` (新增 `ozalog`、`hft`、`high-performance`、`zero-dependency`;移除誤導的 `nlog` tag)、調整 `Title`。
- `PackageReleaseNotes` 改用完整 GitHub URL (NuGet 不解析相對路徑)。

### 技術改進
- **新建專案介紹網站**: Nuxt 4 + @nuxt/content + Tailwind CSS,部署至 GitHub Pages → <https://ozakboy.github.io/OzaLog/>
- **Repo 文件結構重整**: 所有對外文件搬到 `docs/{en,zh-TW}/` 雙語樹 (`changelog.md`、`migration.md`,並含 5 個子頁模板 `getting-started.md` / `configuration.md` / `api.md` / `async-pipeline.md` / `benchmarks.md`)。
- GitHub Actions 在 push 至 main 時自動部署網站。
- 贊助頁新增 USDT (BEP20) 錢包 + Binance Pay QR。
- `uplog` 發佈流程擴充: 現在會自動建立 GitHub Release 並推送到 NuGet.org。

### 備註
- v2.x 升級指南見 [升級指南](./migration.md)。

---

## [3.0.0] - 2026-05-09

### 破壞性變更
- **套件改名**：`Ozakboy.NLOG` → `OzaLog`。NuGet 上原套件標 deprecated 並指向此處。升級指南請見[升級指南](./migration.md)。
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
