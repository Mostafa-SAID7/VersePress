namespace VersePress.Web.Middleware;

/// <summary>
/// Middleware to handle theme persistence by reading theme preference from cookie
/// and injecting it into ViewBag for use in layout views.
/// </summary>
public class ThemeMiddleware
{
    private readonly RequestDelegate _next;
    private const string ThemeCookieName = "theme";
    private const string DefaultTheme = "light";

    public ThemeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Read theme preference from cookie, default to light theme
        var theme = context.Request.Cookies[ThemeCookieName] ?? DefaultTheme;

        // Validate theme value (only allow "light" or "dark")
        if (theme != "light" && theme != "dark")
        {
            theme = DefaultTheme;
        }

        // Store theme in HttpContext.Items for access in views
        context.Items["Theme"] = theme;

        await _next(context);
    }
}

/// <summary>
/// Extension method to register ThemeMiddleware in the pipeline
/// </summary>
public static class ThemeMiddlewareExtensions
{
    public static IApplicationBuilder UseTheme(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ThemeMiddleware>();
    }
}
