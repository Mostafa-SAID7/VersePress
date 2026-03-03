using System.Diagnostics;

namespace VersePress.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception with full stack trace and request context
        _logger.LogError(exception,
            "Unhandled exception occurred. Request: {Method} {Path} {QueryString}. User: {User}. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.User?.Identity?.Name ?? "Anonymous",
            Activity.Current?.Id ?? context.TraceIdentifier);

        // Set response status code
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Redirect to custom error page
        context.Response.Redirect("/Home/Error?statusCode=500");
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
