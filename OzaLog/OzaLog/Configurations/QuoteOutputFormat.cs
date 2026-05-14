namespace OzaLog
{
    /// <summary>
    /// 報價 pipeline 的輸出格式選項
    /// Output format for the independent Quote pipeline.
    /// </summary>
    /// <remarks>
    /// v3.1+ 引入:搭配 <c>QuoteOptions.OutputFormat</c> 使用。
    /// 報價 pipeline 獨立於主 logger,可設定不同格式(例如主 logger 用 Txt,報價用 Json)。
    /// </remarks>
    public enum QuoteOutputFormat
    {
        /// <summary>
        /// 人類可讀的 key=value 格式,副檔名 <c>.txt</c>(預設)
        /// 範例:<c>[2026-05-13 10:23:45.123] binance_spot BTCUSDT last=60123.5 bid=60123.0 ask=60124.0</c>
        /// </summary>
        Txt = 0,

        /// <summary>
        /// 人類可讀的 key=value 格式,副檔名 <c>.log</c>(內容與 Txt 完全相同)
        /// </summary>
        Log = 1,

        /// <summary>
        /// NDJSON 格式,副檔名 <c>.json</c>
        /// 範例:<c>{"ts":1715587425123,"symbol":"BTCUSDT","bucket":"binance_spot","last":60123.5,"extras":{"funding":0.0001}}</c>
        /// </summary>
        Json = 2,
    }
}
