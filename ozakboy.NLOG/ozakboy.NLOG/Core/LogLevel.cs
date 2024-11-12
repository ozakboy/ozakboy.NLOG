using System;
using System.Collections.Generic;
using System.Text;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌級別
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 追蹤記錄檔
        /// </summary>
        Trace = 0,
        /// <summary>
        /// 測試記錄檔
        /// </summary>
        Debug = 1,
        /// <summary>
        /// 訊息記錄檔
        /// </summary>
        Info = 2,
        /// <summary>
        /// 警告記錄檔
        /// </summary>
        Warn = 3,
        /// <summary>
        /// 錯誤記錄檔
        /// </summary>
        Error = 4,
        /// <summary>
        /// 致命記錄檔
        /// </summary>
        Fatal = 5,
        /// <summary>
        /// 自定義名稱Log記錄檔
        /// </summary>
        CostomName = 99
    }
}
