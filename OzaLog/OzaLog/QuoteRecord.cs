using System;
using System.Collections.Generic;

namespace OzaLog
{
    /// <summary>
    /// 報價/盤口資料記錄。欄位命名對齊 Binance REST API 24hr Ticker schema。
    /// Quote/ticker record. Field naming aligned with Binance REST API 24hr Ticker schema.
    /// </summary>
    /// <remarks>
    /// v3.1+ 引入。透過 <c>LOG.Quote(in QuoteRecord)</c> 寫入獨立的報價 pipeline。
    ///
    /// 必填欄位:<see cref="Symbol"/>、<see cref="Bucket"/>、<see cref="Ticks"/>、<see cref="Last"/>
    /// 選填欄位:其餘 decimal? 全部允許 null,輸出時自動省略(NDJSON)或顯示為空白(尚未支援的格式)
    ///
    /// 自訂欄位:<see cref="Extras"/> 與 <see cref="ExtrasJson"/> 二擇一,同時設定將拋 ArgumentException
    /// </remarks>
    public readonly struct QuoteRecord
    {
        /// <summary>
        /// 商品代號(必填)。範例:<c>BTCUSDT</c>、<c>BTC-USDT-SWAP</c>
        /// 含特殊字元(<c>/ \ : * ? " &lt; &gt; |</c>)會在落檔時自動替換為 <c>-</c>。
        /// </summary>
        public readonly string Symbol;

        /// <summary>
        /// 分桶識別(必填),任意字串。範例:<c>binance_spot</c>、<c>okx_swap</c>
        /// 用於檔名前綴:<c>{Bucket}_{Symbol}_Quote.{ext}</c>,避免不同來源同名商品撞檔。
        /// </summary>
        public readonly string Bucket;

        /// <summary>
        /// 事件時間 ticks(必填,caller 傳入)。
        /// 這是「市場事件發生時刻」,不是「寫 log 時刻」 — 對 HFT 場景的時序分析至關重要。
        /// </summary>
        public readonly long Ticks;

        /// <summary>
        /// 最後成交價(必填)。
        /// </summary>
        public readonly decimal Last;

        /// <summary>最後一筆成交數量(對應 Binance lastQty)。選填。</summary>
        public readonly decimal? LastQty;

        /// <summary>最佳買價(對應 Binance bidPrice)。選填。</summary>
        public readonly decimal? Bid;

        /// <summary>最佳買單數量(對應 Binance bidQty)。選填。</summary>
        public readonly decimal? BidQty;

        /// <summary>最佳賣價(對應 Binance askPrice)。選填。</summary>
        public readonly decimal? Ask;

        /// <summary>最佳賣單數量(對應 Binance askQty)。選填。</summary>
        public readonly decimal? AskQty;

        /// <summary>開盤價。選填。</summary>
        public readonly decimal? Open;

        /// <summary>昨收價。選填。</summary>
        public readonly decimal? PrevClose;

        /// <summary>區間最高價。選填。</summary>
        public readonly decimal? High;

        /// <summary>區間最低價。選填。</summary>
        public readonly decimal? Low;

        /// <summary>
        /// 累積成交量(對應 Binance volume,base asset)。選填。
        /// 區間語意由 caller 決定(24h / 盤中 / tick) — logger 不驗證。
        /// </summary>
        public readonly decimal? Volume;

        /// <summary>
        /// 累積計價成交額(對應 Binance quoteVolume,quote asset)。選填。
        /// </summary>
        public readonly decimal? QuoteVolume;

        /// <summary>
        /// 自訂欄位字典(慢路徑,反射友善)。與 <see cref="ExtrasJson"/> 二擇一,同時設定拋例外。
        /// 字典 key 若撞名內建欄位(symbol/bucket/last 等)會拋例外。
        /// </summary>
        public readonly IReadOnlyDictionary<string, object> Extras;

        /// <summary>
        /// 自訂欄位 JSON 字串(快路徑,使用者已自序列化)。與 <see cref="Extras"/> 二擇一。
        /// 應為合法的 JSON 物件字串,例如 <c>{"funding":0.0001}</c>。
        /// </summary>
        public readonly string ExtrasJson;

        /// <summary>
        /// 主要建構子。建議優先使用 <c>LOG.Quote(...)</c> 強型別便利多載。
        /// </summary>
        public QuoteRecord(
            string symbol,
            string bucket,
            long ticks,
            decimal last,
            decimal? lastQty = null,
            decimal? bid = null,
            decimal? bidQty = null,
            decimal? ask = null,
            decimal? askQty = null,
            decimal? open = null,
            decimal? prevClose = null,
            decimal? high = null,
            decimal? low = null,
            decimal? volume = null,
            decimal? quoteVolume = null,
            IReadOnlyDictionary<string, object> extras = null,
            string extrasJson = null)
        {
            Symbol = symbol;
            Bucket = bucket;
            Ticks = ticks;
            Last = last;
            LastQty = lastQty;
            Bid = bid;
            BidQty = bidQty;
            Ask = ask;
            AskQty = askQty;
            Open = open;
            PrevClose = prevClose;
            High = high;
            Low = low;
            Volume = volume;
            QuoteVolume = quoteVolume;
            Extras = extras;
            ExtrasJson = extrasJson;
        }
    }
}
