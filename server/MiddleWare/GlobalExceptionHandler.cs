using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace server.MiddleWare;

/// <summary>
/// .NET 10 IExceptionHandler — structured, testable, DI-friendly global error handler.
/// Replaces the old RequestDelegate-based middleware pattern.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly string _logFilePath;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
        _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "errors.log");
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Structured logging
        _logger.LogError(exception, "Unhandled exception for {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        // File logging (kept for backward compatibility)
        var errorDetails = $"[{DateTime.UtcNow:o}] {httpContext.Request.Method} {httpContext.Request.Path} | {exception.Message}\n{exception.StackTrace}\n";
        await File.AppendAllTextAsync(_logFilePath, errorDetails, cancellationToken);

        // Return RFC 9457 ProblemDetails response
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An internal server error occurred.",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Exception is handled
    }
}
