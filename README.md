# Ozakboy.NLOG

[![nuget](https://img.shields.io/badge/nuget-ozakboy.NLOG-blue)](https://www.nuget.org/packages/Ozakboy.NLOG/) 
[![github](https://img.shields.io/badge/github-ozakboy.NLOG-blue)](https://github.com/ozakboy/ozakboy.NLOG/)

簡單易用的日誌記錄工具，支援文字檔案日誌記錄，無需資料庫配置。適合快速整合到專案中使用。

## 支援框架

- .NET Framework 4.6.2
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET Standard 2.0/2.1

## 特點

- 📝 自動建立日誌檔案和目錄結構
- 🔄 智能檔案管理，自動分割大型日誌檔案
- ⏰ 彈性的日誌保留期限設定
- 🎯 多層級日誌支援（Trace、Debug、Info、Warn、Error、Fatal）
- 💡 支援自定義日誌類型
- 🔒 執行緒安全設計
- ✨ 內建 JSON 序列化支援
- 📊 完整的例外資訊記錄
- 🛡️ 防止格式化字串錯誤
- 🔍 詳細的執行緒資訊追蹤

## 安裝

透過 NuGet Package Manager 安裝：

```bash
Install-Package Ozakboy.NLOG
```

或透過 .NET CLI：

```bash
dotnet add package Ozakboy.NLOG
```

## 詳細功能說明

### 1. 基本日誌記錄

支援六種標準日誌層級：

```csharp
using ozakboy.NLOG;

// 追蹤日誌 - 用於詳細追蹤程式執行流程
LOG.Trace_Log("API 請求開始處理");

// 調試日誌 - 用於開發階段的調試資訊
LOG.Debug_Log("變數值: " + value);

// 訊息日誌 - 用於記錄一般操作資訊
LOG.Info_Log("使用者登入成功");

// 警告日誌 - 用於記錄潛在問題
LOG.Warn_Log("API 響應時間超過預期");

// 錯誤日誌 - 用於記錄錯誤但不影響系統運行
LOG.Error_Log("資料驗證失敗");

// 致命日誌 - 用於記錄嚴重錯誤
LOG.Fatal_Log("資料庫連線中斷");
```

### 2. 物件序列化記錄

支援自動序列化物件為 JSON 格式：

```csharp
// 記錄自定義類別
var user = new User { Id = 1, Name = "Test User" };
LOG.Info_Log(user);

// 記錄異常物件
try {
    // 程式碼
}
catch (Exception ex) {
    LOG.Error_Log(ex);  // 自動序列化異常資訊
}
```

### 3. 格式化字串支援

```csharp
// 基本格式化
LOG.Info_Log("使用者 {0} 執行了 {1} 操作", true, new string[] { "admin", "delete" });

// JSON 字串自動處理
var jsonString = "{\"key\": \"value\"}";
LOG.Info_Log(jsonString);  // 自動處理 JSON 字串中的大括號
```

### 4. 自定義日誌類型

支援自定義日誌類型，方便分類管理：

```csharp
// 記錄 API 相關日誌
LOG.CostomName_Log("API", "外部 API 呼叫開始");

// 記錄資料庫操作
LOG.CostomName_Log("Database", "執行資料庫備份");

// 記錄安全相關資訊
LOG.CostomName_Log("Security", "偵測到異常登入嘗試");

// 自定義類型也支援物件序列化
var apiResponse = new ApiResponse { Status = 200 };
LOG.CostomName_Log("API", apiResponse);
```

### 5. 進階配置選項

```csharp
// 設定日誌保留天數（使用負數）
LOG.SetLogKeepDay(-7);  // 保留最近 7 天的日誌

// 設定單個日誌檔案大小上限
LOG.SetBigFilesByte(100 * 1024 * 1024);  // 設定為 100MB

// 控制是否寫入檔案
LOG.Info_Log("僅控制台輸出", false);  // 第二個參數設為 false 則只輸出到控制台
```

### 6. 日誌檔案管理

#### 檔案位置
- 根目錄：`[應用程式根目錄]\logs\LogFiles\`
- 檔案命名：`yyyyMMdd_[LogType]_Log.txt`
- 分割檔案：`yyyyMMdd_[LogType]_part[n]_Log.txt`

#### 檔案格式
每行日誌包含以下資訊：
```
HH:mm:ss[ThreadId] 日誌內容
```

### 7. 異常處理功能

完整的異常資訊記錄：

```csharp
try {
    // 程式碼
}
catch (Exception ex) {
    // 記錄完整異常資訊，包括：
    // - 異常類型
    // - 異常訊息
    // - 堆疊追蹤
    // - 內部異常
    // - 自定義資料
    LOG.Error_Log(ex);
}
```

## 效能考量

- 使用 lock 機制確保執行緒安全
- 智能檔案分割避免單個檔案過大
- 自動清理過期日誌節省磁碟空間
- 優化的字串處理和格式化邏輯

## 最佳實踐

1. 適當設定日誌保留時間，避免佔用過多磁碟空間
2. 根據專案需求合理使用不同日誌層級
3. 善用自定義日誌類型進行日誌分類
4. 使用物件序列化功能記錄結構化資料
5. 在關鍵操作點加入適當的日誌記錄

## 疑難排解

常見問題：

1. 檔案存取權限問題
   - 確保應用程式對日誌目錄具有寫入權限
   
2. 日誌檔案鎖定
   - 使用 `WriteTxt` 參數控制檔案寫入
   
3. 格式化字串問題
   - 使用陣列傳遞格式化參數
   - 注意 JSON 字串中的大括號處理

## 授權條款

MIT License

## 貢獻指南

歡迎透過以下方式貢獻：
 
1. 提交 Issue 回報問題
2. 提交 Pull Request 改進程式碼
3. 完善文件說明
4. 分享使用經驗

## 支援與聯繫

- GitHub Issues: [提交問題](https://github.com/ozakboy/ozakboy.NLOG/issues)
- Pull Requests: [提交改進](https://github.com/ozakboy/ozakboy.NLOG/pulls)