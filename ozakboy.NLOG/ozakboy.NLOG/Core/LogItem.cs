using System;
using System.Collections.Generic;
using System.Text;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌項目類別，用於在不同組件間傳遞日誌資訊
    /// </summary>
    internal class LogItem
    {
        /// <summary>
        /// 日誌級別
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 日誌名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 日誌訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日誌參數
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// 是否需要立即寫入
        /// </summary>
        public bool RequireImmediateFlush { get; set; }
    }
}
