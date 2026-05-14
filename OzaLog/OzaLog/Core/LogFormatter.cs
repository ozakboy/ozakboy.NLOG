using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OzaLog.Core
{
    /// <summary>
    /// 日誌格式化處理器 - 將 LogItem 轉為要寫入檔案的單行字串。
    /// v3.0：格式化全部移到 dispatcher 執行緒，呼叫端不再做 string.Format / StringBuilder。
    /// v3.1：時間格式變為使用者可設定的 .NET DateTime 格式;支援 Thread ID/Name 顯示開關。
    /// </summary>
    internal static class LogFormatter
    {
        /// <summary>
        /// 預設時間格式(對應 v3.0 行為,避免變更使用者輸出)
        /// </summary>
        private const string DefaultTimeFormat = "HH:mm:ss.fff";

        /// <summary>
        /// 格式化 LogItem 為單行檔案輸出（不含換行符；換行由 StreamWriter.WriteLine 補）
        /// 格式取決於 <see cref="LogConfiguration.LogOptions.TimeFormat"/>、
        /// <see cref="LogConfiguration.LogOptions.ShowThreadId"/>、
        /// <see cref="LogConfiguration.LogOptions.ShowThreadName"/> 設定
        /// </summary>
        public static string Format(in LogItem item)
        {
            var current = LogConfiguration.Current;
            var dt = new DateTime(item.TimestampTicks, DateTimeKind.Local);

            // 容量估計：時間戳 12-25 + thread 包裝 8-30 + message 概略長度
            var sb = new StringBuilder(128);

            AppendTimestamp(sb, dt, current.TimeFormat);
            AppendThreadSegment(sb, item.ThreadId, item.ThreadName, current.ShowThreadId, current.ShowThreadName);

            var msg = item.Message ?? string.Empty;
            var args = item.Args;
            if (args != null && args.Length > 0)
            {
                try
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, msg, args);
                }
                catch (FormatException)
                {
                    // 訊息含未配對的 {} → 退而求其次直接附加原字串
                    sb.Append(msg);
                }
            }
            else
            {
                sb.Append(msg);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 處理訊息中的特殊字符 - 確保訊息中的 {} 在無格式參數時被跳脫
        /// </summary>
        public static string EscapeMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return message;

            if (message.IndexOf('{') < 0 && message.IndexOf('}') < 0)
                return message;

            // 含 {N} 數字 placeholder 視為已是格式字串，不跳脫
            if (Regex.IsMatch(message, @"\{[0-9]+\}"))
                return message;

            return message.Replace("{", "{{").Replace("}", "}}");
        }

        /// <summary>
        /// 將 DateTime 依使用者設定的格式字串輸出。
        /// 預設 <c>HH:mm:ss.fff</c> 走手寫 fast path(零配置);其他格式走 .NET ToString。
        /// </summary>
        private static void AppendTimestamp(StringBuilder sb, DateTime dt, string timeFormat)
        {
            // Fast path:預設格式手寫,避免 ToString 開銷
            if (string.IsNullOrEmpty(timeFormat) || timeFormat == DefaultTimeFormat)
            {
                AppendTwoDigit(sb, dt.Hour); sb.Append(':');
                AppendTwoDigit(sb, dt.Minute); sb.Append(':');
                AppendTwoDigit(sb, dt.Second); sb.Append('.');
                AppendThreeDigit(sb, dt.Millisecond);
                return;
            }

            try
            {
                sb.Append(dt.ToString(timeFormat, CultureInfo.InvariantCulture));
            }
            catch (FormatException)
            {
                // 使用者給了無效的格式字串 → fallback 預設格式
                AppendTwoDigit(sb, dt.Hour); sb.Append(':');
                AppendTwoDigit(sb, dt.Minute); sb.Append(':');
                AppendTwoDigit(sb, dt.Second); sb.Append('.');
                AppendThreeDigit(sb, dt.Millisecond);
            }
        }

        /// <summary>
        /// 附加 thread 區段。規則:
        ///   - 只開 ShowThreadId(或 ShowThreadName 但 Name 為 null) → "[T:12] "
        ///   - ShowThreadId 開,且 ShowThreadName 開且 Name 非 null → "[T:12/Name] "
        ///   - 只開 ShowThreadName 且 Name 非 null → "[N:Name] "
        ///   - 兩者都關 或 對應條件不成立 → 整個區段省略
        /// </summary>
        private static void AppendThreadSegment(StringBuilder sb, int threadId, string threadName,
            bool showId, bool showName)
        {
            var hasName = showName && !string.IsNullOrEmpty(threadName);

            if (showId && hasName)
            {
                sb.Append("[T:").Append(threadId).Append('/').Append(threadName).Append("] ");
            }
            else if (showId)
            {
                sb.Append("[T:").Append(threadId).Append("] ");
            }
            else if (hasName)
            {
                sb.Append("[N:").Append(threadName).Append("] ");
            }
            // 兩者都關 / name 為 null → 不輸出任何 thread 區段
        }

        private static void AppendTwoDigit(StringBuilder sb, int value)
        {
            if (value < 10) sb.Append('0');
            sb.Append(value);
        }

        private static void AppendThreeDigit(StringBuilder sb, int value)
        {
            if (value < 100) sb.Append('0');
            if (value < 10) sb.Append('0');
            sb.Append(value);
        }
    }
}
