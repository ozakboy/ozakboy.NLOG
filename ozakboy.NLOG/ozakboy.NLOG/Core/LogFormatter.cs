using System;
using System.Text;
using System.Threading;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌格式化處理器 - 負責將日誌內容轉換為標準格式
    /// Log Formatter - Responsible for converting log content into standardized format
    /// </summary>
    internal static class LogFormatter
    {
        /// <summary>
        /// 格式化日誌訊息 - 將原始訊息轉換為包含時間戳記和執行緒 ID 的格式化訊息
        /// Format Log Message - Converts raw message into formatted message with timestamp and thread ID
        /// </summary>
        /// <param name="message">要格式化的訊息 / Message to format</param>
        /// <param name="args">格式化參數 / Formatting parameters</param>
        /// <returns>格式化後的訊息 / Formatted message</returns>
        public static string FormatMessage(string message, string[] args)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.Append($"[{Thread.CurrentThread.ManagedThreadId}] ");

            if (args != null && args.Length > 0)
                sb.AppendFormat(message, args);
            else
                sb.Append(message);

            return sb.ToString();
        }

        /// <summary>
        /// 處理訊息中的特殊字符 - 確保訊息中的格式化字符被正確處理
        /// Handle Special Characters - Ensures format characters in messages are properly handled
        /// </summary>
        /// <param name="message">原始訊息 / Original message</param>
        /// <returns>處理後的訊息 / Processed message</returns>
        public static string EscapeMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return message;

            if (message.Contains("{") || message.Contains("}"))
            {
                bool containsFormatting = System.Text.RegularExpressions.Regex.IsMatch(message, @"\{[0-9]+\}");
                if (!containsFormatting)
                {
                    return message.Replace("{", "{{").Replace("}", "}}");
                }
            }
            return message;
        }
    }
}