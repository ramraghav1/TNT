using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly string _logFilePath;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
        _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "errors.log");
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var errorDetails = $"[{DateTime.Now}] {context.Request.Method} {context.Request.Path} | {ex.Message}\n{ex.StackTrace}\n";
        await File.AppendAllTextAsync(_logFilePath, errorDetails);

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            status = 500,
            message = "An internal server error occurred."
        };

        var json = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(json);
    }
}
