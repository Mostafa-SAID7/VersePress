using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using VersePress.Application.Interfaces;
using VersePress.Application.Services;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;
using VersePress.Infrastructure.Data;
using VersePress.Infrastructure.Repositories;
using VersePress.Web.Middleware;
using Serilog;
using Serilog.Events;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/versepress-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{MachineName}] [{EnvironmentName}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10_485_760) // 10MB
    .CreateLogger();

try
{
    Log.Information("Starting VersePress application");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// Override Serilog configuration based on environment
if (builder.Environment.IsProduction())
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithThreadId()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/versepress-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{MachineName}] [{EnvironmentName}] {Message:lj} {Properties:j}{NewLine}{Exception}",
            retainedFileCountLimit: 30,
            fileSizeLimitBytes: 10_485_760) // 10MB
        .CreateLogger();
}

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Add WebOptimizer for bundling and minification
builder.Services.AddWebOptimizer(pipeline =>
{
    // Minify CSS files
    pipeline.MinifyCssFiles("wwwroot/css/**/*.css");
    
    // Minify JavaScript files
    pipeline.MinifyJsFiles("wwwroot/js/**/*.js");
    
    // Create CSS bundle
    pipeline.AddCssBundle("/css/site.min.css", "wwwroot/css/site.css");
    
    // Create JS bundle
    pipeline.AddJavaScriptBundle("/js/site.min.js", "wwwroot/js/site.js");
});

// Add response compression with Gzip and Brotli
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/json",
        "application/javascript",
        "text/css",
        "text/html",
        "text/plain",
        "image/svg+xml"
    });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

// Add output caching for SEO endpoints
builder.Services.AddOutputCache();

// Add memory cache for view counting
builder.Services.AddMemoryCache();

// Configure SignalR with JSON serialization options
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.PayloadSerializerOptions.WriteIndented = false;
    });

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("VersePress.Infrastructure")
    );
    
    // Enable detailed logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
    
    // Log database commands with execution time
    options.LogTo(
        (eventId, level) => level >= LogLevel.Information 
            && eventId.Name == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted",
        (eventData) =>
        {
            if (eventData is Microsoft.EntityFrameworkCore.Diagnostics.CommandExecutedEventData commandData)
            {
                Log.Information("Database query executed in {ElapsedMilliseconds}ms: {CommandText}",
                    commandData.Duration.TotalMilliseconds,
                    commandData.Command.CommandText);
            }
        });
});

// Configure localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "ar-SA" };
    options.SetDefaultCulture("en-US")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
    
    options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider());
});

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Set to true in production with email service
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthorPolicy", policy =>
        policy.RequireRole("Author", "Admin"));
    
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

// Register Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configure application settings
builder.Services.Configure<VersePress.Web.Configuration.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<VersePress.Web.Configuration.ApplicationSettings>(
    builder.Configuration.GetSection("Application"));
builder.Services.Configure<VersePress.Web.Configuration.CachingSettings>(
    builder.Configuration.GetSection("Caching"));
builder.Services.Configure<VersePress.Web.Configuration.RateLimitingSettings>(
    builder.Configuration.GetSection("RateLimiting"));

// Register configuration validator
builder.Services.AddSingleton<VersePress.Web.Configuration.ConfigurationValidator>();

// Register Application Services
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ISeoService, SeoService>();
builder.Services.AddScoped<IViewCounterService, ViewCounterService>();
builder.Services.AddScoped<IShareTrackingService, ShareTrackingService>();
builder.Services.AddScoped<VersePress.Application.Interfaces.IEmailService, VersePress.Infrastructure.Services.EmailService>();

// Configure email settings
builder.Services.Configure<VersePress.Infrastructure.Services.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Configure CORS for SignalR connections
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCorsPolicy", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                ?? new[] { "http://localhost:5000", "https://localhost:5001" }
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Configure health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>(
        name: "database",
        tags: new[] { "db", "sql" })
    .AddCheck<VersePress.Infrastructure.HealthChecks.SignalRHealthCheck>(
        name: "signalr",
        tags: new[] { "signalr", "hub" },
        timeout: TimeSpan.FromSeconds(3));

var app = builder.Build();

// Validate configuration on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var configValidator = services.GetRequiredService<VersePress.Web.Configuration.ConfigurationValidator>();
        configValidator.ValidateConfiguration();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Configuration validation failed. Application cannot start.");
        throw;
    }
}

// Apply migrations automatically in development environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            // Seed database with sample data
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var seederLogger = loggerFactory.CreateLogger<VersePress.Infrastructure.Data.DatabaseSeeder>();
            
            var seeder = new VersePress.Infrastructure.Data.DatabaseSeeder(
                context,
                userManager,
                roleManager,
                seederLogger);
            
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Use custom exception handling middleware in production
    app.UseExceptionHandling();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Use developer exception page in development
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Enable status code pages for custom error handling
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

// Enable Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "Unknown");
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        diagnosticContext.Set("UserAgent", string.IsNullOrEmpty(userAgent) ? "Unknown" : userAgent);
        diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
    };
    // Log slow requests (>1000ms) as warnings
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null) return LogEventLevel.Error;
        if (elapsed > 1000) return LogEventLevel.Warning;
        return LogEventLevel.Information;
    };
});

// Enable WebOptimizer (must be before static files)
app.UseWebOptimizer();

// Enable response compression (must be before static files)
app.UseResponseCompression();

// Configure static files with caching
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static assets for 1 year
        const int durationInSeconds = 60 * 60 * 24 * 365; // 1 year
        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={durationInSeconds}");
    }
});

// Disable caching for HTML responses in development
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        await next();
        
        // Only apply to HTML responses
        if (context.Response.ContentType?.Contains("text/html") == true)
        {
            context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.Headers.Append("Pragma", "no-cache");
            context.Response.Headers.Append("Expires", "0");
        }
    });
}

app.UseRouting();

// Enable output caching
app.UseOutputCache();

// Enable contact form rate limiting
app.UseMiddleware<VersePress.Web.Middleware.ContactFormRateLimitMiddleware>();

// Enable theme middleware
app.UseTheme();

// Enable localization
app.UseRequestLocalization();

// Enable CORS for SignalR
app.UseCors("SignalRCorsPolicy");

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Map health check endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        if (report.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                status = "Healthy",
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration.TotalMilliseconds
                })
            }));
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                status = "Unhealthy",
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    exception = e.Value.Exception?.Message,
                    duration = e.Value.Duration.TotalMilliseconds
                })
            }));
        }
    }
});

// Map SignalR hub endpoints
app.MapHub<VersePress.Web.Hubs.NotificationHub>("/hubs/notifications");
app.MapHub<VersePress.Web.Hubs.InteractionHub>("/hubs/interactions");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
