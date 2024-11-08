using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ozakboy.NLOG
{
    /// <summary>
    /// 用於序列化的異常資料類別
    /// </summary>
    public class SerializableExceptionInfo
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string HelpLink { get; set; }
        public string StackTrace { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public SerializableExceptionInfo InnerException { get; set; }
        public Dictionary<string, string> AdditionalProperties { get; set; }

        public static SerializableExceptionInfo FromException(Exception ex)
        {
            if (ex == null) return null;

            var info = new SerializableExceptionInfo
            {
                Type = ex.GetType().FullName,
                Message = ex.Message.Trim(),
                Source = ex.Source,
                HelpLink = ex.HelpLink,
                StackTrace = ex.StackTrace.Trim(),
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
                info.InnerException = FromException(ex.InnerException);
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
    }

}
