---
title: 配置選項
description: LOG.Configure() 公開的所有 LogOptions 設定。
---

# 配置選項

> TODO: 撰寫內容。

```csharp
LOG.Configure(o =>
{
    o.KeepDays = -7;
    o.SetFileSizeInMB(50);
    o.EnableAsyncLogging = true;
    o.EnableConsoleOutput = true;
});
```
