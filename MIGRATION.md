# Migration Guide — `Ozakboy.NLOG` v2.x → `OzaLog` v3.0

> **Why the rename?** `Ozakboy.NLOG` and the well-known [`NLog`](https://www.nuget.org/packages/NLog) package have nothing to do with each other. The naming similarity caused user confusion. Starting v3.0 the package is renamed to **`OzaLog`** to avoid this and to mark a clean break for the v3.0 architectural rewrite.

> **`Ozakboy.NLOG` v2.1.0 is now deprecated** and will not receive further updates. All future development happens on `OzaLog`.

---

## 1. Update the NuGet PackageReference

In your `.csproj`:

```diff
- <PackageReference Include="Ozakboy.NLOG" Version="2.1.0" />
+ <PackageReference Include="OzaLog" Version="3.0.0" />
```

Or via CLI:

```bash
dotnet remove package Ozakboy.NLOG
dotnet add package OzaLog
```

## 2. Update `using` statements

Across your codebase replace:

```diff
- using ozakboy.NLOG;
+ using OzaLog;
```

```diff
- using ozakboy.NLOG.Core;
+ using OzaLog.Core;
```

> Tip: a single project-wide find-and-replace handles this. The two patterns above cover all references.

## 3. The API surface is unchanged

You do **not** need to change any `LOG.*_Log(...)` calls. All public method signatures are preserved:

```csharp
LOG.Trace_Log("...");
LOG.Debug_Log("...");
LOG.Info_Log("...");
LOG.Warn_Log("...");
LOG.Error_Log("...");
LOG.Fatal_Log("...");
LOG.CustomName_Log("BTC", "...");
LOG.Configure(o => { ... });
```

## 4. Breaking changes (read carefully)

### 4.1 Dropped TargetFrameworks

OzaLog v3.0 supports:
- ✅ `netstandard2.0` / `netstandard2.1`
- ✅ `net8.0` / `net9.0` / `net10.0`

OzaLog v3.0 **drops**:
- ❌ `.NET Framework 4.6.2` — use Ozakboy.NLOG v2.1.0 if you need .NET Framework
- ❌ `net6.0` (Microsoft EOL Nov 2024)
- ❌ `net7.0` (Microsoft EOL May 2024)

If you're still on .NET 6/7, upgrade to net8 LTS first.

### 4.2 `LogLevel.CostomName` → `LogLevel.CustomName` (typo fix)

The enum value name was a typo. Fixed in v3.0:

```diff
- LogLevel.CostomName
+ LogLevel.CustomName
```

The numeric value (`99`) is unchanged so wire-format / serialized representations still match.

> 99% of users never touch this enum directly (the public API is `LOG.CustomName_Log(...)`, with the correct spelling). If you do reference the enum value in code, update it.

### 4.3 New `LogOptions` properties (additive — not breaking)

You can ignore these unless you want to use them:

```csharp
LOG.Configure(o =>
{
    // ... existing options ...

    // NEW in v3.0
    o.EnableGlobalExceptionCapture = false;   // default false; opt-in for AppDomain handler
    o.MaxOpenFileStreams = 100;               // LRU upper bound (default 100)
    o.DiskFlushIntervalMs = 100;              // disk flush interval (default 100ms)
    o.OnDropped = () => { /* counter */ };    // backpressure callback
});
```

### 4.4 Internal architecture rewrite (transparent to you)

The internals were rewritten for HFT-grade throughput:
- `LogItem` is now a `readonly struct` (zero GC pressure)
- Persistent `FileStream` pool with LRU eviction
- Cached timestamp updated every 1 ms (avoids `DateTime.Now` syscall)
- Drop-oldest backpressure when queue is full (was: degrades to synchronous write)

If you previously relied on the implementation detail that "queue full → caller blocks on sync write", be aware that the new behavior is "queue full → drop oldest log and keep going". Use `OnDropped` callback if you need to track drops.

### 4.5 `Console.WriteLine` double-format bug fix

In v2.x, `LOG.Info_Log("msg with {0}", args)` could produce `FormatException` if console output was enabled and the formatted message contained additional `{N}` placeholders. Fixed in v3.0.

## 5. Files + folders changed (informational)

If you cloned the source repo:

| v2.x | v3.0 |
|------|------|
| `ozakboy.NLOG/ozakboy.NLOG/ozakboy.NLOG.sln` | `OzaLog/OzaLog.sln` |
| `ozakboy.NLOG/ozakboy.NLOG/` (solution folder) | `OzaLog/` |
| `ozakboy.NLOG/ozakboy.NLOG/ozakboy.NLOG/` (project folder) | `OzaLog/OzaLog/` |
| `ozakboy.NLOG.csproj` | `OzaLog.csproj` |
| `NLOG.cs` (filename) | `LOG.cs` |

## 6. If you cannot migrate now

`Ozakboy.NLOG` v2.1.0 will remain published on NuGet (with a deprecation marker pointing to OzaLog), so existing projects continue to install and run. **No updates will be made to v2.x — including no security patches**. If you need a fix, the only path forward is OzaLog v3.0+.

The git tag `v2-frozen` marks the final v2.x state in the repository.

## 7. Need help?

Open an issue: https://github.com/ozakboy/OzaLog/issues
