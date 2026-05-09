---
title: 升級指南
description: 從 Ozakboy.NLOG v2.x 升級至 OzaLog v3.0。
---

# 升級指南 — `Ozakboy.NLOG` v2.x → `OzaLog` v3.0

> **為何改名？** `Ozakboy.NLOG` 與知名套件 [`NLog`](https://www.nuget.org/packages/NLog) 完全無關,但名稱相近常造成使用者混淆。v3.0 改名為 **`OzaLog`** 一次解決,並標記 v3.0 架構大改寫的明確分界。

> **`Ozakboy.NLOG` v2.1.0 已棄用**,不再更新。所有後續開發都在 `OzaLog`。

---

## 1. 更新 NuGet PackageReference

在您的 `.csproj`:

```diff
- <PackageReference Include="Ozakboy.NLOG" Version="2.1.0" />
+ <PackageReference Include="OzaLog" Version="3.0.0" />
```

或透過 CLI:

```bash
dotnet remove package Ozakboy.NLOG
dotnet add package OzaLog
```

## 2. 更新 `using` 陳述式

在程式碼中全域取代:

```diff
- using ozakboy.NLOG;
+ using OzaLog;
```

```diff
- using ozakboy.NLOG.Core;
+ using OzaLog.Core;
```

> 提示:單次專案範圍的「尋找與取代」即可處理。上面兩個 pattern 涵蓋所有引用。

## 3. 公開 API 完全沒變

您**不需要**修改任何 `LOG.*_Log(...)` 呼叫。所有公開方法簽章都保留:

```csharp
LOG.Trace_Log("...");
LOG.Debug_Log("...");
LOG.Info_Log("...");
LOG.Warn_Log("...");
LOG.Error_Log("...");
LOG.Fatal_Log("...");
LOG.CustomName_Log("BTC", "...");
LOG.Configure(o => { ... });
```

## 4. 破壞性變更(務必詳讀)

### 4.1 移除部分 TargetFrameworks

OzaLog v3.0 支援:
- ✅ `netstandard2.0` / `netstandard2.1`
- ✅ `net8.0` / `net9.0` / `net10.0`

OzaLog v3.0 **移除**:
- ❌ `.NET Framework 4.6.2` — 若需 .NET Framework 請繼續使用 Ozakboy.NLOG v2.1.0
- ❌ `net6.0`(微軟已於 2024-11 停止支援)
- ❌ `net7.0`(微軟已於 2024-05 停止支援)

若仍在 .NET 6/7,請先升級至 net8 LTS。

### 4.2 `LogLevel.CostomName` → `LogLevel.CustomName`(拼字修正)

Enum 值名稱原本是拼錯的,v3.0 修正:

```diff
- LogLevel.CostomName
+ LogLevel.CustomName
```

數值(`99`)未變,因此 wire-format / 序列化表示仍維持相容。

> 99% 使用者不會直接接觸這個 enum(公開 API 是 `LOG.CustomName_Log(...)`,本就拼對)。若您有直接引用 enum 值,請更新。

### 4.3 新增 `LogOptions` 屬性(增量,非破壞)

不需要的話可以忽略:

```csharp
LOG.Configure(o =>
{
    // ... 既有選項 ...

    // v3.0 新增
    o.EnableGlobalExceptionCapture = false;   // 預設 false;啟用 AppDomain 全域意外處理
    o.MaxOpenFileStreams = 100;               // LRU 上限(預設 100)
    o.DiskFlushIntervalMs = 100;              // 落盤間隔(預設 100ms)
    o.OnDropped = () => { /* counter */ };    // 背壓 callback
});
```

### 4.4 內部架構大改寫(對您透明)

內部已重寫為 HFT 級吞吐:
- `LogItem` 改為 `readonly struct`(零 GC 壓力)
- 持久化 `FileStream` 池含 LRU 淘汰
- 時間戳每 1 ms 快取一次(避免 `DateTime.Now` syscall)
- 隊列滿時改 drop-oldest 背壓(舊版:降級為同步寫入)

若您先前依賴「隊列滿 → 呼叫端在同步寫上阻塞」這個實作細節,請注意新版改為「隊列滿 → 丟棄最舊 log 繼續執行」。需追蹤丟棄數量請使用 `OnDropped` callback。

### 4.5 `Console.WriteLine` 雙重格式化 bug 修正

v2.x 中,若啟用 console 輸出且格式化後訊息恰好含有額外的 `{N}` placeholder,`LOG.Info_Log("含 {0} 的訊息", args)` 可能丟出 `FormatException`。v3.0 已修正。

## 5. 檔案 / 資料夾改名(資訊用)

若您 clone 了原始 repo:

| v2.x | v3.0 |
|------|------|
| `ozakboy.NLOG/ozakboy.NLOG/ozakboy.NLOG.sln` | `OzaLog/OzaLog.sln` |
| `ozakboy.NLOG/ozakboy.NLOG/`(solution 資料夾) | `OzaLog/` |
| `ozakboy.NLOG/ozakboy.NLOG/ozakboy.NLOG/`(專案資料夾) | `OzaLog/OzaLog/` |
| `ozakboy.NLOG.csproj` | `OzaLog.csproj` |
| `NLOG.cs`(檔名) | `LOG.cs` |

## 6. 暫時無法升級怎麼辦

`Ozakboy.NLOG` v2.1.0 仍會留在 NuGet 上(會標 deprecated 並指向 OzaLog),既有專案可繼續安裝執行。**v2.x 不會再有任何更新——包含 security patch**。若您需要修正,唯一路徑是 OzaLog v3.0+。

git tag `v2-frozen` 標記 repo 中 v2.x 的最終狀態。

## 7. 需要協助?

開 issue: https://github.com/ozakboy/OzaLog/issues
