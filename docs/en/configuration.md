---
title: Configuration
description: All LogOptions exposed by LOG.Configure().
---

# Configuration

> TODO: write content.

```csharp
LOG.Configure(o =>
{
    o.KeepDays = -7;
    o.SetFileSizeInMB(50);
    o.EnableAsyncLogging = true;
    o.EnableConsoleOutput = true;
});
```
