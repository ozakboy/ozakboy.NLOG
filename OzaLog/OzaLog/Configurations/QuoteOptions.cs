using System;

namespace OzaLog
{
    /// <summary>
    /// 報價 pipeline 的設定選項。透過 <see cref="LogConfiguration.LogOptions.ConfigureQuote(Action{QuoteOptions})"/> 設定。
    /// Options for the independent Quote pipeline.
    /// </summary>
    /// <remarks>
    /// v3.1+ 引入:報價 pipeline 預設關閉(<c>Enable = false</c>),使用者需 opt-in。
    /// 啟用後會建立獨立的 dispatcher、佇列與 FileStreamPool,不與主 logger 互相影響。
    /// </remarks>
    public class QuoteOptions
    {
        /// <summary>
        /// 是否啟用報價 pipeline。預設 <c>false</c>(opt-in)。
        /// 設為 true 後第一次呼叫 <c>LOG.Quote(...)</c> 會啟動 dispatcher。
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// 報價檔案的輸出格式。預設 <see cref="QuoteOutputFormat.Txt"/>。
        /// </summary>
        public QuoteOutputFormat OutputFormat { get; set; } = QuoteOutputFormat.Txt;

        /// <summary>
        /// 報價檔案的根目錄(會接在 <see cref="LogConfiguration.LogOptions.LogPath"/> 與日期目錄之下)。
        /// 預設 <c>"Quotes"</c>。最終路徑形如:<c>{baseDir}/{LogPath}/{yyyyMMdd}/{QuotePath}/{bucket}_{symbol}_Quote.{ext}</c>
        /// </summary>
        public string QuotePath { get; set; } = "Quotes";

        private int _maxOpenStreams = 500;
        /// <summary>
        /// 報價 FileStreamPool 同時保持開啟的檔案上限(獨立於主 logger 的 <c>MaxOpenFileStreams</c>)。
        /// 預設 500(因為報價 symbol 量大);範圍 [4, 4096]。
        /// </summary>
        public int MaxOpenStreams
        {
            get => _maxOpenStreams;
            set => _maxOpenStreams = Math.Max(4, Math.Min(4096, value));
        }

        private int _maxQueueSize = 50_000;
        /// <summary>
        /// 報價佇列容量。預設 50000(因為報價量遠大於普通 log);範圍 [1000, 1_000_000]。
        /// 佇列滿時走 drop oldest + <see cref="OnDropped"/> callback。
        /// </summary>
        public int MaxQueueSize
        {
            get => _maxQueueSize;
            set => _maxQueueSize = Math.Max(1000, Math.Min(1_000_000, value));
        }

        private int _flushIntervalMs = 100;
        /// <summary>
        /// 報價 dispatcher 的批次 flush 間隔(ms)。預設 100ms;範圍 [10, 10000]。
        /// </summary>
        public int FlushIntervalMs
        {
            get => _flushIntervalMs;
            set => _flushIntervalMs = Math.Max(10, Math.Min(10_000, value));
        }

        private int _maxBatchSize = 500;
        /// <summary>
        /// 報價 dispatcher 單次處理的最大筆數。預設 500;範圍 [1, 10000]。
        /// </summary>
        public int MaxBatchSize
        {
            get => _maxBatchSize;
            set => _maxBatchSize = Math.Max(1, Math.Min(10_000, value));
        }

        /// <summary>
        /// 報價佇列滿時 drop oldest 後觸發的 callback。
        /// 參數為「自上次回呼以來新丟棄的累計筆數」;callback body 請保持極輕量(會在生產者執行緒呼叫)。
        /// 預設 null(靜默丟棄)。
        /// </summary>
        public Action<long> OnDropped { get; set; }
    }

    /// <summary>
    /// 報價 pipeline 設定的唯讀視圖介面
    /// Read-only view of Quote options.
    /// </summary>
    public interface IQuoteOptions
    {
        /// <summary>是否啟用報價 pipeline</summary>
        bool Enable { get; }

        /// <summary>報價輸出格式</summary>
        QuoteOutputFormat OutputFormat { get; }

        /// <summary>報價根目錄(預設 "Quotes")</summary>
        string QuotePath { get; }

        /// <summary>同時開啟的 FileStream 上限</summary>
        int MaxOpenStreams { get; }

        /// <summary>報價佇列容量</summary>
        int MaxQueueSize { get; }

        /// <summary>dispatcher 批次 flush 間隔(ms)</summary>
        int FlushIntervalMs { get; }

        /// <summary>dispatcher 單次處理的最大筆數</summary>
        int MaxBatchSize { get; }

        /// <summary>drop oldest callback</summary>
        Action<long> OnDropped { get; }
    }

    internal sealed class ReadOnlyQuoteOptions : IQuoteOptions
    {
        private readonly QuoteOptions _options;

        public ReadOnlyQuoteOptions(QuoteOptions options)
        {
            _options = options;
        }

        public bool Enable => _options.Enable;
        public QuoteOutputFormat OutputFormat => _options.OutputFormat;
        public string QuotePath => _options.QuotePath;
        public int MaxOpenStreams => _options.MaxOpenStreams;
        public int MaxQueueSize => _options.MaxQueueSize;
        public int FlushIntervalMs => _options.FlushIntervalMs;
        public int MaxBatchSize => _options.MaxBatchSize;
        public Action<long> OnDropped => _options.OnDropped;
    }
}
