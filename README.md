# Ozakboy.NLOG

[![nuget](https://img.shields.io/badge/nuget-ozakboy.NLOG-blue)](https://www.nuget.org/packages/Ozakboy.NLOG/) 
[![github](https://img.shields.io/badge/github-ozakboy.NLOG-blue)](https://github.com/ozakboy/ozakboy.NLOG/)

簡單易用的日誌記錄工具，支援文字檔案日誌記錄，無需資料庫配置。適合快速整合到專案中使用。

## 支援框架

- .NET Core 3.1
- .NET 6.0
- .NET 7.0
- .NET Standard 2.0/2.1

## 特點

- 📝 自動建立日誌檔案
- 🔄 自動分割大型日誌檔案
- ⏰ 支援日誌保留期限設定
- 🎯 支援多種日誌層級
- 💡 支援自定義日誌類型
- 🔒 執行緒安全
- ✨ 支援 JSON 字串記錄

## 安裝

透過 NuGet Package Manager 安裝：

```bash
Install-Package Ozakboy.NLOG
```

或透過 .NET CLI：

```bash
dotnet add package Ozakboy.NLOG
```

## 基本使用

### 1. 記錄不同層級的日誌

```csharp
using ozakboy.NLOG;

// 追蹤日誌
LOG.Trace_Log("這是一條追蹤日誌");

// 調試日誌
LOG.Debug_Log("這是一條調試日誌");

// 訊息日誌
LOG.Info_Log("這是一條訊息日誌");

// 警告日誌
LOG.Warn_Log("這是一條警告日誌");

// 錯誤日誌
LOG.Error_Log("這是一條錯誤日誌");

// 致命錯誤日誌
LOG.Fatal_Log("這是一條致命錯誤日誌");
```

### 2. 記錄例外資訊

```csharp
try
{
    // 您的程式碼
}
catch (Exception ex)
{
    LOG.Error_Log(ex.Message);
    // 或者
    LOG.Warn_Log(ex);
}
```

### 3. 記錄 JSON 資料

```csharp
string jsonData = @"{""name"": ""測試"", ""value"": 123}";
LOG.Info_Log(jsonData);
```

### 4. 自定義日誌類型

```csharp
// 使用自定義日誌類型記錄日誌
LOG.CostomName_Log("API", "API呼叫記錄");
LOG.CostomName_Log("Database", "資料庫操作記錄");
```

### 5. 設定配置

```csharp
// 設定日誌保留天數（負數表示天數）
LOG.SetLogKeepDay(-7);  // 保留7天的日誌

// 設定單個日誌檔案的最大大小（預設50MB）
LOG.SetBigFilesByte(100 * 1024 * 1024);  // 設定為100MB
```

## 日誌檔案說明

- 日誌檔案位置：`[應用程式根目錄]\logs\LogFiles\`
- 日誌檔案命名：`yyyyMMdd_[LogType]_Log.txt`
- 當檔案超過大小限制時，會自動建立新檔案：`yyyyMMdd_[LogType]_part[n]_Log.txt`

### 日誌格式

```
HH:mm:ss[ThreadId] 日誌內容
```

## 進階功能

### 1. 控制日誌是否寫入檔案

```csharp
// 第二個參數設定為 false 時只輸出到控制台，不寫入檔案
LOG.Info_Log("只顯示在控制台的日誌", false);
```

### 2. 帶格式化參數的日誌

```csharp
string[] args = new[] { "參數1", "參數2" };
LOG.Info_Log("格式化訊息: {0}, {1}", true, args);
```

## 效能考量

- 自動清理過期日誌檔案
- 大檔案自動分割
- 執行緒安全實現
- 非同步寫入檔案

## 授權條款

MIT License

## 維護說明

- 長期維護
- 持續更新
- 歡迎提交 Issues 和 Pull Requests

## 貢獻

如果您發現任何問題或有改進建議，歡迎：

1. 提交 Issue
2. 提交 Pull Request
3. 與我們聯繫

## 更新日誌

### v1.x.x (最新版本)
- 增加 JSON 字串支援
- 修復多執行緒並發寫入問題
- 優化日誌檔案管理