using System;
using System.Collections;
using System.Collections.Generic;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 異常處理器 - 負責收集和序列化異常資訊
    /// Exception Handler - Responsible for collecting and serializing exception information
    /// </summary>
    internal static class ExceptionHandler
    {
        /// <summary>
        /// 用於序列化的異常資訊類別 - 提供異常資訊的結構化存儲
        /// Serializable Exception Info Class - Provides structured storage for exception information
        /// </summary>
        public class SerializableExceptionInfo
        {
            /// <summary>
            /// 異常類型的完整名稱 - 包含命名空間的類型名稱
            /// Full Exception Type Name - Type name with namespace
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// 異常的錯誤訊息 - 描述錯誤的具體內容
            /// Exception Message - Detailed description of the error
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// 異常來源
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// 異常的幫助連結
            /// </summary>
            public string HelpLink { get; set; }

            /// <summary>
            /// 異常的堆疊追蹤
            /// </summary>
            public string StackTrace { get; set; }

            /// <summary>
            /// 異常的額外資料字典
            /// </summary>
            public Dictionary<string, string> Data { get; set; }

            /// <summary>
            /// 內部異常資訊
            /// </summary>
            public SerializableExceptionInfo InnerException { get; set; }

            /// <summary>
            /// 額外的異常屬性
            /// </summary>
            public Dictionary<string, string> AdditionalProperties { get; set; }
        }

        /// <summary>
        /// 獲取完整的異常訊息文字，包含內部異常
        /// </summary>
        public static string GetFullExceptionMessage(Exception ex)
        {
            if (ex == null) return string.Empty;

            var message = string.Empty;
            BuildExceptionMessage(ex, ref message);
            return message;
        }

        /// <summary>
        /// 將異常轉換為可序列化的格式
        /// </summary>
        public static SerializableExceptionInfo CreateSerializableException(Exception ex)
        {
            if (ex == null) return null;

            var info = new SerializableExceptionInfo
            {
                Type = ex.GetType().FullName,
                Message = ex.Message?.Trim(),
                Source = ex.Source,
                HelpLink = ex.HelpLink,
                StackTrace = ex.StackTrace?.Trim(),
                Data = new Dictionary<string, string>(),
                AdditionalProperties = new Dictionary<string, string>()
            };

            // 處理 Data 字典
            if (ex.Data != null && ex.Data.Count > 0)
            {
                foreach (DictionaryEntry entry in ex.Data)
                {
                    info.Data[entry.Key?.ToString() ?? "null"] = entry.Value?.ToString() ?? "null";
                }
            }

            // 處理內部異常
            if (ex.InnerException != null)
            {
                info.InnerException = CreateSerializableException(ex.InnerException);
            }

            // 處理額外屬性
            var standardProps = new HashSet<string>
            {
                "Message", "StackTrace", "Source", "HelpLink",
                "InnerException", "Data", "TargetSite"
            };

            foreach (var prop in ex.GetType().GetProperties())
            {
                try
                {
                    if (!standardProps.Contains(prop.Name) && prop.CanRead)
                    {
                        var value = prop.GetValue(ex);
                        if (value != null)
                        {
                            info.AdditionalProperties[prop.Name] = value.ToString();
                        }
                    }
                }
                catch
                {
                    // 忽略無法獲取的屬性
                }
            }

            return info;
        }

        /// <summary>
        /// 處理序列化異常的錯誤訊息
        /// </summary>
        public static string HandleSerializationException(Exception ex)
        {
            return $"序列化異常時發生錯誤: {ex.Message}";
        }

        private static void BuildExceptionMessage(Exception ex, ref string message)
        {
            if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.StackTrace))
            {
                BuildExceptionMessage(ex.InnerException, ref message);
            }
            message += "\n" + ex.Message;
            message += "\n" + ex.StackTrace;
        }
    }
}