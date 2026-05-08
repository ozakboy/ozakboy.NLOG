using System;
using System.IO;
using System.Threading;

namespace ozakboy.NLOG.Core
{
    /// <summary>
    /// 過期 log 清理器 - 由背景 Timer 每 60 秒掃一次，刪除超過 KeepDays 的日期目錄。
    /// 從 v2.x 的 hot path（每筆寫入後執行）移出，避免阻塞 dispatcher。
    /// Background log retention cleaner; runs every 60s instead of on every batch write.
    /// </summary>
    internal static class LogRetentionCleaner
    {
        private const int IntervalMs = 60_000;
        private static Timer _timer;
        private static int _initialized;

        public static void EnsureStarted()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
                return;

            _timer = new Timer(static _ => RunOnce(), null, dueTime: 5_000, period: IntervalMs);
        }

        internal static void Shutdown()
        {
            var t = Interlocked.Exchange(ref _timer, null);
            t?.Dispose();
            Interlocked.Exchange(ref _initialized, 0);
        }

        private static void RunOnce()
        {
            try
            {
                var current = LogConfiguration.Current;
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, current.LogPath);
                if (!Directory.Exists(basePath)) return;

                var cutoff = DateTime.Now.AddDays(current.KeepDays);
                var cutoffInt = cutoff.Year * 10000 + cutoff.Month * 100 + cutoff.Day;

                foreach (var dir in Directory.GetDirectories(basePath))
                {
                    var folderName = Path.GetFileName(dir);
                    if (folderName == null || folderName.Length != 8) continue;
                    if (!int.TryParse(folderName, out var folderDate)) continue;
                    if (folderDate < cutoffInt)
                    {
                        try { Directory.Delete(dir, recursive: true); }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"LogRetentionCleaner 刪除 {dir} 失敗: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogRetentionCleaner.RunOnce 錯誤: {ex.Message}");
            }
        }
    }
}
