using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌格式化處理器 - 將 LogItem 轉為要寫入檔案的單行字串。
    /// v3.0：格式化全部移到 dispatcher 執行緒，呼叫端不再做 string.Format / StringBuilder。
    /// </summary>
    internal static class LogFormatter
    {
        /// <summary>
        /// 格式化 LogItem 為單行檔案輸出（不含換行符；換行由 StreamWriter.WriteLine 補）
        /// 格式：HH:mm:ss.fff[T:tid] message
        /// </summary>
        public static string Format(in LogItem item)
        {
            var dt = new DateTime(item.TimestampTicks, DateTimeKind.Local);

            // 容量估計：時間戳 12 + thread 包裝 8 + message 概略長度
            var sb = new StringBuilder(64);

            // 時間戳 HH:mm:ss.fff（手寫格式比 ToString("HH:mm:ss.fff") 略快且零配置）
            AppendTwoDigit(sb, dt.Hour); sb.Append(':');
            AppendTwoDigit(sb, dt.Minute); sb.Append(':');
            AppendTwoDigit(sb, dt.Second); sb.Append('.');
            AppendThreeDigit(sb, dt.Millisecond);

            sb.Append("[T:").Append(item.ThreadId).Append("] ");

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
