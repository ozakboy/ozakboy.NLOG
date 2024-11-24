# Ozakboy.NLOG

[![nuget](https://img.shields.io/badge/nuget-ozakboy.NLOG-blue)](https://www.nuget.org/packages/Ozakboy.NLOG/) 
[![github](https://img.shields.io/badge/github-ozakboy.NLOG-blue)](https://github.com/ozakboy/ozakboy.NLOG/)

[English](README.md) | [繁體中文](README_zh-TW.md) 

A lightweight and high-performance logging tool that provides asynchronous writing, intelligent file management, and rich configuration options. A local logging solution designed specifically for .NET applications.

## Supported Frameworks

- .NET Framework 4.6.2
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET Standard 2.0/2.1

## Key Features

### Core Features
- 📝 Automatic log file and directory structure creation
- 🔄 Support for asynchronous log writing to enhance application performance
- ⚡ Smart batch processing and queue management
- 🔍 Detailed exception information logging and serialization
- 📊 Multi-level logging support
- 🛡️ Thread-safe design

### Advanced Features
- ⚙️ Flexible configuration system
- 📂 Custom log directory structure
- 🔄 Automatic file splitting and management
- ⏰ Configurable log retention period
- 💾 Smart file size management
- 🎯 Support for custom log types
- 🖥️ Optional console output

## Installation

Via NuGet Package Manager:
```bash
Install-Package Ozakboy.NLOG
```

Or using .NET CLI:
```bash
dotnet add package Ozakboy.NLOG
```

## Quick Start

### Basic Configuration
```csharp
LOG.Configure(options => {
    options.KeepDays = -7;                    // Keep logs for the last 7 days
    options.SetFileSizeInMB(50);              // Set single file size limit to 50MB
    options.EnableAsyncLogging = true;         // Enable asynchronous writing
    options.EnableConsoleOutput = true;        // Enable console output
    
    // Configure async options
    options.ConfigureAsync(async => {
        async.MaxBatchSize = 100;              // Process up to 100 logs per batch
        async.MaxQueueSize = 10000;            // Maximum queue capacity
        async.FlushIntervalMs = 1000;          // Write once per second
    });
});
```

### Basic Usage

```csharp
// Log different levels
LOG.Trace_Log("Detailed trace information");
LOG.Debug_Log("Debug information");
LOG.Info_Log("General information");
LOG.Warn_Log("Warning message");
LOG.Error_Log("Error information");
LOG.Fatal_Log("Fatal error");

// Log with parameters
LOG.Info_Log("User {0} performed {1} operation", new string[] { "admin", "login" });

// Log objects
var data = new { Id = 1, Name = "Test" };
LOG.Info_Log("Data record", data);

// Log exceptions
try {
    // Code
} catch (Exception ex) {
    LOG.Error_Log(ex);
}

// Custom log type
LOG.CustomName_Log("API", "External service call");
```

## Log File Management

### Default Directory Structure
```
Application Root/
└── logs/                          # Default root directory (modifiable via LogPath)
    └── yyyyMMdd/                  # Date directory
        └── LogFiles/              # Default log file directory (modifiable via TypeDirectories.DirectoryPath)
            └── [LogType]_Log.txt  # Log files
```

### Custom Directory Structure
You can configure independent directories for different log levels:

```csharp
LOG.Configure(options => {
    // Modify root directory
    options.LogPath = "CustomLogs";  // Default is "logs"
    
    // Configure independent directories for different levels
    options.TypeDirectories.DirectoryPath = "AllLogs";     // Default directory for unspecified levels
    options.TypeDirectories.ErrorPath = "ErrorLogs";       // Directory for error logs
    options.TypeDirectories.InfoPath = "InfoLogs";         // Directory for info logs
    options.TypeDirectories.WarnPath = "WarningLogs";      // Directory for warning logs
    options.TypeDirectories.DebugPath = "DebugLogs";       // Directory for debug logs
    options.TypeDirectories.TracePath = "TraceLogs";       // Directory for trace logs
    options.TypeDirectories.FatalPath = "FatalLogs";       // Directory for fatal logs
    options.TypeDirectories.CustomPath = "CustomLogs";     // Directory for custom type logs
});
```

Example directory structure after configuration:
```
Application Root/
└── CustomLogs/                    # Custom root directory
    └── yyyyMMdd/                  # Date directory
        ├── ErrorLogs/             # Error logs directory
        │   └── Error_Log.txt
        ├── InfoLogs/              # Info logs directory
        │   └── Info_Log.txt
        ├── WarningLogs/           # Warning logs directory
        │   └── Warn_Log.txt
        └── AllLogs/               # Default directory (for unspecified log types)
            └── [LogType]_Log.txt
```

### File Naming Rules
- Basic format: `[LogType]_Log.txt`
- Split files: `[LogType]_part[N]_Log.txt`
- Custom logs: `[CustomName]_Log.txt`

### File Size Management
```csharp
LOG.Configure(options => {
    // Set single file size limit (in MB)
    options.SetFileSizeInMB(50);  // Automatically split when file reaches 50MB
});
```

When a file exceeds the size limit, new split files are automatically created:
- First split file: `[LogType]_part1_Log.txt`
- Second split file: `[LogType]_part2_Log.txt`
- And so on...

### Example Use Cases

1. Unified log management:
```csharp
LOG.Configure(options => {
    options.LogPath = "logs";
    options.TypeDirectories.DirectoryPath = "LogFiles";
});
```

2. Separate error log storage:
```csharp
LOG.Configure(options => {
    options.LogPath = "logs";
    options.TypeDirectories.ErrorPath = "CriticalErrors";
    options.TypeDirectories.FatalPath = "CriticalErrors";
});
```

3. Fully separated logging system:
```csharp
LOG.Configure(options => {
    options.LogPath = "SystemLogs";
    options.TypeDirectories.ErrorPath = "Errors";
    options.TypeDirectories.InfoPath = "Information";
    options.TypeDirectories.WarnPath = "Warnings";
    options.TypeDirectories.DebugPath = "Debugging";
    options.TypeDirectories.TracePath = "Traces";
    options.TypeDirectories.FatalPath = "Critical";
    options.TypeDirectories.CustomPath = "Custom";
});
```

### Automatic Cleanup Mechanism
```csharp
// Set log retention period
LOG.Configure(options => {
    options.KeepDays = -30; // Keep logs for the last 30 days
});
```

## Exception Handling Features

### Detailed Exception Logging
```csharp
try {
    // Your code
} catch (Exception ex) {
    // Log complete exception information, including:
    // - Exception type and message
    // - Stack trace
    // - Inner exceptions
    // - Additional properties
    LOG.Error_Log(ex);
}
```

### Custom Exception Information
```csharp
try {
    // Your code
} catch (Exception ex) {
    // Add custom message
    LOG.Error_Log("Data processing failed", ex);
    
    // Also log related data
    var contextData = new { UserId = "123", Operation = "DataProcess" };
    LOG.Error_Log("Operation context", contextData);
}
```

### Exception Serialization
```csharp
try {
    // Your code
} catch (Exception ex) {
    // Exception will be automatically serialized to structured JSON format
    LOG.Error_Log(ex);
    
    // Or serialize with other information
    var errorContext = new {
        Exception = ex,
        TimeStamp = DateTime.Now,
        Environment = "Production"
    };
    LOG.Error_Log(errorContext);
}
```

## Immediate Write Mode

### Synchronous Immediate Write
```csharp
// Use immediateFlush parameter to force immediate writing
LOG.Error_Log("Important error", new string[] { "error_details" }, true, true);

// For custom logs
LOG.CustomName_Log("Critical", "System anomaly", new string[] { "error_code" }, true, true);
```

### Asynchronous Immediate Write Configuration
```csharp
LOG.Configure(options => {
    options.EnableAsyncLogging = true;
    options.ConfigureAsync(async => {
        async.FlushIntervalMs = 100;     // Reduce write interval
        async.MaxBatchSize = 1;          // Set minimum batch size
        async.MaxQueueSize = 1000;       // Set appropriate queue size
    });
});

// Error and Fatal level logs automatically trigger immediate writing
LOG.Error_Log("Severe error");
LOG.Fatal_Log("System crash");
```

### Conditional Immediate Write
```csharp
// Decide whether to write immediately based on conditions
void LogMessage(string message, bool isCritical) {
    if (isCritical) {
        LOG.Error_Log(message, new string[] { }, true, true);  // Immediate write
    } else {
        LOG.Info_Log(message);  // Normal write
    }
}
```

## Performance Optimization

- Asynchronous writing to avoid I/O blocking
- Smart batch processing to reduce disk operations
- Optimized serialization mechanism
- Thread-safe queue management
- Automatic file management to avoid oversized files

## Best Practices

1. Choose between synchronous or asynchronous mode based on application needs
2. Configure appropriate batch size and write intervals
3. Adjust file size limits based on log volume
4. Set reasonable log retention periods
5. Use custom types for log classification
6. Record necessary exception information at critical points

## Troubleshooting

Common issue handling:

1. File Access Permission Issues
   - Ensure application has write permissions
   - Check folder access permission settings

2. Performance Issues
   - Adjust async configuration parameters
   - Check log file size settings
   - Optimize write frequency

3. File Management
   - Regularly check log cleanup status
   - Monitor disk space usage

## License

MIT License

## Support & Reporting

- GitHub Issues: [Report Issues](https://github.com/ozakboy/ozakboy.NLOG/issues)
- Pull Requests: [Contribute Code](https://github.com/ozakboy/ozakboy.NLOG/pulls)