---
title: HFT 異步架構
description: ConcurrentQueue + 持久化 FileStream 池 + 快取時間戳 + drop-oldest 背壓。
---

# HFT 異步架構

> TODO: 撰寫內容。

## 總覽

- `ConcurrentQueue<struct LogItem>` — 呼叫端入隊零 GC
- 單一 dispatcher 執行緒序列化每個 `(level, name)` 的寫入
- `FileStreamPool` — 持久化 FileStream + LRU 淘汰,免每批次開關
- `TimestampCache` — 1ms 快取 `DateTime` ticks
- 隊列滿時 drop-oldest 背壓
