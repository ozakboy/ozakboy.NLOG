```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8328)
AMD Ryzen 9 9950X3D, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]     : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-GITXSF : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=3  LaunchCount=1  WarmupCount=1  

```
| Method       | Mean      | Error        | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------- |----------:|-------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Ozakboy_NLOG |  65.96 ns |    18.636 ns |  1.021 ns |  65.71 ns |  1.00 |    0.02 | 0.0019 |     151 B |        1.00 |
| ZLogger_Lib  | 219.53 ns | 1,012.521 ns | 55.500 ns | 189.21 ns |  3.33 |    0.73 |      - |     278 B |        1.84 |
| ZeroLog_Lib  |  12.19 ns |     1.953 ns |  0.107 ns |  12.16 ns |  0.18 |    0.00 |      - |         - |        0.00 |
| Serilog_Lib  | 168.87 ns | 1,461.984 ns | 80.136 ns | 214.84 ns |  2.56 |    1.05 | 0.0019 |     160 B |        1.06 |
