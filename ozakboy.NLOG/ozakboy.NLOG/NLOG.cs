using System;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text;
using ozakboy.NLOG.Core;
using System.Xml.Linq;

namespace ozakboy.NLOG
{
    /// <summary>
    /// 記錄檔
    /// </summary>
    public static partial class LOG
    {
        #region 核心日誌方法

        private static void Log(LogLevel level, string name = "", string message = "", bool writeTxt = true, string[] args = null)
        {
            var escapedMessage = LogFormatter.EscapeMessage(message);
            var formattedMessage = LogFormatter.FormatMessage(escapedMessage, args ?? Array.Empty<string>());

            if(LogConfiguration.Current.EnableConsoleOutput)
                Console.WriteLine(formattedMessage, args);
            if (writeTxt)            
                 LogText.Add_LogText(level, name, formattedMessage, args ?? Array.Empty<string>());            

        }

        private static void LogObject<T>(LogLevel level, T obj, string name = "", string message = "", bool writeTxt = true, string[] args = null) where T : class
        {
            if (obj == null) return;

            try
            {
                string jsonString = message + "\n";
                if (level >= LogLevel.Warn && obj is Exception ex)
                {
                    jsonString += LogSerializer.SerializeException(ex);
                }
                else
                {
                    jsonString += LogSerializer.SerializeObject(obj);
                }
                Log(level, name, jsonString, writeTxt);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, name, ExceptionHandler.HandleSerializationException(ex));
            }
        }
        #endregion

        #region 公開方法 - 各種日誌級別

        // Trace
        public static void Trace_Log(string message) => Log(LogLevel.Trace,string.Empty, message);
        public static void Trace_Log(string message, bool writeTxt) => Log(LogLevel.Trace,string.Empty, message, writeTxt);
        public static void Trace_Log(string message, bool writeTxt, string[] args) => Log(LogLevel.Trace,string.Empty, message, writeTxt, args);
        public static void Trace_Log<T>(T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Trace, obj, string.Empty, string.Empty, writeTxt);
        public static void Trace_Log<T>(string message, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Trace, obj, string.Empty, message, writeTxt);

        // Debug
        public static void Debug_Log(string message) => Log(LogLevel.Debug, message);
        public static void Debug_Log(string message, bool writeTxt) => Log(LogLevel.Debug, string.Empty, message, writeTxt);
        public static void Debug_Log(string message, bool writeTxt, string[] args) => Log(LogLevel.Debug, string.Empty, message, writeTxt, args);
        public static void Debug_Log<T>(T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Debug, obj, string.Empty, string.Empty , writeTxt);
        public static void Debug_Log<T>(string message, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Debug, obj, string.Empty, message, writeTxt);

        // Info
        public static void Info_Log(string message) => Log(LogLevel.Info, message);
        public static void Info_Log(string message, bool writeTxt) => Log(LogLevel.Info, string.Empty, message, writeTxt);
        public static void Info_Log(string message, bool writeTxt, string[] args) => Log(LogLevel.Info, string.Empty, message, writeTxt, args);
        public static void Info_Log<T>(T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Info, obj, string.Empty, string.Empty,  writeTxt);
        public static void Info_Log<T>(string message, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Info, obj, string.Empty, message, writeTxt);

        // Warn
        public static void Warn_Log(string message) => Log(LogLevel.Warn, message);
        public static void Warn_Log(string message, bool writeTxt) => Log(LogLevel.Warn, string.Empty, message, writeTxt);
        public static void Warn_Log(string message, bool writeTxt, string[] args) => Log(LogLevel.Warn, string.Empty, message, writeTxt, args);
        public static void Warn_Log<T>(T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Warn, obj, string.Empty, string.Empty,  writeTxt);
        public static void Warn_Log<T>(string message, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Warn, obj, string.Empty, message, writeTxt);

        // Error
        public static void Error_Log(string message) => Log(LogLevel.Error, message);
        public static void Error_Log(string message, bool writeTxt) => Log(LogLevel.Error, string.Empty, message, writeTxt);
        public static void Error_Log(string message, bool writeTxt, string[] args) => Log(LogLevel.Error, string.Empty, message, writeTxt, args);
        public static void Error_Log<T>(T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Error, obj, string.Empty, string.Empty,  writeTxt);
        public static void Error_Log<T>(string message, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Error, obj, string.Empty, message, writeTxt);

        // Fatal
        public static void Fatal_Log(string message) => Log(LogLevel.Fatal, message);
        public static void Fatal_Log(string message, bool writeTxt) => Log(LogLevel.Fatal, string.Empty, message, writeTxt);
        public static void Fatal_Log(string message, bool writeTxt, string[] args) => Log(LogLevel.Fatal, string.Empty, message, writeTxt, args);
        public static void Fatal_Log<T>(T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Fatal, obj, string.Empty, string.Empty, writeTxt);
        public static void Fatal_Log<T>(string message, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.Fatal, obj, string.Empty, message, writeTxt);

        //CustomName
        public static void CustomName_Log(string name ,string message) => Log(LogLevel.CostomName , name, message);
        public static void CustomName_Log(string name, string message, bool writeTxt) => Log(LogLevel.CostomName, name, message, writeTxt);
        public static void CustomName_Log(string name, string message, bool writeTxt, string[] args) => Log(LogLevel.CostomName, name, message, writeTxt, args);
        public static void CustomName_Log<T>(string name, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.CostomName, obj, name, string.Empty,  writeTxt);
        public static void CustomName_Log<T>(string name, string message, T obj, bool writeTxt = true) where T : class => LogObject(LogLevel.CostomName, obj, name, message, writeTxt);


        #endregion


        #region  LOG預設檔案配置

        /// <summary>
        /// 配置日誌系統
        /// </summary>
        /// <param name="configure">配置動作</param>
        public static void Configure(Action<LogConfiguration.LogOptions> configure)
        {
            LogConfiguration.Initialize(configure);
        }

        /// <summary>
        /// 取得當前日誌配置
        /// </summary>
        public static LogConfiguration.ILogOptions GetCurrentOptions()
        {
            return LogConfiguration.GetCurrentOptions();
        }

        #endregion

    }
}
