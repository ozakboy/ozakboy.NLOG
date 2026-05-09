---
title: Changelog
description: All notable changes to OzaLog.
---

# Changelog

This file tracks all notable changes to the **OzaLog** package (formerly **Ozakboy.NLOG**).
Version numbers follow [Semantic Versioning](https://semver.org/).

---

## [3.0.1] - 2026-05-09

> Metadata + repository improvements release. **No library code changes** — the OzaLog assembly is byte-identical to v3.0.0 (Deterministic build).

### Improved
- **NuGet package metadata refreshed**: cleaner `Description` (highlights `LOG.Info_Log("...")` API + HFT pipeline + zero dependencies + crypto tick stream use case), updated `PackageTags` (added `ozalog`, `hft`, `high-performance`, `zero-dependency`; removed misleading `nlog` tag), polished `Title`.
- `PackageReleaseNotes` now uses absolute GitHub URLs for cross-references (NuGet doesn't render relative paths).

### Technical
- **New project website**: Nuxt 4 + @nuxt/content + Tailwind CSS, deployed to GitHub Pages → <https://ozakboy.github.io/OzaLog/>
- **Repository documentation restructured**: all user-facing docs moved to `docs/{en,zh-TW}/` bilingual tree (`changelog.md`, `migration.md`, plus templates for `getting-started.md`, `configuration.md`, `api.md`, `async-pipeline.md`, `benchmarks.md`).
- GitHub Actions auto-deploys the site on push to main.
- Sponsor page added with USDT (BEP20) wallet + Binance Pay QR.
- `uplog` release flow extended: now also creates GitHub Release and pushes to NuGet.org automatically.

### Notes
- For migration from v2.x see [migration guide](./migration.md).

---

## [3.0.0] - 2026-05-09

### Breaking Changes
- **Package renamed**: `Ozakboy.NLOG` → `OzaLog`. The previous package on NuGet is deprecated and points here. See [migration guide](./migration.md) for the upgrade guide.
- **Namespace renamed**: `ozakboy.LOG` → `OzaLog`. All `using` statements in consumer code must be updated.
- **Removed TargetFrameworks**: dropped `.NET Framework 4.6.2`, `net6.0`, `net7.0` (all EOL). Now supports `netstandard2.0`, `netstandard2.1`, `net8.0`, `net9.0`, `net10.0`.
- **Enum typo fixed**: `LogLevel.CostomName` → `LogLevel.CustomName`. The public method `LOG.CustomName_Log(...)` was already correctly named — only the underlying enum value was renamed.

### Added
- HFT-grade async pipeline: `ConcurrentQueue<struct LogItem>` + persistent FileStream pool + 1ms cached timestamp + drop-oldest backpressure.
- `LogOptions.EnableGlobalExceptionCapture` (default `false`) — opt-in subscription to `AppDomain.UnhandledException` and `TaskScheduler.UnobservedTaskException`, auto-logs at Fatal level with synchronous immediate flush.
- `LogOptions.MaxOpenFileStreams` (default `100`) — LRU upper bound; exceeding closes the least recently written stream.
- `LogOptions.DiskFlushIntervalMs` (default `100`) — periodic `Flush()` interval for persistent FileStreams.
- `LogOptions.OnDropped` (default `null`) — callback invoked when the async queue drops the oldest item under backpressure.
- `OzaLog.Tests/` — xUnit test project covering concurrency, LRU, day rollover, backpressure, GlobalExceptionCapture toggle, and format correctness.
- `OzaLog.Benchmarks/` — BenchmarkDotNet project comparing OzaLog with ZLogger, ZeroLog, and Serilog.
- `MIGRATION.md` — upgrade guide from `Ozakboy.NLOG` v2.x.

### Improved
- Zero NuGet dependencies on `net8.0` / `net9.0` / `net10.0` (System.Text.Json is built into the BCL on these targets).
- Formatting work moved off the calling thread — callers only enqueue the raw tuple `(level, name, message, args, ticks, threadId)`; no `string.Format` on the hot path.
- Persistent FileStreams eliminate per-batch open/close, reducing syscall cost to near-zero.
- Day rollover handled inline in the dispatcher (compares cached ticks date vs. stream date).
- Expired-log cleanup moved to a background timer (was on the hot path in v2.x).

### Fixed
- Double-format bug in `LOG.cs` where `Console.WriteLine(formattedMessage, args)` could throw `FormatException` if the formatted message coincidentally contained `{0}`-style tokens.
- Auto-flush level selection — `Error` and `Fatal` now correctly trigger immediate flush regardless of the caller's `immediateFlush` argument.

### Technical
- `LogItem` changed from class to `readonly struct` — zero GC on the hot path.
- New `Core/TimestampCache.cs` — background timer updates `volatile long _currentTicks` every 1ms; callers only read.
- New `Core/FileStreamPool.cs` — persistent FileStreams keyed by `(level, name)` with LRU eviction.
- New `Core/LogRetentionCleaner.cs` — background expired-log cleanup, off the hot path.
- New `Core/GlobalExceptionCapture.cs` — opt-in global exception subscription.
- Build verified across all 5 TargetFrameworks with 0 warnings / 0 errors.

---

## [2.1.0] - 2024

### Added
- Added support for .NET 8.0
- Introduced async logging with configurable batch processing
- Added customizable directory structure for different log levels
- Added support for custom log types (`CustomName_Log`)
- Added console output support

### Improved
- Enhanced file management with automatic log rotation
- Enhanced exception handling and serialization
- Improved configuration system with more options
- Better handling of file paths across operating systems

### Technical
- Improved thread safety and performance
- Implemented intelligent file size management

---

> History prior to 2.1.0 is not fully tracked here. See git history and the NuGet package page for details.
