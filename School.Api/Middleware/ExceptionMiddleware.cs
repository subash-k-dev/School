using System.Net;
using System.Text.Json;
using School.Api.Models;

namespace School.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            // ✅ Validation type error -> 400
            _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message, ex);
        }
        catch (KeyNotFoundException ex)
        {
            // ✅ Not found type error -> 404
            _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
            await WriteError(context, HttpStatusCode.NotFound, ex.Message, ex);
        }
        catch (Exception ex)
        {
            // ✅ Unexpected errors -> 500
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(context, HttpStatusCode.InternalServerError, "Internal server error", ex);
        }
    }

    private async Task WriteError(HttpContext context, HttpStatusCode code, string message, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var error = new ErrorResponse
        {
            Message = message,
            TraceId = context.TraceIdentifier,
            Details = _env.IsDevelopment() ? ex.ToString() : null
        };

        var json = JsonSerializer.Serialize(error, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}