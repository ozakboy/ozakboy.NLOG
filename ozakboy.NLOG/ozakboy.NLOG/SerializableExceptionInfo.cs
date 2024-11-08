using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ozakboy.NLOG
{
    /// <summary>
    /// 用於序列化的異常資料類別，提供異常資訊的結構化存儲
    /// </summary>
    public class SerializableExceptionInfo
    {
        /// <summary>
        /// 異常類型的完整名稱
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 異常的錯誤訊息
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

        /// <summary>
        /// 將 Exception 物件轉換為可序列化的異常資訊
        /// </summary>
        /// <param name="ex">要轉換的異常物件</param>
        /// <returns>可序列化的異常資訊物件</returns>
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
