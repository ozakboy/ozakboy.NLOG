---
title: 效能對比
description: OzaLog 與 ZLogger / ZeroLog / Serilog 的 BenchmarkDotNet 實測對比。
---

# 效能對比

> TODO: 貼上最新 BenchmarkDotNet 輸出與圖表說明。

測試環境: AMD Ryzen 9 9950X3D、.NET 10.0.7、BenchmarkDotNet 0.14。

## S1 — 單筆短訊息

| 方法 | 平均時間 | 配置記憶體 |
|--------|------|-----------|
| **OzaLog** | **65.96 ns** | 151 B |
| ZLogger | 219.53 ns | 278 B |
| ZeroLog | 12.19 ns | 0 B |
| Serilog | 168.87 ns | 160 B |

## S3 — HFT 8 執行緒 × 50 商品 × 2000 筆 log

| 方法 | 平均時間 | 配置記憶體 |
|--------|------|-----------|
| **OzaLog** | **3,047 μs** | 3.94 MB |
| ZLogger | 4,996 μs | 5.21 KB |
| ZeroLog | 649 μs | 3.35 KB |
| Serilog | 11,092 μs | 8.27 MB |

→ 自己跑: `dotnet run -c Release --project OzaLog.Benchmarks`
