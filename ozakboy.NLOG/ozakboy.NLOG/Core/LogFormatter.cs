using System;
using System.Text;
using System.Threading;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌格式化處理器
    /// </summary>
    internal static class LogFormatter
    {
        /// <summary>
        /// 格式化日誌訊息
        /// </summary>
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
        /// 處理訊息中的特殊字符
        /// </summary>
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