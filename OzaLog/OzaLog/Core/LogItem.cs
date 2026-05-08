using System;

namespace OzaLog.Core
{
    /// <summary>
    /// 日誌項目 - 在 producer / consumer 之間傳遞日誌資訊。
    /// Log Item - Carries log information between producer and consumer threads.
    /// </summary>
    /// <remarks>
    /// v3.0 改為 readonly struct，避免高頻 log 時每筆 heap 配置造成 GC 壓力。
    /// 欄位都採 raw 形式（未格式化），格式化動作延遲到 dispatcher 執行緒處理，
    /// 讓呼叫端執行緒以最低成本完成入隊。
    /// </remarks>
    internal readonly struct LogItem
    {
        /// <summary>日誌級別</summary>
        public readonly LogLevel Level;

        /// <summary>日誌名稱（CustomName 用，其他層級為空字串）</summary>
        public readonly string Name;

        /// <summary>原始訊息（尚未做 string.Format）</summary>
        public readonly string Message;

        /// <summary>格式化參數，未使用時為 null（避免配置空陣列）</summary>
        public readonly object[] Args;

        /// <summary>入隊瞬間的本地時間 ticks（由 TimestampCache 提供，不打 syscall）</summary>
        public readonly long TimestampTicks;

        /// <summary>入隊瞬間的執行緒 ID</summary>
        public readonly int ThreadId;

        /// <summary>是否需要立即寫入並 flush 落盤</summary>
        public readonly bool RequireImmediateFlush;

        public LogItem(
            LogLevel level,
            string name,
            string message,
            object[] args,
            long timestampTicks,
            int threadId,
            bool requireImmediateFlush)
        {
            Level = level;
            Name = name;
            Message = message;
            Args = args;
            TimestampTicks = timestampTicks;
            ThreadId = threadId;
            RequireImmediateFlush = requireImmediateFlush;
        }
    }
}
