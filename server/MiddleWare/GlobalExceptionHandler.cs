using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace server.MiddleWare;

/// <summary>
/// .NET 10 IExceptionHandler — structured, testable, DI-friendly global error handler.
/// Works with Serilog for comprehensive logging to console (Render) and file (local dev).
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IStringLocalizer<GlobalExceptionHandler> _localizer;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IStringLocalizer<GlobalExceptionHandler> localizer,
        IHostEnvironment environment)
    {
        _logger = logger;
        _localizer = localizer;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Map known exception types to appropriate HTTP status codes
        var (statusCode, rfcType) = exception switch
        {
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "https://tools.ietf.org/html/rfc9110#section-15.5.2"),
            _ => (
                StatusCodes.Status500InternalServerError,
                "https://tools.ietf.org/html/rfc9110#section-15.6.1")
        };

        // Structured logging with context (Serilog handles file/console routing)
        var logLevel = statusCode >= 500 ? Microsoft.Extensions.Logging.LogLevel.Error
                                         : Microsoft.Extensions.Logging.LogLevel.Warning;
        _logger.Log(logLevel, exception,
            "Unhandled exception [{Status}]: {Method} {Path} | User: {User} | IP: {IP}",
            statusCode,
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.User?.Identity?.Name ?? "Anonymous",
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

        // Get localized error message
        var errorMessage = statusCode == StatusCodes.Status401Unauthorized
            ? exception.Message
            : _localizer["Error_InternalServerError"].Value;

        // Return RFC 9457 ProblemDetails response
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = errorMessage,
            Type = rfcType,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        // Include stack trace in development only
        if (_environment.IsDevelopment() && statusCode >= 500)
        {
            problemDetails.Detail = exception.ToString();
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Exception is handled
    }
}
