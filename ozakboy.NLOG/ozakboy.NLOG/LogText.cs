using System.Text;
using System.IO;
using System;

namespace ozakboy.NLOG
{
    /// <summary>
    /// 建立 記錄檔
    /// </summary>
    static class LogText
    {
        private static object lockMe = new object();
        /// <summary>
        /// Log紀錄檔保存天數  預設3天(-3) 
        /// 請設定天數為負數
        /// </summary>
        public static int LogKeepDay = -3;
        public static int BigFilesByte = 1024;

        /// <summary>
        /// 建立或是新增LOG紀錄
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Message"></param>
        /// <param name="arg"></param>
        internal static void Add_LogText(string Type, string Message, object[] arg)
        {
            try
            {
                // 使用 lock 避免多執行敘執行時 檔案被佔用問題
                lock (lockMe)
                {                  
                    string LogPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\LogFiles\\";

                    //判斷有無資料表 若沒有建立資料表
                    if (!Directory.Exists(LogPath))
                    {
                        Directory.CreateDirectory(LogPath);
                    }

                    var s_File_Path = $"{LogPath}{DateTime.Now.ToString("yyyyMMdd")}_{Type}_LOG.txt";

                    //判斷有無檔案，若沒有則建立檔案
                    if (!File.Exists(s_File_Path))
                    {
                        using (FileStream fileStream = new FileStream(s_File_Path, FileMode.Create))
                        {
                            fileStream.Close();
                        }
                    }


                    using (StreamWriter sw = new StreamWriter(s_File_Path, true, Encoding.UTF8))
                    {
                        sw.WriteLine(Message, arg);
                        sw.Close();
                    }
                   
                    Remove_TimeOutLogText();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}>>LogText Add_LogText Error:{ex.Message}");
            }
        }

        /// <summary>
        /// 刪除過久紀錄檔
        /// </summary>
        /// <param name="Type"></param>
        internal static void Remove_LogText(string Type)
        {            
            string LogPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\LogFiles\\";

            var s_File_Path = $"{LogPath}{DateTime.Now.AddDays(LogKeepDay).ToString("yyyyMMdd")}_{Type}_LOG.txt";
            //判斷有無檔案，若有則刪除檔案
            if (File.Exists(s_File_Path))
            {
                File.Delete(s_File_Path);
            }
        }


        internal static void Remove_TimeOutLogText()
        {
            string LogPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\LogFiles\\";
            DirectoryInfo di = new DirectoryInfo(LogPath);
            var LastKeepDate = DateTime.UtcNow.AddDays(LogKeepDay);
            var PathFiles = di.GetFiles().Where(x => x.Extension == ".txt" && x.LastWriteTimeUtc < LastKeepDate);
            foreach (var item in PathFiles)
            {
                item.Delete();
            }
        }
    }
}
