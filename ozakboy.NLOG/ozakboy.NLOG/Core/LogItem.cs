using System;
using System.Collections.Generic;
using System.Text;

namespace ozakboy.NLOG.Core
{

    /// <summary>
    /// 日誌項目類別 - 用於在不同組件間傳遞日誌資訊
    /// Log Item Class - Used for transferring log information between different components
    /// </summary>
    internal class LogItem
    {
        /// <summary>
        /// 日誌級別 - 定義日誌的重要程度
        /// Log Level - Defines the severity of the log entry
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 日誌名稱 - 用於識別日誌來源或類型
        /// Log Name - Used to identify the source or type of log
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 日誌訊息 - 記錄的實際內容
        /// Log Message - The actual content of the log entry
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日誌參數 - 用於格式化日誌訊息的參數陣列
        /// Log Parameters - Array of parameters used for formatting log messages
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// 是否需要立即寫入 - 控制日誌的即時性
        /// Immediate Flush Required - Controls whether the log needs immediate writing
        /// </summary>
        public bool RequireImmediateFlush { get; set; }
    }
}
