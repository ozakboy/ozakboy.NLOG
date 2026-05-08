using System;
using System.IO;
using System.Linq;
using System.Threading;
using OzaLog;
using Xunit;

namespace OzaLog.Tests
{
    /// <summary>
    /// 整合測試 - 透過公開 API LOG.* 寫入後檢查 logs/ 目錄是否生成預期檔案內容。
    /// </summary>
    /// <remarks>
    /// LOG.Configure 全域只能呼叫一次（v3.0 維持不可重入）。
    /// 為避免各測試彼此影響，使用 ThreadStatic 不可行（Configure 影響全域 LogConfiguration）。
    /// 這裡只跑「不需特定設定」即可驗證的最小路徑：寫入 → 等待 dispatcher → 檢查檔案。
    /// </remarks>
    public class BasicLoggingTests
    {
        [Fact]
        public void Log_WritesToFileWithinReasonableTime()
        {
            var marker = "smoke_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            LOG.Info_Log("integration smoke test " + marker);

            // 等待 dispatcher（FlushIntervalMs 預設 1000ms）+ disk flush（100ms）
            Thread.Sleep(1500);

            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                LOG.GetCurrentOptions().LogPath);
            Assert.True(Directory.Exists(baseDir), $"log base dir should exist: {baseDir}");

            // 任何當天的 yyyyMMdd 子目錄
            var dateDirs = Directory.GetDirectories(baseDir);
            Assert.NotEmpty(dateDirs);

            // 任何 _Log.txt 包含 marker
            var allFiles = dateDirs.SelectMany(d => Directory.GetFiles(d, "*.txt", SearchOption.AllDirectories));
            bool found = false;
            foreach (var f in allFiles)
            {
                try
                {
                    using var fs = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var sr = new StreamReader(fs);
                    var content = sr.ReadToEnd();
                    if (content.Contains(marker))
                    {
                        found = true;
                        break;
                    }
                }
                catch
                {
                    // 檔案被 dispatcher 持有 → 略過繼續找
                }
            }

            Assert.True(found, $"marker {marker} should appear in some log file");
        }
    }
}
