using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 日誌檔案處理類別，負責日誌檔案的建立、寫入和管理
    ///  支援單次寫入和批次寫入
    /// </summary>
    static class LogText
    {
        private static object lockMe = new object();

        /// <summary>
        /// 建立或是新增單條LOG紀錄
        /// </summary>
        internal static void Add_LogText(LogLevel level, string name, string message, object[] args)
        {
            var logItem = new LogItem
            {
                Level = level,
                Name = name,
                Message = message,
                Args = args
            };

            Add_BatchLogText(new[] { logItem });
        }


        /// <summary>
        /// 批次寫入多條日誌
        /// </summary>
        internal static void Add_BatchLogText(IEnumerable<LogItem> logItems)
        {
            try
            {
                lock (lockMe)
                {
                    // 按日誌級別和名稱分組
                    var groupedLogs = logItems.GroupBy(item => new { item.Level, item.Name });

                    foreach (var group in groupedLogs)
                    {
                        var level = group.Key.Level;
                        var name = string.IsNullOrEmpty(group.Key.Name) ? level.ToString() : group.Key.Name;

                        // 獲取日誌檔案資訊
                        var fileInfo = GetLogFileInfo(level, name);

                        // 批次寫入檔案
                        WriteLogsToFile(fileInfo, group);
                    }

                    // 清理過期日誌（只在批次處理完成後執行一次）
                    Remove_TimeOutLogText();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}>>LogText Add_BatchLogText Error:{ex.Message}");
            }
        }


        /// <summary>
        /// 獲取日誌檔案資訊
        /// </summary>
        private static LogFileInfo GetLogFileInfo(LogLevel level, string name)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logPath = Path.Combine(baseDir, LogConfiguration.Current.LogPath,
                                     DateTime.Now.ToString("yyyyMMdd"),
                                     LogConfiguration.Current.TypeDirectories.GetPathForType(level));

            var fileInfo = new LogFileInfo
            {
                DirectoryPath = logPath
            };

            // 確保目錄存在
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            // 確定檔案路徑和是否需要新檔案
            DetermineLogFile(fileInfo, name);

            return fileInfo;
        }

        /// <summary>
        /// 確定日誌檔案路徑和狀態
        /// </summary>
        private static void DetermineLogFile(LogFileInfo fileInfo, string name)
        {
            var logFileName = $"{name}_Log.txt";
            var searchPattern = $"{name}*";
            var di = new DirectoryInfo(fileInfo.DirectoryPath);
            var files = di.GetFiles(searchPattern).OrderBy(x => x.LastWriteTimeUtc).ToArray();

            if (files.Length == 0)
            {
                fileInfo.FilePath = Path.Combine(fileInfo.DirectoryPath, logFileName);
                fileInfo.RequiresNewFile = true;
                return;
            }

            var currentFile = files.Last();
            if (currentFile.Length > LogConfiguration.Current.MaxFileSize)
            {
                var splits = currentFile.Name.Replace("_" + name, "").Split('_');
                int nextPart = 1;

                if (splits.Length > 1 && splits[1].StartsWith("part"))
                {
                    nextPart = int.Parse(splits[1].Replace("part", "")) + 1;
                }

                logFileName = $"{name}_part{nextPart}_Log.txt";
                fileInfo.RequiresNewFile = true;
            }
            else
            {
                logFileName = currentFile.Name;
                fileInfo.RequiresNewFile = false;
            }

            fileInfo.FilePath = Path.Combine(fileInfo.DirectoryPath, logFileName);
        }


        /// <summary>
        /// 批次寫入日誌到檔案
        /// </summary>
        private static void WriteLogsToFile(LogFileInfo fileInfo, IEnumerable<LogItem> logs)
        {
            // 如果需要新檔案，先創建
            if (fileInfo.RequiresNewFile)
            {
                using (File.Create(fileInfo.FilePath)) { }
            }

            // 批次寫入所有日誌
            using (var writer = new StreamWriter(fileInfo.FilePath, true, Encoding.UTF8))
            {
                foreach (var log in logs)
                {
                    writer.WriteLine(string.Format(log.Message, log.Args));
                }
            }
        }

        /// <summary>
        /// 刪除超時紀錄檔
        /// </summary>
        private static void Remove_TimeOutLogText()
        {
            try
            {
                string logBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogConfiguration.Current.LogPath);      
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


        #region Class

        /// <summary>
        /// 日誌檔案資訊類
        /// </summary>
        private class LogFileInfo
        {
            public string DirectoryPath { get; set; }
            public string FilePath { get; set; }
            public bool RequiresNewFile { get; set; }
        }


        #endregion
    }
}
