using Microsoft.Extensions.Options;

namespace VersePress.Web.Configuration;

/// <summary>
/// Validates required configuration settings on application startup
/// </summary>
public class ConfigurationValidator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationValidator> _logger;

    public ConfigurationValidator(IConfiguration configuration, ILogger<ConfigurationValidator> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Validates all required configuration keys
    /// </summary>
    public void ValidateConfiguration()
    {
        var missingKeys = new List<string>();

        // Validate connection string
        if (string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")))
        {
            missingKeys.Add("ConnectionStrings:DefaultConnection");
        }

        // Validate application settings
        if (string.IsNullOrWhiteSpace(_configuration["Application:Name"]))
        {
            missingKeys.Add("Application:Name");
        }

        if (string.IsNullOrWhiteSpace(_configuration["Application:BaseUrl"]))
        {
            missingKeys.Add("Application:BaseUrl");
        }

        // Validate Serilog settings
        if (_configuration.GetSection("Serilog").GetChildren().Count() == 0)
        {
            missingKeys.Add("Serilog");
        }

        // Log warnings for optional but recommended settings
        if (string.IsNullOrWhiteSpace(_configuration["EmailSettings:SmtpServer"]))
        {
            _logger.LogWarning("EmailSettings:SmtpServer is not configured. Email functionality will not work.");
        }

        // Fail fast if required configuration is missing
        if (missingKeys.Any())
        {
            var errorMessage = $"Missing required configuration keys: {string.Join(", ", missingKeys)}";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation("Configuration validation passed successfully");
    }
}

/// <summary>
/// Email settings configuration
/// </summary>
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Application settings configuration
/// </summary>
public class ApplicationSettings
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string[] SupportedCultures { get; set; } = Array.Empty<string>();
    public string DefaultCulture { get; set; } = string.Empty;
}

/// <summary>
/// Caching settings configuration
/// </summary>
public class CachingSettings
{
    public OutputCacheSettings OutputCache { get; set; } = new();
    public DistributedCacheSettings DistributedCache { get; set; } = new();
}

public class OutputCacheSettings
{
    public int DefaultExpirationMinutes { get; set; }
    public int HomePageExpirationMinutes { get; set; }
    public int BlogListExpirationMinutes { get; set; }
    public int SitemapExpirationMinutes { get; set; }
    public int RssFeedExpirationMinutes { get; set; }
}

public class DistributedCacheSettings
{
    public int SlidingExpirationMinutes { get; set; }
    public int AbsoluteExpirationMinutes { get; set; }
}

/// <summary>
/// Rate limiting settings configuration
/// </summary>
public class RateLimitingSettings
{
    public ContactFormRateLimitSettings ContactForm { get; set; } = new();
}

public class ContactFormRateLimitSettings
{
    public int PermitLimit { get; set; }
    public int WindowMinutes { get; set; }
}
