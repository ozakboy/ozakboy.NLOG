---
title: 快速開始
description: 安裝 OzaLog,一分鐘內寫下第一行 log。
---

# 快速開始

> TODO: 撰寫內容。本檔是 `/docs/getting-started` 的繁中正式來源。

## 安裝

```bash
dotnet add package OzaLog
```

## 第一行 log

```csharp
using OzaLog;

LOG.Info_Log("Hello, OzaLog!");
```

完成 —— 不需呼叫 `Configure`。
