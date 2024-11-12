using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌檔案處理類別，負責日誌檔案的建立、寫入和管理
    /// </summary>
    static class LogText
    {
        private static object lockMe = new object();

        /// <summary>
        /// 建立或是新增LOG紀錄
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Message"></param>
        /// <param name="arg"></param>
        internal static void Add_LogText(LogLevel level, string name, string Message, object[] arg)
        {
            try
            {
                // 使用 lock 避免多執行敘執行時 檔案被佔用問題
                lock (lockMe)
                {

                    string LogPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\{LogConfiguration.Current.LogPath}\\{DateTime.Now.ToString("yyyyMMdd")}\\{LogConfiguration.Current.TypeDirectories.GetPathForType(level)}\\";

                    CheckDirectoryExistCreate(LogPath);

                    if (string.IsNullOrEmpty(name)) name = level.ToString();

                    var LogFilePath = CheckFileExistCreate(LogPath, name);

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
        private static string CheckFileExistCreate(string _LogPath, string _FileName)
        {
            var LogFIleName = $"{_FileName}_Log.txt";
            var SearchFIleName = $"{_FileName}*";
            FileExistCreate(_LogPath + LogFIleName);

            DirectoryInfo di = new DirectoryInfo(_LogPath);
            var Files = di.GetFiles(SearchFIleName).OrderBy(x => x.LastWriteTimeUtc).ToArray();
            var NowWriteFile = Files[Files.Length - 1];
            if (NowWriteFile.Length > LogConfiguration.Current.MaxFileSize)
            {
                var FileNameSplits = NowWriteFile.Name.Replace("_" + _FileName, "").Split('_');
                if (!FileNameSplits[1].Contains("part"))
                {
                    LogFIleName = $"{_FileName}_part{1}_Log.txt";
                }
                else
                {
                    var Part = Convert.ToInt32(FileNameSplits[1].Replace("part", ""));
                    LogFIleName = $"{_FileName}_part{Part + 1}_Log.txt";
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

        private static void FIleWriteLine(object[] arg, string _filePath, string _Message)
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
            try
            {
                string logBasePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\{LogConfiguration.Current.LogPath}\\";
                // 確保目錄存在
                if (!Directory.Exists(logBasePath))
                    return;
                var directories = Directory.GetDirectories(logBasePath);
                int cutoffDate = int.Parse(DateTime.Now.AddDays(LogConfiguration.Current.KeepDays).ToString("yyyyMMdd"));
                foreach (string dir in directories)
                {
                    string folderName = Path.GetFileName(dir);

                    // 判斷是否為8位數的日期格式
                    if (folderName.Length == 8 && int.TryParse(folderName, out int folderDate))
                    {
                        // 數字直接比較，小於截止日期就刪除
                        if (folderDate < cutoffDate)
                        {                         
                            Directory.Delete(dir, true);                           
                        }
                    }
                }
            }
            catch 
            {

            }
        }
    }
}
