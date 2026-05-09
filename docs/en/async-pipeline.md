---
title: HFT Async Architecture
description: ConcurrentQueue + persistent FileStream pool + cached timestamp + drop-oldest backpressure.
---

# HFT Async Architecture

> TODO: write content.

## Overview

- `ConcurrentQueue<struct LogItem>` — zero-GC enqueue from caller threads
- Single dispatcher thread serializes writes per `(level, name)`
- `FileStreamPool` — persistent FileStreams with LRU eviction, no per-batch open/close
- `TimestampCache` — 1ms cached `DateTime` ticks
- Drop-oldest backpressure when queue full
