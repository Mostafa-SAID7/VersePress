using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Seeds;

/// <summary>
/// Seeds users and roles for the application
/// </summary>
public class UserSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<UserSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<(User admin, User author1, User author2)> SeedAsync()
    {
        // Seed roles
        await SeedRolesAsync();

        // Seed users
        return await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "Author" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
                _logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }

    private async Task<(User admin, User author1, User author2)> SeedUsersAsync()
    {
        // Create admin user
        var adminUser = new User
        {
            UserName = "admin@versepress.com",
            Email = "admin@versepress.com",
            EmailConfirmed = true,
            FullName = "Admin User",
            Bio = "Platform Administrator | مدير المنصة",
            ProfileImageUrl = "https://ui-avatars.com/api/?name=Admin+User&size=200"
        };

        if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
        {
            await _userManager.CreateAsync(adminUser, "Admin@123");
            await _userManager.AddToRoleAsync(adminUser, "Admin");
            _logger.LogInformation("Created admin user: {Email}", adminUser.Email);
        }
        else
        {
            adminUser = await _userManager.FindByEmailAsync(adminUser.Email) ?? adminUser;
        }

        // Create tech author 1
        var authorUser1 = new User
        {
            UserName = "sarah.tech@versepress.com",
            Email = "sarah.tech@versepress.com",
            EmailConfirmed = true,
            FullName = "Sarah Johnson | سارة جونسون",
            Bio = "Senior Software Engineer & Tech Writer | مهندسة برمجيات ومدونة تقنية",
            ProfileImageUrl = "https://ui-avatars.com/api/?name=Sarah+Johnson&size=200"
        };

        if (await _userManager.FindByEmailAsync(authorUser1.Email) == null)
        {
            await _userManager.CreateAsync(authorUser1, "Author@123");
            await _userManager.AddToRoleAsync(authorUser1, "Author");
            _logger.LogInformation("Created author user: {Email}", authorUser1.Email);
        }
        else
        {
            authorUser1 = await _userManager.FindByEmailAsync(authorUser1.Email) ?? authorUser1;
        }

        // Create tech author 2
        var authorUser2 = new User
        {
            UserName = "ahmed.dev@versepress.com",
            Email = "ahmed.dev@versepress.com",
            EmailConfirmed = true,
            FullName = "Ahmed Hassan | أحمد حسن",
            Bio = "Cloud Architect & DevOps Specialist | مهندس سحابي ومتخصص DevOps",
            ProfileImageUrl = "https://ui-avatars.com/api/?name=Ahmed+Hassan&size=200"
        };

        if (await _userManager.FindByEmailAsync(authorUser2.Email) == null)
        {
            await _userManager.CreateAsync(authorUser2, "Author@123");
            await _userManager.AddToRoleAsync(authorUser2, "Author");
            _logger.LogInformation("Created author user: {Email}", authorUser2.Email);
        }
        else
        {
            authorUser2 = await _userManager.FindByEmailAsync(authorUser2.Email) ?? authorUser2;
        }

        return (adminUser!, authorUser1!, authorUser2!);
    }
}
