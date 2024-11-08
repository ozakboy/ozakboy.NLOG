using System;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ozakboy.NLOG
{
    /// <summary>
    /// 記錄檔
    /// </summary>
    public static class LOG
    {
        #region 追蹤記錄檔

        /// <summary>
        /// 記錄任意物件的擴充方法
        /// </summary>
        public static void Trace_Log<T>(T obj, bool writeTxt = true) where T : class
        {
            if (obj == null) return;
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, _defaultJsonOptions);
                Trace_Log(jsonString, writeTxt);
            }
            catch (Exception ex)
            {
                Error_Log($"JSON序列化失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 追蹤記錄檔
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="WriteTxt">要不要寫Text</param>
        /// <param name="arg"></param>
        public static void Trace_Log(string Message, bool WriteTxt, string[] arg )
        {
            var escapedMessage = EscapeMessageIfNeeded(Message);
            var formattedMessage = FormatLogMessage(escapedMessage,arg);
            Console.WriteLine(formattedMessage, arg);
            if (WriteTxt)
                LogText.Add_LogText("Trace", formattedMessage, arg);
        }
        /// <summary>
        /// 追蹤記錄檔
        /// </summary>
        /// <param name="Message"></param>       
        public static void Trace_Log(string Message)
        {
            Trace_Log(Message, true, new string[0]);
        }

        /// <summary>
        /// 追蹤記錄檔
        /// </summary>
        /// <param name="Message"></param>       
        /// <param name="WriteTxt">要不要寫Text</param>
        public static void Trace_Log(string Message, bool WriteTxt)
        {
            Trace_Log(Message, WriteTxt, new string[0]);
        }

        #endregion

        #region 測試記錄檔


        /// <summary>
        /// 記錄任意物件的擴充方法
        /// </summary>
        public static void Debug_Log<T>(T obj, bool writeTxt = true) where T : class
        {
            if (obj == null) return;
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, _defaultJsonOptions);
                Debug_Log(jsonString, writeTxt);
            }
            catch (Exception ex)
            {
                Error_Log($"JSON序列化失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="WriteTxt">要不要寫Text</param>
        /// <param name="arg">正規化文字</param>
        public static void Debug_Log(string Message, bool WriteTxt, string[] arg)
        {
            var escapedMessage = EscapeMessageIfNeeded(Message);
            var formattedMessage = FormatLogMessage(escapedMessage, arg);
            Console.WriteLine(formattedMessage, arg);
            if (WriteTxt)
                LogText.Add_LogText("Debug", formattedMessage, arg);            
        }

        /// <summary>
        /// 測試記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>       
        public static void Debug_Log(string Message)
        {
            Debug_Log(Message , true, new string[0]);
        }
        /// <summary>
        /// 測試記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>       
        /// <param name="WriteTxt">要不要寫Text</param>
        public static void Debug_Log(string Message, bool WriteTxt)
        {
            Debug_Log(Message, WriteTxt, new string[0]);
        }

        #endregion


        #region 訊息記錄檔

        /// <summary>
        /// 記錄任意物件的擴充方法
        /// </summary>
        public static void Info_Log<T>(T obj, bool writeTxt = true) where T : class
        {
            if (obj == null) return;
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, _defaultJsonOptions);
                Info_Log(jsonString, writeTxt);
            }
            catch (Exception ex)
            {
                Error_Log($"JSON序列化失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 訊息記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="WriteTxt">要不要寫Text</param>
        /// <param name="arg">正規化文字</param>
        public static void Info_Log(string Message, bool WriteTxt, string[] arg)
        {
            var escapedMessage = EscapeMessageIfNeeded(Message);
            var formattedMessage = FormatLogMessage(escapedMessage, arg);
            Console.WriteLine(formattedMessage, arg);
            if (WriteTxt)
                LogText.Add_LogText("Info", formattedMessage, arg);           
        }

        /// <summary>
        /// 訊息記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>       
        public static void Info_Log(string Message)
        {
            Info_Log(Message,true, new string[0]);
        }

        /// <summary>
        /// 訊息記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>         
        /// <param name="WriteTxt">要不要寫Text</param>
        public static void Info_Log(string Message, bool WriteTxt)
        {
            Info_Log(Message, WriteTxt, new string[0]);
        }


        #endregion 訊息記錄檔

        #region 警告記錄檔
        /// <summary>
        /// 記錄任意物件的擴充方法
        /// </summary>
        public static void Warn_Log<T>(T obj, bool writeTxt = true) where T : class
        {
            if (obj == null) return;
            try
            {
                if (obj is Exception ex)
                {
                    var serializableEx = SerializableExceptionInfo.FromException(ex);
                    string jsonString = JsonSerializer.Serialize(serializableEx, _exceptionJsonOptions);
                    Warn_Log(jsonString, writeTxt);
                }
                else
                {
                    string jsonString = JsonSerializer.Serialize(obj, _defaultJsonOptions);
                    Warn_Log(jsonString, writeTxt);
                }                
            }
            catch (Exception ex)
            {
                Error_Log($"JSON序列化失敗: {ex.Message}");
            }
        }


        /// <summary>
        /// 警告記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="WriteTxt">要不要寫Text</param>
        /// <param name="arg">正規化文字</param>
        public static void Warn_Log(string Message, bool WriteTxt, string[] arg)
        {
            var escapedMessage = EscapeMessageIfNeeded(Message);
            var formattedMessage = FormatLogMessage(escapedMessage, arg);
            Console.WriteLine(formattedMessage, arg);
            if (WriteTxt)
                LogText.Add_LogText("Warn", formattedMessage, arg);
        }

        /// <summary>
        /// 警告記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>       
        public static void Warn_Log(string Message)
        {
            Warn_Log(Message, true, new string[0]);
        }

        /// <summary>
        /// 警告記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>       
        /// <param name="WriteTxt">要不要寫Text</param>
        public static void Warn_Log(string Message, bool WriteTxt)
        {
            Warn_Log(Message, WriteTxt, new string[0]);
        }     

        #endregion

        #region 錯誤記錄檔

        /// <summary>
        /// 記錄任意物件的擴充方法
        /// </summary>
        public static void Error_Log<T>(T obj, bool writeTxt = true) where T : class
        {
            if (obj == null) return;
            try
            {
                if (obj is Exception ex)
                {
                    var serializableEx = SerializableExceptionInfo.FromException(ex);
                    string jsonString = JsonSerializer.Serialize(serializableEx, _exceptionJsonOptions);
                    Error_Log(jsonString, writeTxt);
                }
                else
                {
                    string jsonString = JsonSerializer.Serialize(obj, _defaultJsonOptions);
                    Error_Log(jsonString, writeTxt);
                }
             
            }
            catch (Exception ex)
            {
                Error_Log($"JSON序列化失敗: {ex.Message}");
            }
        }
        /// <summary>
        /// 錯誤紀錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="WriteTxt">要不要寫Text</param>
        /// <param name="arg">正規化文字</param>
        public static void Error_Log(string Message, bool WriteTxt, string[] arg)
        {
            var escapedMessage = EscapeMessageIfNeeded(Message);
            var formattedMessage = FormatLogMessage(escapedMessage, arg);
            Console.WriteLine(formattedMessage, arg);
            if (WriteTxt)
                LogText.Add_LogText("Error", formattedMessage, arg);
        }

        /// <summary>
        /// 錯誤紀錄檔
        /// </summary>
        /// <param name="Message">訊息</param>      
        public static void Error_Log(string Message)
        {
            Error_Log(Message,true, new string[0]);
        }

        /// <summary>
        /// 錯誤紀錄檔
        /// </summary>
        /// <param name="Message">訊息</param>      
        /// <param name="WriteTxt">要不要寫Text</param>
        public static void Error_Log(string Message, bool WriteTxt)
        {
            Error_Log(Message, WriteTxt, new string[0]);
        }

        #endregion

        #region 致命記錄檔

        /// <summary>
        /// 致命記錄檔 記錄任意物件的擴充方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writeTxt">是否寫LOG</param>
        /// <param name="obj">物件</param>
        public static void Fatal_Log<T>(T obj, bool writeTxt = true) where T : class
        {
            if (obj == null) return;

            try
            {
                if (obj is Exception ex)
                {
                    var serializableEx = SerializableExceptionInfo.FromException(ex);
                    string jsonString = JsonSerializer.Serialize(serializableEx, _exceptionJsonOptions);
                    Fatal_Log(jsonString, writeTxt);
                }
                else
                {
                    string jsonString = JsonSerializer.Serialize(obj, _defaultJsonOptions);
                    Fatal_Log(jsonString , writeTxt);
                }
            }
            catch (Exception ex)
            {
                // 如果是異常物件且序列化失敗，使用基本異常格式
                if (obj is Exception exception)
                {
                    string message = string.Empty;
                    GetExceptionMessage(exception, ref message);
                    Fatal_Log(message);
                }
                else
                {
                    Error_Log($"JSON序列化失敗: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 致命記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="arg">正規化文字</param>
        public static void Fatal_Log(string Message, string[] arg)
        {
            var escapedMessage = EscapeMessageIfNeeded(Message);
            var formattedMessage = FormatLogMessage(escapedMessage, arg);
            Console.WriteLine(formattedMessage, arg);
            LogText.Add_LogText("Fatal", formattedMessage, arg);
        }
        /// <summary>
        /// 致命記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>                     
        public static void Fatal_Log(string Message)
        {
            Fatal_Log(Message, new string[0]);
        }

        #endregion

        #region 自定義名稱Log記錄檔
        /// <summary>
        /// 記錄任意物件的擴充方法
        /// </summary>
        public static void CostomName_Log<T>(string custom, T obj, bool writeTxt = true) where T : class
        {
            if (obj == null) return;
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, _defaultJsonOptions);
                CostomName_Log(custom, jsonString, writeTxt);
            }
            catch (Exception ex)
            {
                Error_Log($"JSON序列化失敗: {ex.Message}");
            }
        }
        /// <summary>
        /// 自定義名稱Log記錄檔
        /// </summary>
        /// <param name="Custom">自定義名稱</param>
        /// <param name="Message">訊息</param>
        /// <param name="WriteTxt">要不要寫Text</param>
        /// <param name="arg">正規化文字</param>
        public static void CostomName_Log(string Custom, string Message, bool WriteTxt, string[] arg)
        {
            var escapedMessage = EscapeMessageIfNeeded(Message);
            var formattedMessage = FormatLogMessage(escapedMessage, arg);
            Console.WriteLine(formattedMessage, arg);
            if (WriteTxt)
                LogText.Add_LogText(Custom, formattedMessage, arg);
        }

        /// <summary>
        /// 自定義名稱Log記錄檔
        /// </summary>
        /// <param name="Custom">自定義名稱</param>
        /// <param name="Message">訊息</param>
        public static void CostomName_Log(string Custom, string Message)
        {
            CostomName_Log(Custom, Message , true, new string[0]);
        }

        /// <summary>
        /// 自定義名稱Log記錄檔
        /// </summary>
        /// <param name="Custom">自定義名稱</param>
        /// <param name="Message">訊息</param>
        /// <param name="WriteTxt">要不要寫Text</param>
        public static void CostomName_Log(string Custom, string Message, bool WriteTxt)
        {
            CostomName_Log(Custom, Message, WriteTxt, new string[0]);
        }

        /// <summary>
        /// 自定義名稱Log記錄檔
        /// </summary>
        /// <param name="Custom">自定義名稱</param>
        /// <param name="ex">例外</param>
        public static void CostomName_Log(string Custom, Exception ex)
        {
            string Message = string.Empty;
            GetExceptionMessage(ex, ref Message);
            CostomName_Log(Custom, Message);
        }

        #endregion

        #region 設定Log紀錄保存天數

        /// <summary>
        /// Log紀錄檔保存天數  預設3天(-3) 
        /// 請設定天數為負數
        /// </summary>
        /// <param name="KeepDay">保留天數</param>
        public static void SetLogKeepDay(int KeepDay)
        {
            if (KeepDay > 0)
                KeepDay = Math.Abs(KeepDay) * -1;
            LogText.LogKeepDay = KeepDay;
        }

        #endregion

        #region 設定預設最大檔案限制

        /// <summary>
        /// 設定預設最大檔案限制
        /// 預設最大檔限制 50MB
        /// </summary>
        /// <param name="_BigFilesByte">最大檔案位元組限制</param>
        public static void SetBigFilesByte(long _BigFilesByte)
        {
            LogText.BigFilesByte = _BigFilesByte;
        }

        #endregion

        #region 私用方法

        private static void GetExceptionMessage(Exception ex, ref String Message)
        {
            if (ex.InnerException != null && !String.IsNullOrEmpty(ex.InnerException.StackTrace))
            {
                GetExceptionMessage(ex.InnerException, ref Message);
            }
            Message += "\n" + ex.Message;
            Message += "\n" + ex.StackTrace;
        }

        private static string FormatLogMessage(string message, string[] args)
        {
            string timestamp = $"{DateTime.Now:HH:mm:ss}[{Thread.CurrentThread.ManagedThreadId}] ";
            if (args != null && args.Length > 0)
            {
                // 先處理訊息的格式化
                message = string.Format(message, args);
            }
            return timestamp + message;
        }

        // 处理 JSON 字符串的帮助方法
        private static string EscapeMessageIfNeeded(string message)
        {
            if (message.Contains("{") || message.Contains("}"))
            {
                // 檢查是否包含格式化佔位符 {0}, {1} 等
                bool containsFormatting = System.Text.RegularExpressions.Regex.IsMatch(message, @"\{[0-9]+\}");
                if (!containsFormatting)
                {
                    // 如果不是格式化字串，則進行 JSON 轉義
                    return message.Replace("{", "{{").Replace("}", "}}");
                }
            }
            return message;
        }

        private static readonly JsonSerializerOptions _defaultJsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static readonly JsonSerializerOptions _exceptionJsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static Dictionary<string, object> GetAdditionalExceptionProperties(Exception ex)
        {
            var properties = new Dictionary<string, object>();

            // 使用反射獲取額外的公開屬性
            var type = ex.GetType();
            var standardProperties = new HashSet<string> { "Message", "StackTrace", "Source", "HelpLink", "InnerException" };

            foreach (var prop in type.GetProperties())
            {
                if (!standardProperties.Contains(prop.Name))
                {
                    try
                    {
                        var value = prop.GetValue(ex);
                        if (value != null)
                        {
                            properties[prop.Name] = value;
                        }
                    }
                    catch
                    {
                        // 忽略無法獲取的屬性
                    }
                }
            }

            return properties;
        }

        #endregion

    }
}
