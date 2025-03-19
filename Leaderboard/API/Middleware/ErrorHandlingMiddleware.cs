using System.Net;
using System.Text.Json;

namespace API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Exception happened in {context.Request.Method} {context.Request.Path}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorResponse = new
            {
                message = exception.Message,
                statusCode = context.Response.StatusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}