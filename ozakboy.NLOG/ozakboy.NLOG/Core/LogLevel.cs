using System;
using System.Collections.Generic;
using System.Text;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌級別
    /// Log Levels
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 追蹤記錄檔 - 用於追蹤程式執行的詳細流程
        /// Trace Log - Used for tracking detailed program execution flow
        /// </summary>
        Trace = 0,

        /// <summary>
        /// 除錯記錄檔 - 用於開發階段的除錯訊息
        /// Debug Log - Used for debugging messages during development
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 資訊記錄檔 - 記錄一般性的系統運作資訊
        /// Info Log - Records general system operational information
        /// </summary>
        Info = 2,

        /// <summary>
        /// 警告記錄檔 - 記錄可能影響系統運作但不致嚴重的問題
        /// Warning Log - Records potential issues that might affect system operation but are not severe
        /// </summary>
        Warn = 3,

        /// <summary>
        /// 錯誤記錄檔 - 記錄系統運作中的錯誤狀況
        /// Error Log - Records error conditions in system operation
        /// </summary>
        Error = 4,

        /// <summary>
        /// 致命錯誤記錄檔 - 記錄導致系統無法運作的嚴重錯誤
        /// Fatal Log - Records severe errors that cause system failure
        /// </summary>
        Fatal = 5,

        /// <summary>
        /// 自定義記錄檔 - 用於特定需求的客製化日誌類型
        /// Custom Log - Customized log type for specific requirements
        /// </summary>
        CostomName = 99
    }
}
