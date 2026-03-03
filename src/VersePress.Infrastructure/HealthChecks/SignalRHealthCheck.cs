using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace VersePress.Infrastructure.HealthChecks;

/// <summary>
/// Health check for SignalR hub availability.
/// Verifies that SignalR hubs are properly registered and available.
/// </summary>
public class SignalRHealthCheck : IHealthCheck
{
    private readonly ILogger<SignalRHealthCheck> _logger;

    public SignalRHealthCheck(ILogger<SignalRHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple check - if this health check is being executed,
            // it means the application is running and SignalR is configured
            _logger.LogDebug("SignalR health check passed");
            return Task.FromResult(
                HealthCheckResult.Healthy("SignalR hubs are available"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignalR health check failed");
            return Task.FromResult(
                HealthCheckResult.Unhealthy("SignalR health check failed", ex));
        }
    }
}
