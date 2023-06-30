using System;
using System.IO;
using System.Linq;
using System.Text;

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
        /// <summary>
        /// 預設最大檔案 50MB 超過自動分割檔案
        /// </summary>
        public static long BigFilesByte =50 * 1024 * 1024 ;

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

                    CheckDirectoryExistCreate(LogPath);

                    var LogFilePath = CheckFileExistCreate(LogPath, Type);

                    FIleWriteLine(arg, LogFilePath, Message);

                    Remove_TimeOutLogText();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}>>LogText Add_LogText Error:{ex.Message}");
            }
        }

        /// <summary>
        /// 判斷有無資料表 若沒有建立資料表
        /// </summary>
        /// <param name="LogPath"></param>
        private static void CheckDirectoryExistCreate(string LogPath)
        {            
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }

        /// <summary>
        /// 判斷有無檔案或檔案過大，若沒有或檔案過大則建立新檔案
        /// </summary>
        /// <param name="_LogPath">檔案路徑</param>
        /// <param name="_FileName">檔案名稱</param>
        private static string CheckFileExistCreate(string _LogPath , string _FileName)
        {
            var LogFIleName = $"{DateTime.Now.ToString("yyyyMMdd")}_{_FileName}_Log.txt";
            var SearchFIleName = $"{DateTime.Now.ToString("yyyyMMdd")}_{_FileName}*";
            FileExistCreate(_LogPath + LogFIleName);

            DirectoryInfo di = new DirectoryInfo(_LogPath);
            var Files = di.GetFiles(SearchFIleName).OrderBy(x=>x.LastWriteTimeUtc).ToArray();
            var NowWriteFile = Files[Files.Length -1];
            if(NowWriteFile.Length > BigFilesByte)
            {
                var FileNameSplits = NowWriteFile.Name.Replace("_"+_FileName,"").Split('_');
                if (!FileNameSplits[1].Contains("part"))
                {
                    LogFIleName = $"{DateTime.Now.ToString("yyyyMMdd")}_{_FileName}_part{1}_Log.txt";                    
                }
                else
                {
                    var Part = Convert.ToInt32(FileNameSplits[1].Replace("part",""));
                    LogFIleName = $"{DateTime.Now.ToString("yyyyMMdd")}_{_FileName}_part{Part + 1}_Log.txt";
                }
                FileExistCreate(_LogPath + LogFIleName);
            }
            else
            {
                LogFIleName = NowWriteFile.Name;
            }
            return _LogPath + LogFIleName;
        }

        /// <summary>
        /// 判斷有無檔案，若沒有則建立新檔案
        /// </summary>
        /// <param name="_LogFilePath"></param>
        private static void FileExistCreate(string _LogFilePath)
        {
            if (!File.Exists(_LogFilePath))
            {
                using (FileStream fileStream = new FileStream(_LogFilePath, FileMode.Create))
                {
                    fileStream.Close();
                }
            }
        }

        private static void FIleWriteLine(object[] arg , string _filePath ,string _Message)
        {
            using (StreamWriter sw = new StreamWriter(_filePath, true, Encoding.UTF8))
            {
                sw.WriteLine(_Message, arg);
                sw.Close();
            }
        }



        /// <summary>
        /// 刪除超時紀錄檔
        /// </summary>
        private static void Remove_TimeOutLogText()
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
