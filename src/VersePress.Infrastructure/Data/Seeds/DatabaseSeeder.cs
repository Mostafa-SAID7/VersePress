using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Seeds;

/// <summary>
/// Orchestrates all database seeders for development environment
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<DatabaseSeeder>();
    }

    /// <summary>
    /// Seeds the database with tech-focused sample data
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Check if data already exists
            if (await _context.BlogPosts.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Starting database seeding with tech-focused content...");

            // Seed users and roles
            var userSeeder = new UserSeeder(_userManager, _roleManager, _loggerFactory.CreateLogger<UserSeeder>());
            var (adminUser, authorUser1, authorUser2) = await userSeeder.SeedAsync();

            // Seed tags
            var tagSeeder = new TagSeeder(_context, _loggerFactory.CreateLogger<TagSeeder>());
            var tags = await tagSeeder.SeedAsync();

            // Seed categories
            var categorySeeder = new CategorySeeder(_context, _loggerFactory.CreateLogger<CategorySeeder>());
            var categories = await categorySeeder.SeedAsync();

            // Seed series
            var seriesSeeder = new SeriesSeeder(_context, _loggerFactory.CreateLogger<SeriesSeeder>());
            var series = await seriesSeeder.SeedAsync();

            // Seed projects
            var projectSeeder = new ProjectSeeder(_context, _loggerFactory.CreateLogger<ProjectSeeder>());
            var projects = await projectSeeder.SeedAsync();

            // Seed blog posts with tech news content
            var blogPostSeeder = new BlogPostSeeder(_context, _loggerFactory.CreateLogger<BlogPostSeeder>());
            await blogPostSeeder.SeedAsync(adminUser, authorUser1, authorUser2, tags, categories, series, projects);

            _logger.LogInformation("Database seeding completed successfully with tech-focused content");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
