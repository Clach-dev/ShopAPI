namespace Presentation.Common.Middleware;

public class LoggingMiddleware(
    RequestDelegate next,
    ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        LogRequest(context.Request);

        await next(context);

        LogResponse(context.Response);
    }

    private void LogRequest(HttpRequest request)
    {
        logger.LogInformation("Incuming Request: Method: {Method}, Path: {Path}", request.Method, request.Path);
    }

    private void LogResponse(HttpResponse response)
    {
        logger.LogInformation("Outgoing Response: Status Code: {StatusCode}", response.StatusCode);
    }
}