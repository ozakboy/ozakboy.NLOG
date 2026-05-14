namespace OzaLog
{
    /// <summary>
    /// 主 logger 的輸出格式選項
    /// Output format for the main logger pipeline.
    /// </summary>
    /// <remarks>
    /// v3.1+ 引入：搭配 <c>LogOptions.OutputFormat</c> 全域設定使用。
    /// txt 與 log 內容完全相同,只差副檔名;json 是結構化 NDJSON。
    /// </remarks>
    public enum LogOutputFormat
    {
        /// <summary>
        /// 純文字格式,副檔名 <c>.txt</c>(預設)
        /// </summary>
        Txt = 0,

        /// <summary>
        /// 純文字格式,副檔名 <c>.log</c>(內容與 Txt 完全相同)
        /// </summary>
        Log = 1,

        /// <summary>
        /// NDJSON 格式(每行一個 JSON 物件),副檔名 <c>.json</c>
        /// Schema: <c>{"ts":epoch_ms,"lv":string,"nm":string,"tid":int?,"tn":string?,"msg":string,"data":object?}</c>
        /// </summary>
        Json = 2,
    }
}
