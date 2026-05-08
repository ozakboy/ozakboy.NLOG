using BenchmarkDotNet.Running;

// BenchmarkSwitcher 允許 CLI 過濾，例如：
//   dotnet run -c Release --project OzaLog.Benchmarks -- --filter '*S1*'
//   dotnet run -c Release --project OzaLog.Benchmarks -- --filter '*S3*'
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
