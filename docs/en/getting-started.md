---
title: Getting Started
description: Install OzaLog and write your first log line in under a minute.
---

# Getting Started

> TODO: write content. This file is the canonical English source for `/docs/getting-started`.

## Install

```bash
dotnet add package OzaLog
```

## First log

```csharp
using OzaLog;

LOG.Info_Log("Hello, OzaLog!");
```

That's it — no `Configure` call required.
