---
title: Benchmarks
description: OzaLog vs ZLogger / ZeroLog / Serilog measured with BenchmarkDotNet.
---

# Benchmarks

> TODO: paste latest BenchmarkDotNet output and chart commentary.

Reference setup: AMD Ryzen 9 9950X3D, .NET 10.0.7, BenchmarkDotNet 0.14.

## S1 — Single short message

| Method | Mean | Allocated |
|--------|------|-----------|
| **OzaLog** | **65.96 ns** | 151 B |
| ZLogger | 219.53 ns | 278 B |
| ZeroLog | 12.19 ns | 0 B |
| Serilog | 168.87 ns | 160 B |

## S3 — HFT 8 thread × 50 products × 2000 logs

| Method | Mean | Allocated |
|--------|------|-----------|
| **OzaLog** | **3,047 μs** | 3.94 MB |
| ZLogger | 4,996 μs | 5.21 KB |
| ZeroLog | 649 μs | 3.35 KB |
| Serilog | 11,092 μs | 8.27 MB |

→ Run yourself: `dotnet run -c Release --project OzaLog.Benchmarks`
