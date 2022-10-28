using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ozakboy.NLOG
{
    /// <summary>
    /// 記錄檔
    /// </summary>
    public static class LOG
    {
        #region 追蹤記錄檔

        /// <summary>
        /// 追蹤記錄檔
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="WriteTxt"></param>
        /// <param name="arg"></param>
        public static void Trace_Log(string Message, bool WriteTxt, string[] arg )
        {
            Message = $"{DateTime.Now.ToString("HH:mm:ss")}[{Thread.CurrentThread.ManagedThreadId}] {Message}";          
            Console.WriteLine(Message, arg);
            if(WriteTxt)
                LogText.Add_LogText("Trace", Message, arg);
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
        /// <param name="WriteTxt"></param>       
        public static void Trace_Log(string Message, bool WriteTxt)
        {
            Trace_Log(Message, WriteTxt, new string[0]);
        }

        #endregion

        #region 測試記錄檔

        /// <summary>
        /// 測試記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="arg">正規化文字</param>
        public static void Debug_Log(string Message, bool WriteTxt, string[] arg)
        {
            Message = $"{DateTime.Now.ToString("HH:mm:ss")}[{Thread.CurrentThread.ManagedThreadId}] {Message}";
            Console.WriteLine(Message, arg);
            if(WriteTxt)
                LogText.Add_LogText("Debug", Message, arg);
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
        public static void Debug_Log(string Message, bool WriteTxt)
        {
            Debug_Log(Message, WriteTxt, new string[0]);
        }

        #endregion


        #region 訊息記錄檔

        /// <summary>
        /// 訊息記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="arg">正規化文字</param>
        public static void Info_Log(string Message, bool WriteTxt, string[] arg)
        {
            Message = $"{DateTime.Now.ToString("HH:mm:ss")}[{Thread.CurrentThread.ManagedThreadId}] {Message}";
            Console.WriteLine(Message, arg);
            if(WriteTxt)
                LogText.Add_LogText("Info", Message, arg);
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
        public static void Info_Log(string Message, bool WriteTxt)
        {
            Info_Log(Message, WriteTxt, new string[0]);
        }


        #endregion 訊息記錄檔

        #region 警告記錄檔

        /// <summary>
        /// 警告記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="arg">正規化文字</param>
        public static void Warn_Log(string Message, bool WriteTxt, string[] arg)
        {
            Message = $"{DateTime.Now.ToString("HH:mm:ss")}[{Thread.CurrentThread.ManagedThreadId}] {Message}";
            Console.WriteLine(Message, arg);
            if(WriteTxt)
                LogText.Add_LogText("Warn", Message, arg);
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
        public static void Warn_Log(string Message, bool WriteTxt)
        {
            Warn_Log(Message, WriteTxt, new string[0]);
        }

        /// <summary>
        /// 警告記錄檔
        /// </summary>
        /// <param name="ex">例外</param>
        public static void Warn_Log(Exception ex)
        {
            string Message = string.Empty;
            if (ex != null) 
            {                
                GetExceptionMessage(ex, ref  Message);
                Warn_Log(Message);
            }
            else
            {
                Warn_Log(Message);
            }
        }

        /// <summary>
        /// 警告記錄檔
        /// </summary>
        /// <param name="ex">例外</param>
        public static void Warn_Log(ErrorMessageException ex)
        {
            string Message = string.Empty;
            if (ex != null)
            {
                Message = ex.Message;
                Warn_Log(Message);
            }
            else
            {
                Warn_Log(Message);
            }
        }

        /// <summary>
        /// 警告記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="ex">例外</param>
        public static void Warn_Log(string Message, Exception ex)
        {
            if (ex != null)
            {
                GetExceptionMessage(ex, ref Message);
            }
            Warn_Log(Message);
        }

        private static void GetExceptionMessage(Exception ex, ref String Message)
        {
            if (ex.InnerException != null && !String.IsNullOrEmpty(ex.InnerException.StackTrace))
            {
                GetExceptionMessage(ex.InnerException, ref Message);
            }
            Message += "\n" + ex.Message;
            Message += "\n" + ex.StackTrace;
        }

        #endregion

        #region 錯誤記錄檔

        /// <summary>
        /// 錯誤紀錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="arg">正規化文字</param>
        public static void Error_Log(string Message, bool WriteTxt, string[] arg)
        {
            Message = $"{DateTime.Now.ToString("HH:mm:ss")}[{Thread.CurrentThread.ManagedThreadId}] {Message}";
            Console.WriteLine(Message, arg);
            if(WriteTxt)
                LogText.Add_LogText("Error", Message, arg);
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
        public static void Error_Log(string Message, bool WriteTxt)
        {
            Error_Log(Message, WriteTxt, new string[0]);
        }

        #endregion

        #region 致命記錄檔

        /// <summary>
        /// 致命記錄檔
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="arg">正規化文字</param>
        public static void Fatal_Log(string Message, string[] arg)
        {
            Message = $"{DateTime.Now.ToString("HH:mm:ss")}[{Thread.CurrentThread.ManagedThreadId}] {Message}";
            Console.WriteLine(Message, arg);
            LogText.Add_LogText("Fatal", Message, arg);
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
        /// 自定義名稱Log記錄檔
        /// </summary>
        /// <param name="Custom">自定義名稱</param>
        /// <param name="Message">訊息</param>
        /// <param name="arg">正規化文字</param>
        public static void CostomName_Log(string Custom, string Message, bool WriteTxt, string[] arg)
        {
            Message = $"{DateTime.Now.ToString("HH:mm:ss")}[{Thread.CurrentThread.ManagedThreadId}] {Message}";
            Console.WriteLine(Message, arg);
            if(WriteTxt)
                LogText.Add_LogText(Custom, Message, arg);
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

    }
}
