# TNT API Logging Guide

## Overview

The TNT API uses **Serilog** for structured logging with environment-aware configuration:
- **Production (Render)**: Logs to console → captured by Render's log aggregation
- **Development (Local)**: Logs to both console AND rotating daily files in `logs/` folder

## Global Exception Handler

✅ **Already Configured** in `server/MiddleWare/GlobalExceptionHandler.cs`

The exception handler automatically:
- Catches all unhandled exceptions
- Logs with structured context (user, IP, path, method)
- Returns standardized error responses (RFC 9457 ProblemDetails)
- Includes stack traces in development only

## Log Levels

```
Trace < Debug < Information < Warning < Error < Fatal
```

### Production (Render)
- **Information** and above logged to console
- **Warning** and above logged to file
- Microsoft.AspNetCore logs only **Warning** and above

### Development (Local)
- **Debug** and above logged to console
- **Debug** and above logged to file
- Microsoft.AspNetCore logs **Information** and above

## Configuration

### appsettings.json (Production)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

### appsettings.Development.json (Local)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft.AspNetCore": "Information"
      }
    }
  }
}
```

## Log Files (Local Development)

**Location**: `server/logs/tnt-YYYYMMDD.log`

- **Rolling**: New file created daily
- **Retention**: Last 7 days kept automatically
- **Format**: `[timestamp] [LEVEL] message`

Example:
```
[2026-04-05 10:30:15.123 +00:00] [INF] Starting TNT API application
[2026-04-05 10:30:16.456 +00:00] [ERR] Unhandled exception: GET /api/hotels | User: admin | IP: 127.0.0.1
System.Data.DataException: Error parsing column...
```

## Accessing Logs on Render

### Option 1: Render Dashboard (Recommended)
1. Go to https://dashboard.render.com
2. Select your **tnt-api** service
3. Click **Logs** tab
4. Filter by:
   - Time range
   - Log level (Info, Warning, Error)
   - Search text

### Option 2: Render CLI
```bash
# Install Render CLI
npm install -g @render-dev/cli

# View live logs
render logs -s tnt-api --tail

# View last 100 lines
render logs -s tnt-api --lines 100
```

### Option 3: External Logging Services (Advanced)

For production-grade logging, integrate with:

1. **Seq** (Self-hosted or Cloud)
```bash
dotnet add package Serilog.Sinks.Seq
```
```csharp
.WriteTo.Seq("https://your-seq-server.com", apiKey: "your-key")
```

2. **Application Insights** (Azure)
```bash
dotnet add package Serilog.Sinks.ApplicationInsights
```

3. **Sentry** (Error tracking)
```bash
dotnet add package Sentry.Serilog
```

## Important: File Logging on Render

⚠️ **Container filesystems are ephemeral on Render**

- Files written to `logs/` folder are **NOT persisted** across deployments
- Logs are **lost** when containers restart
- File logging is **only reliable for local development**

**For production, rely on:**
1. Console logs (automatically captured by Render)
2. External logging services (Seq, AppInsights, etc.)

## Cloudflare Pages (Frontend)

Your **Angular frontend** is deployed on Cloudflare Pages, which has **separate logging**:

- **Access via**: Cloudflare Dashboard > Pages > suryantra > Logs
- Cloudflare logs show:
  - HTTP requests
  - Response times
  - Status codes
  - Traffic patterns

The backend (.NET API) and frontend (Angular) have completely separate logging systems.

## Custom Logging in Controllers

You can inject `ILogger<T>` into any controller or service:

```csharp
public class HotelController : ControllerBase
{
    private readonly ILogger<HotelController> _logger;
    
    public HotelController(ILogger<HotelController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult GetHotels()
    {
        _logger.LogInformation("Fetching all hotels");
        
        try 
        {
            // Your code
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch hotels");
            throw; // GlobalExceptionHandler will catch this
        }
    }
}
```

## Structured Logging Examples

```csharp
// Simple message
_logger.LogInformation("User logged in");

// With parameters (structured)
_logger.LogInformation("User {UserId} logged in from {IP}", userId, ipAddress);

// With exception
_logger.LogError(exception, "Failed to process order {OrderId}", orderId);

// Conditional logging
if (_logger.IsEnabled(LogLevel.Debug))
{
    _logger.LogDebug("Complex debug data: {Data}", complexObject);
}
```

## Log Viewing Commands (Local)

```bash
# View latest log file
tail -f server/logs/tnt-*.log

# View errors only
grep -i "ERR" server/logs/tnt-*.log

# View logs from specific date
cat server/logs/tnt-20260405.log

# Clear old logs (keep last 3 days)
find server/logs -name "tnt-*.log" -mtime +3 -delete
```

## Deployment Checklist

✅ Before deploying to Render:

1. Verify Serilog logs to **console** (not just files)
2. Test exception handling triggers proper logging
3. Remove any `Console.WriteLine()` debug statements
4. Set appropriate log levels in production appsettings
5. Consider external logging service for long-term retention

## Troubleshooting

### Logs not appearing in Render
- Check that `Log.Logger` is configured in `Program.cs`
- Verify `builder.Host.UseSerilog()` is called
- Ensure logs are written to **Console sink**

### File logs not being created locally
- Check write permissions on `logs/` folder
- Verify file sink is configured with correct path
- Look for startup errors in console

### Too many logs
- Increase minimum level to `Warning` or `Error`
- Add specific overrides for noisy namespaces:
  ```json
  "Override": {
    "Microsoft.EntityFrameworkCore": "Warning"
  }
  ```

## Summary

| Environment | Console Logs | File Logs | Retention |
|------------|-------------|-----------|-----------|
| **Local Dev** | ✅ Debug+ | ✅ Debug+ (7 days) | Local disk |
| **Render Production** | ✅ Information+ | ⚠️ Warning+ (ephemeral) | Render dashboard (7 days) |
| **Cloudflare (Frontend)** | N/A | N/A | Cloudflare dashboard |

Your global exception handler is **already working** and will automatically log all errors to both console and file based on the environment!
