```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8328)
AMD Ryzen 9 9950X3D, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]     : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FRZYLA : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=3  LaunchCount=1  WarmupCount=1  

```
| Method       | Mean        | Error       | StdDev    | Ratio | RatioSD | Gen0     | Gen1    | Allocated  | Alloc Ratio |
|------------- |------------:|------------:|----------:|------:|--------:|---------:|--------:|-----------:|------------:|
| Ozakboy_NLOG |  3,046.9 μs |  9,644.6 μs | 528.65 μs |  1.02 |    0.23 |  62.5000 | 54.6875 | 3941.93 KB |       1.000 |
| ZLogger_Lib  |  4,996.2 μs |  4,764.9 μs | 261.18 μs |  1.68 |    0.29 |        - |       - |    5.21 KB |       0.001 |
| ZeroLog_Lib  |    648.8 μs |    145.9 μs |   8.00 μs |  0.22 |    0.04 |   1.9531 |       - |    3.35 KB |       0.001 |
| Serilog_Lib  | 11,092.5 μs | 13,339.7 μs | 731.19 μs |  3.72 |    0.65 | 117.1875 |       - | 8273.76 KB |       2.099 |
