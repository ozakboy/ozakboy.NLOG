# Ozakboy.NLOG

[![nuget](https://img.shields.io/badge/nuget-ozakboy.NLOG-blue)](https://www.nuget.org/packages/Ozakboy.NLOG/) 
[![github](https://img.shields.io/badge/github-ozakboy.NLOG-blue)](https://github.com/ozakboy/ozakboy.NLOG/)

[English](README.md) | [繁體中文](README_zh-TW.md) 

輕量級且高效能的日誌記錄工具，提供異步寫入、智能檔案管理和豐富的配置選項。專為 .NET 應用程式設計的本地日誌解決方案。

## 支援框架

- .NET Framework 4.6.2
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET Standard 2.0/2.1

## 主要特點

### 核心功能
- 📝 自動建立日誌檔案和目錄結構
- 🔄 支援異步日誌寫入，提升應用程式效能
- ⚡ 智能批次處理和隊列管理
- 🔍 詳細的異常資訊記錄和序列化
- 📊 多層級日誌支援
- 🛡️ 執行緒安全設計

### 進階特性
- ⚙️ 靈活的配置系統
- 📂 自定義日誌目錄結構
- 🔄 自動檔案分割和管理
- ⏰ 可配置的日誌保留期限
- 💾 智能檔案大小管理
- 🎯 支援自定義日誌類型
- 🖥️ 可選的控制台輸出

## 安裝

透過 NuGet Package Manager：
```bash
Install-Package Ozakboy.NLOG
```

或使用 .NET CLI：
```bash
dotnet add package Ozakboy.NLOG
```

## 快速入門

### 基本配置
```csharp
LOG.Configure(options => {
    options.KeepDays = -7;                    // 保留最近 7 天的日誌
    options.SetFileSizeInMB(50);              // 設定單個檔案大小上限為 50MB
    options.EnableAsyncLogging = true;         // 啟用異步寫入
    options.EnableConsoleOutput = true;        // 啟用控制台輸出
    
    // 配置異步選項
    options.ConfigureAsync(async => {
        async.MaxBatchSize = 100;              // 每批次最多處理 100 條日誌
        async.MaxQueueSize = 10000;            // 隊列最大容量
        async.FlushIntervalMs = 1000;          // 每秒寫入一次
    });
});
```

### 基本用法

```csharp
// 記錄不同級別的日誌
LOG.Trace_Log("詳細追蹤資訊");
LOG.Debug_Log("除錯資訊");
LOG.Info_Log("一般資訊");
LOG.Warn_Log("警告訊息");
LOG.Error_Log("錯誤資訊");
LOG.Fatal_Log("致命錯誤");

// 記錄帶有參數的日誌
LOG.Info_Log("使用者 {0} 執行了 {1} 操作", new string[] { "admin", "login" });

// 記錄物件
var data = new { Id = 1, Name = "Test" };
LOG.Info_Log("數據記錄", data);

// 記錄異常
try {
    // 程式碼
} catch (Exception ex) {
    LOG.Error_Log(ex);
}

// 自定義日誌類型
LOG.CustomName_Log("API", "外部服務呼叫");
```

## 日誌檔案管理

### 預設目錄結構
```
應用程式根目錄/
└── logs/                          # 預設根目錄（可通過 LogPath 修改）
    └── yyyyMMdd/                  # 日期目錄
        └── LogFiles/              # 預設日誌檔案目錄（可通過 TypeDirectories.DirectoryPath 修改）
            └── [LogType]_Log.txt  # 日誌檔案
```

### 自定義目錄結構
可以通過配置為不同級別的日誌指定獨立的目錄：

```csharp
LOG.Configure(options => {
    // 修改根目錄
    options.LogPath = "CustomLogs";  // 預設是 "logs"
    
    // 為不同級別的日誌配置獨立目錄
    options.TypeDirectories.DirectoryPath = "AllLogs";     // 預設目錄，如未指定特定級別則使用此目錄
    options.TypeDirectories.ErrorPath = "ErrorLogs";       // 錯誤日誌專用目錄
    options.TypeDirectories.InfoPath = "InfoLogs";         // 信息日誌專用目錄
    options.TypeDirectories.WarnPath = "WarningLogs";      // 警告日誌專用目錄
    options.TypeDirectories.DebugPath = "DebugLogs";       // 調試日誌專用目錄
    options.TypeDirectories.TracePath = "TraceLogs";       // 追蹤日誌專用目錄
    options.TypeDirectories.FatalPath = "FatalLogs";       // 致命錯誤日誌專用目錄
    options.TypeDirectories.CustomPath = "CustomLogs";     // 自定義類型日誌專用目錄
});
```

配置後的目錄結構示例：
```
應用程式根目錄/
└── CustomLogs/                    # 自定義根目錄
    └── yyyyMMdd/                  # 日期目錄
        ├── ErrorLogs/             # 錯誤日誌目錄
        │   └── Error_Log.txt
        ├── InfoLogs/              # 信息日誌目錄
        │   └── Info_Log.txt
        ├── WarningLogs/           # 警告日誌目錄
        │   └── Warn_Log.txt
        └── AllLogs/               # 預設目錄（未特別指定的日誌類型）
            └── [LogType]_Log.txt
```

### 檔案命名規則
- 基本格式：`[LogType]_Log.txt`
- 分割檔案：`[LogType]_part[N]_Log.txt`
- 自定義日誌：`[CustomName]_Log.txt`

### 檔案大小管理
```csharp
LOG.Configure(options => {
    // 設定單個檔案大小上限（以 MB 為單位）
    options.SetFileSizeInMB(50);  // 檔案達到 50MB 時自動分割
});
```

當檔案超過設定的大小限制時，會自動建立新的分割檔案：
- 第一個分割檔案：`[LogType]_part1_Log.txt`
- 第二個分割檔案：`[LogType]_part2_Log.txt`
- 以此類推...

### 範例使用場景

1. 所有日誌統一管理：
```csharp
LOG.Configure(options => {
    options.LogPath = "logs";
    options.TypeDirectories.DirectoryPath = "LogFiles";
});
```

2. 錯誤日誌獨立存放：
```csharp
LOG.Configure(options => {
    options.LogPath = "logs";
    options.TypeDirectories.ErrorPath = "CriticalErrors";
    options.TypeDirectories.FatalPath = "CriticalErrors";
});
```

3. 完全分離的日誌系統：
```csharp
LOG.Configure(options => {
    options.LogPath = "SystemLogs";
    options.TypeDirectories.ErrorPath = "Errors";
    options.TypeDirectories.InfoPath = "Information";
    options.TypeDirectories.WarnPath = "Warnings";
    options.TypeDirectories.DebugPath = "Debugging";
    options.TypeDirectories.TracePath = "Traces";
    options.TypeDirectories.FatalPath = "Critical";
    options.TypeDirectories.CustomPath = "Custom";
});
```

### 自動清理機制
```csharp
// 設定日誌保留天數
LOG.Configure(options => {
    options.KeepDays = -30; // 保留最近 30 天的日誌
});
```

## 異常處理功能

### 詳細的異常記錄
```csharp
try {
    // 您的程式碼
} catch (Exception ex) {
    // 記錄完整的異常資訊，包括：
    // - 異常類型和訊息
    // - 堆疊追蹤
    // - 內部異常
    // - 額外屬性
    LOG.Error_Log(ex);
}
```

### 自定義異常資訊
```csharp
try {
    // 您的程式碼
} catch (Exception ex) {
    // 添加自定義訊息
    LOG.Error_Log("資料處理失敗", ex);
    
    // 同時記錄相關資料
    var contextData = new { UserId = "123", Operation = "DataProcess" };
    LOG.Error_Log("操作上下文", contextData);
}
```

### 異常序列化
```csharp
try {
    // 您的程式碼
} catch (Exception ex) {
    // 異常會被自動序列化為結構化的 JSON 格式
    LOG.Error_Log(ex);
    
    // 或者與其他資訊一起序列化
    var errorContext = new {
        Exception = ex,
        TimeStamp = DateTime.Now,
        Environment = "Production"
    };
    LOG.Error_Log(errorContext);
}
```

## 即時寫入模式

### 同步即時寫入
```csharp
// 使用 immediateFlush 參數強制即時寫入
LOG.Error_Log("重要錯誤", new string[] { "error_details" }, true, true);

// 用於自定義日誌
LOG.CustomName_Log("Critical", "系統異常", new string[] { "error_code" }, true, true);
```

### 異步即時寫入配置
```csharp
LOG.Configure(options => {
    options.EnableAsyncLogging = true;
    options.ConfigureAsync(async => {
        async.FlushIntervalMs = 100;     // 縮短寫入間隔
        async.MaxBatchSize = 1;          // 設定最小批次大小
        async.MaxQueueSize = 1000;       // 設定適當的隊列大小
    });
});

// Error 和 Fatal 級別的日誌會自動觸發即時寫入
LOG.Error_Log("嚴重錯誤");
LOG.Fatal_Log("系統崩潰");
```

### 條件式即時寫入
```csharp
// 根據條件決定是否即時寫入
void LogMessage(string message, bool isCritical) {
    if (isCritical) {
        LOG.Error_Log(message, new string[] { }, true, true);  // 即時寫入
    } else {
        LOG.Info_Log(message);  // 一般寫入
    }
}
```

## 效能優化

- 異步寫入避免 I/O 阻塞
- 智能批次處理減少磁碟操作
- 優化的序列化機制
- 執行緒安全的隊列管理
- 自動檔案管理避免過大檔案

## 最佳實踐

1. 根據應用程式需求選擇同步或異步模式
2. 適當配置批次大小和寫入間隔
3. 根據日誌量調整檔案大小限制
4. 設定合理的日誌保留期限
5. 利用自定義類型分類管理日誌
6. 在關鍵節點記錄必要的異常資訊

## 疑難排解

常見問題處理：

1. 檔案存取權限問題
   - 確保應用程式具有寫入權限
   - 檢查資料夾存取權限設定

2. 效能問題
   - 調整異步配置參數
   - 檢查日誌檔案大小設定
   - 優化寫入頻率

3. 檔案管理
   - 定期檢查日誌清理狀況
   - 監控磁碟空間使用

## 授權條款

MIT License

## 支援與回報

- GitHub Issues: [回報問題](https://github.com/ozakboy/ozakboy.NLOG/issues)
- Pull Requests: [貢獻代碼](https://github.com/ozakboy/ozakboy.NLOG/pulls)