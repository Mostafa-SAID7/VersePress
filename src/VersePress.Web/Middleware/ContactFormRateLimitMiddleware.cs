using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace VersePress.Web.Middleware;

/// <summary>
/// Rate limiting middleware for contact form submissions
/// Limits to 3 submissions per hour per IP address
/// </summary>
public class ContactFormRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ContactFormRateLimitMiddleware> _logger;
    private const int MaxSubmissions = 3;
    private static readonly TimeSpan WindowDuration = TimeSpan.FromHours(1);

    public ContactFormRateLimitMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<ContactFormRateLimitMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply rate limiting to contact form POST requests
        if (context.Request.Path.StartsWithSegments("/Home/Contact") &&
            context.Request.Method == "POST")
        {
            var ipAddress = GetClientIpAddress(context);
            var cacheKey = $"ContactForm_RateLimit_{ipAddress}";

            // Get current submission count
            if (!_cache.TryGetValue(cacheKey, out int submissionCount))
            {
                submissionCount = 0;
            }

            if (submissionCount >= MaxSubmissions)
            {
                _logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", ipAddress);
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "text/html";
                
                await context.Response.WriteAsync(@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Too Many Requests</title>
                        <style>
                            body { font-family: Arial, sans-serif; text-align: center; padding: 50px; }
                            h1 { color: #dc3545; }
                        </style>
                    </head>
                    <body>
                        <h1>Too Many Requests</h1>
                        <p>You have exceeded the maximum number of contact form submissions (3 per hour).</p>
                        <p>Please try again later.</p>
                        <a href='/'>Return to Home</a>
                    </body>
                    </html>
                ");
                
                return;
            }

            // Increment submission count
            submissionCount++;
            _cache.Set(cacheKey, submissionCount, WindowDuration);
        }

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Try to get IP from X-Forwarded-For header (for proxies/load balancers)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Fall back to RemoteIpAddress
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
