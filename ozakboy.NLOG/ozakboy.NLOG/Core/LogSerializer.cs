using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌序列化處理器
    /// </summary>
    internal static class LogSerializer
    {
        // 使用傳統的初始化方式以確保跨版本兼容
        /// <summary>
        /// 預設的 JSON 序列化選項
        /// Default JSON serialization options
        /// </summary>
        private static readonly JsonSerializerOptions _defaultOptions;
        /// <summary>
        /// 異常序列化專用的 JSON 序列化選項
        /// JSON serialization options specifically for exceptions
        /// </summary>
        private static readonly JsonSerializerOptions _exceptionOptions;

        // 在靜態建構函數中初始化
        static LogSerializer()
        {
            _defaultOptions = new JsonSerializerOptions()
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            _exceptionOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
           
        }

        /// <summary>
        /// 序列化一般物件
        /// </summary>
        public static string SerializeObject<T>(T obj) where T : class
        {
            if (obj == null) return null;
            return JsonSerializer.Serialize(obj, _defaultOptions);
        }

        /// <summary>
        /// 序列化異常物件
        /// </summary>
        public static string SerializeException(Exception ex)
        {
            if (ex == null) return null;
            var serializableEx = ExceptionHandler.CreateSerializableException(ex);
            return JsonSerializer.Serialize(serializableEx, _exceptionOptions);
        }
    }
}