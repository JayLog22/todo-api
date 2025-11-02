using System.Net;
using System.Text.Json;

namespace TodoApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            StatusCode = HttpStatusCode.InternalServerError
        };

        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
                response.StatusCode = HttpStatusCode.BadRequest;
                break;
            case KeyNotFoundException:
                response.StatusCode = HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException:
                response.StatusCode = HttpStatusCode.Unauthorized;
                break;
            default:
                response.StatusCode = HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.StatusCode = (int)response.StatusCode;

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
}
