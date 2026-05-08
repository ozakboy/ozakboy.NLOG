# 版本更新記錄 / Changelog

本檔案記錄 **Ozakboy.NLOG** 套件的所有重要變更。
版本號遵循 [語意化版本（SemVer）](https://semver.org/lang/zh-TW/)。

This file tracks all notable changes to the **Ozakboy.NLOG** package.
Version numbers follow [Semantic Versioning](https://semver.org/).

---

## [2.1.0] - 2024

### 新增功能 / Added
- 新增 .NET 8.0 支援 / Added support for .NET 8.0
- 新增異步日誌寫入機制（含可配置的批次處理） / Introduced async logging with configurable batch processing
- 新增不同日誌級別的可自訂目錄結構 / Added customizable directory structure for different log levels
- 新增自訂日誌類型支援（`CustomName_Log`） / Added support for custom log types
- 新增主控台輸出開關 / Added console output support

### 功能優化 / Improved
- 強化檔案管理，加入自動分割大檔機制 / Enhanced file management with automatic log rotation
- 強化異常處理與序列化 / Enhanced exception handling and serialization
- 強化配置系統，提供更多選項與便捷方法 / Improved configuration system with more options
- 改善跨作業系統的檔案路徑處理 / Better handling of file paths across operating systems

### 技術改進 / Technical
- 改善線程安全與整體效能 / Improved thread safety and performance
- 智慧型檔案大小管理 / Implemented intelligent file size management

---

> 早期版本（< 2.1.0）的歷史記錄未完整保留。詳細變更可參考 git 歷史與 NuGet 套件頁面。
> History prior to 2.1.0 is not fully tracked here. See git history and NuGet for details.
