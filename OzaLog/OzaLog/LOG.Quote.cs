using System;
using System.Collections.Generic;
using OzaLog.Core;

namespace OzaLog
{
    /// <summary>
    /// LOG 公開 API 之報價(Quote)pipeline 部分。
    /// </summary>
    /// <remarks>
    /// v3.1+ 引入。獨立於主 logger 的非同步寫入管道,專為高頻報價/盤口資料設計。
    ///
    /// 使用前提:必須在 <c>LOG.Configure(...)</c> 時透過
    /// <c>opt.ConfigureQuote(q =&gt; q.Enable = true)</c> 啟用(預設 false)。
    /// 未啟用時呼叫 <c>LOG.Quote(...)</c> 視為 no-op,不會啟動背景執行緒。
    ///
    /// 寫入位置:<c>{baseDir}/{LogPath}/{yyyyMMdd}/{QuotePath}/{Bucket}_{Symbol}_Quote.{ext}</c>
    /// </remarks>
    public static partial class LOG
    {
        // 內建欄位名(撞名檢查用) — 與 QuoteFormatter._reservedKeys 同步
        private static readonly HashSet<string> _quoteReservedKeys = new HashSet<string>(StringComparer.Ordinal)
        {
            "ts", "symbol", "bucket", "last", "lastQty",
            "bid", "bidQty", "ask", "askQty",
            "open", "prevClose", "high", "low",
            "volume", "quoteVolume", "extras",
        };

        /// <summary>
        /// 呼叫端驗證:拋例外讓使用者立即發現 programmer error,而不是延遲到 dispatcher 才靜默失敗。
        /// </summary>
        private static void ValidateQuoteRecord(in QuoteRecord record)
        {
            if (string.IsNullOrEmpty(record.Symbol))
                throw new ArgumentException("QuoteRecord.Symbol 不可為 null 或空字串", nameof(record));
            if (string.IsNullOrEmpty(record.Bucket))
                throw new ArgumentException("QuoteRecord.Bucket 不可為 null 或空字串", nameof(record));
            if (record.Extras != null && !string.IsNullOrEmpty(record.ExtrasJson))
                throw new ArgumentException(
                    "QuoteRecord.Extras 與 QuoteRecord.ExtrasJson 不可同時設定(請擇一)",
                    nameof(record));
            if (record.Extras != null)
            {
                foreach (var kv in record.Extras)
                {
                    if (_quoteReservedKeys.Contains(kv.Key))
                        throw new ArgumentException(
                            $"QuoteRecord.Extras key '{kv.Key}' 撞名內建欄位,請改用其他名稱",
                            nameof(record));
                }
            }
        }

        #region Quote API - A2(底層核心,struct 多載)

        /// <summary>
        /// 寫入報價記錄(核心 API)。
        /// </summary>
        /// <param name="record">完整的 <see cref="QuoteRecord"/>,包含必填欄位與選填欄位</param>
        /// <remarks>
        /// 必填:<c>Symbol</c>、<c>Bucket</c>、<c>Ticks</c>、<c>Last</c>
        /// 違反必填、Extras/ExtrasJson 同時設定、Extras key 撞名 → 立即拋 <see cref="System.ArgumentException"/>(同步,呼叫端可 try/catch)
        /// </remarks>
        public static void Quote(in QuoteRecord record)
        {
            ValidateQuoteRecord(in record);
            QuoteLogHandler.Enqueue(in record);
        }

        #endregion

        #region Quote API - A1(便利多載,內部建構 QuoteRecord 後委派)

        /// <summary>
        /// 寫入最簡報價記錄(僅 last 價格,例如 tick 資料)
        /// </summary>
        public static void Quote(string symbol, string bucket, long ticks, decimal last)
        {
            var rec = new QuoteRecord(symbol, bucket, ticks, last);
            Quote(in rec);
        }

        /// <summary>
        /// 寫入含 bid/ask 的報價記錄
        /// </summary>
        public static void Quote(string symbol, string bucket, long ticks,
            decimal last, decimal bid, decimal ask)
        {
            var rec = new QuoteRecord(symbol, bucket, ticks, last,
                bid: bid, ask: ask);
            Quote(in rec);
        }

        /// <summary>
        /// 寫入含 bid/ask + 買賣量的報價記錄
        /// </summary>
        public static void Quote(string symbol, string bucket, long ticks,
            decimal last,
            decimal bid, decimal bidQty,
            decimal ask, decimal askQty)
        {
            var rec = new QuoteRecord(symbol, bucket, ticks, last,
                bid: bid, bidQty: bidQty,
                ask: ask, askQty: askQty);
            Quote(in rec);
        }

        /// <summary>
        /// 寫入完整 ticker(對齊 Binance REST API 24hr Ticker)。
        /// 所有欄位都可為 null,只有 symbol/bucket/ticks/last 是必填。
        /// </summary>
        public static void QuoteTicker(string symbol, string bucket, long ticks,
            decimal last,
            decimal? lastQty = null,
            decimal? bid = null, decimal? bidQty = null,
            decimal? ask = null, decimal? askQty = null,
            decimal? open = null, decimal? prevClose = null,
            decimal? high = null, decimal? low = null,
            decimal? volume = null, decimal? quoteVolume = null)
        {
            var rec = new QuoteRecord(symbol, bucket, ticks, last,
                lastQty: lastQty,
                bid: bid, bidQty: bidQty,
                ask: ask, askQty: askQty,
                open: open, prevClose: prevClose,
                high: high, low: low,
                volume: volume, quoteVolume: quoteVolume);
            Quote(in rec);
        }

        /// <summary>
        /// 寫入完整 ticker + 自訂欄位字典(Extras)。
        /// </summary>
        public static void QuoteTicker(string symbol, string bucket, long ticks,
            decimal last,
            IReadOnlyDictionary<string, object> extras,
            decimal? lastQty = null,
            decimal? bid = null, decimal? bidQty = null,
            decimal? ask = null, decimal? askQty = null,
            decimal? open = null, decimal? prevClose = null,
            decimal? high = null, decimal? low = null,
            decimal? volume = null, decimal? quoteVolume = null)
        {
            var rec = new QuoteRecord(symbol, bucket, ticks, last,
                lastQty: lastQty,
                bid: bid, bidQty: bidQty,
                ask: ask, askQty: askQty,
                open: open, prevClose: prevClose,
                high: high, low: low,
                volume: volume, quoteVolume: quoteVolume,
                extras: extras);
            Quote(in rec);
        }

        #endregion
    }
}
