using System.Text.Json;

namespace EasyShop.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found.");

            await HandleExceptionAsync(
                context,
                StatusCodes.Status404NotFound,
                ex.Message
            );
        }
        catch (UnauthorizedAccessException ex)
{
    _logger.LogWarning(ex, "Unauthorized request.");

    await HandleExceptionAsync(
        context,
        StatusCodes.Status401Unauthorized,
        ex.Message
    );
}
        catch (InvalidOperationException ex)
{
    _logger.LogWarning(ex, "Business rule violation.");

    await HandleExceptionAsync(
        context,
        StatusCodes.Status409Conflict,
        ex.Message
    );
}
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Something went wrong."
            );
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = message,
            statusCode = statusCode
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response)
        );
    }
}