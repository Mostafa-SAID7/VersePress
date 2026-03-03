using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;

namespace VersePress.Infrastructure.Data;

/// <summary>
/// Seeds the database with sample data for development
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with sample data
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

            _logger.LogInformation("Starting database seeding...");

            // Seed roles
            await SeedRolesAsync();

            // Seed users
            var (adminUser, authorUser1, authorUser2) = await SeedUsersAsync();

            // Seed tags
            var tags = await SeedTagsAsync();

            // Seed categories
            var categories = await SeedCategoriesAsync();

            // Seed series
            var series = await SeedSeriesAsync();

            // Seed projects
            var projects = await SeedProjectsAsync();

            // Seed blog posts
            await SeedBlogPostsAsync(adminUser, authorUser1, authorUser2, tags, categories, series, projects);

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
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
            Bio = "Platform Administrator",
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

        // Create author user 1
        var authorUser1 = new User
        {
            UserName = "john.doe@versepress.com",
            Email = "john.doe@versepress.com",
            EmailConfirmed = true,
            FullName = "John Doe",
            Bio = "Software developer passionate about web technologies and clean code",
            ProfileImageUrl = "https://ui-avatars.com/api/?name=John+Doe&size=200"
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

        // Create author user 2
        var authorUser2 = new User
        {
            UserName = "jane.smith@versepress.com",
            Email = "jane.smith@versepress.com",
            EmailConfirmed = true,
            FullName = "Jane Smith",
            Bio = "Tech writer and developer advocate",
            ProfileImageUrl = "https://ui-avatars.com/api/?name=Jane+Smith&size=200"
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

    private async Task<List<Tag>> SeedTagsAsync()
    {
        var tags = new List<Tag>
        {
            new() { NameEn = "Technology", NameAr = "التكنولوجيا", Slug = "technology" },
            new() { NameEn = "Programming", NameAr = "البرمجة", Slug = "programming" },
            new() { NameEn = "Web Development", NameAr = "تطوير الويب", Slug = "web-development" },
            new() { NameEn = "ASP.NET Core", NameAr = "ASP.NET Core", Slug = "aspnet-core" },
            new() { NameEn = "C#", NameAr = "C#", Slug = "csharp" },
            new() { NameEn = "JavaScript", NameAr = "JavaScript", Slug = "javascript" },
            new() { NameEn = "Clean Architecture", NameAr = "البنية النظيفة", Slug = "clean-architecture" },
            new() { NameEn = "Design Patterns", NameAr = "أنماط التصميم", Slug = "design-patterns" },
            new() { NameEn = "Best Practices", NameAr = "أفضل الممارسات", Slug = "best-practices" },
            new() { NameEn = "Tutorial", NameAr = "درس تعليمي", Slug = "tutorial" }
        };

        await _context.Tags.AddRangeAsync(tags);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} tags", tags.Count);

        return tags;
    }

    private async Task<List<Category>> SeedCategoriesAsync()
    {
        var categories = new List<Category>
        {
            new()
            {
                NameEn = "Tutorials",
                NameAr = "دروس تعليمية",
                Slug = "tutorials",
                DescriptionEn = "Step-by-step guides and tutorials",
                DescriptionAr = "أدلة ودروس تعليمية خطوة بخطوة"
            },
            new()
            {
                NameEn = "News",
                NameAr = "أخبار",
                Slug = "news",
                DescriptionEn = "Latest news and updates",
                DescriptionAr = "آخر الأخبار والتحديثات"
            },
            new()
            {
                NameEn = "Opinion",
                NameAr = "رأي",
                Slug = "opinion",
                DescriptionEn = "Thoughts and opinions on technology",
                DescriptionAr = "أفكار وآراء حول التكنولوجيا"
            },
            new()
            {
                NameEn = "How-To",
                NameAr = "كيفية",
                Slug = "how-to",
                DescriptionEn = "Practical how-to guides",
                DescriptionAr = "أدلة عملية"
            }
        };

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} categories", categories.Count);

        return categories;
    }

    private async Task<List<Series>> SeedSeriesAsync()
    {
        var series = new List<Series>
        {
            new()
            {
                NameEn = "Getting Started with ASP.NET Core",
                NameAr = "البدء مع ASP.NET Core",
                Slug = "getting-started-aspnet-core",
                DescriptionEn = "A comprehensive series for beginners learning ASP.NET Core",
                DescriptionAr = "سلسلة شاملة للمبتدئين في تعلم ASP.NET Core"
            },
            new()
            {
                NameEn = "Clean Architecture in Practice",
                NameAr = "البنية النظيفة في الممارسة",
                Slug = "clean-architecture-practice",
                DescriptionEn = "Learn how to implement Clean Architecture in real-world applications",
                DescriptionAr = "تعلم كيفية تطبيق البنية النظيفة في التطبيقات الحقيقية"
            }
        };

        await _context.Series.AddRangeAsync(series);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} series", series.Count);

        return series;
    }

    private async Task<List<Project>> SeedProjectsAsync()
    {
        var projects = new List<Project>
        {
            new()
            {
                NameEn = "VersePress Development",
                NameAr = "تطوير VersePress",
                Slug = "versepress-development",
                DescriptionEn = "Building a bilingual blog platform with ASP.NET Core",
                DescriptionAr = "بناء منصة مدونة ثنائية اللغة باستخدام ASP.NET Core"
            }
        };

        await _context.Projects.AddRangeAsync(projects);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} projects", projects.Count);

        return projects;
    }

    private async Task SeedBlogPostsAsync(
        User adminUser,
        User authorUser1,
        User authorUser2,
        List<Tag> tags,
        List<Category> categories,
        List<Series> series,
        List<Project> projects)
    {
        var blogPosts = new List<BlogPost>();
        var random = new Random();

        // Post 1 - Featured
        var post1 = new BlogPost
        {
            Slug = "introduction-to-aspnet-core-9",
            TitleEn = "Introduction to ASP.NET Core 9",
            TitleAr = "مقدمة إلى ASP.NET Core 9",
            ContentEn = GenerateSampleContent("ASP.NET Core 9 is the latest version of Microsoft's cross-platform framework for building modern web applications. In this comprehensive guide, we'll explore the new features and improvements that make ASP.NET Core 9 a powerful choice for web development."),
            ContentAr = GenerateSampleContent("ASP.NET Core 9 هو أحدث إصدار من إطار عمل Microsoft متعدد المنصات لبناء تطبيقات الويب الحديثة. في هذا الدليل الشامل، سنستكشف الميزات والتحسينات الجديدة التي تجعل ASP.NET Core 9 خيارًا قويًا لتطوير الويب."),
            ExcerptEn = "Discover the new features and improvements in ASP.NET Core 9",
            ExcerptAr = "اكتشف الميزات والتحسينات الجديدة في ASP.NET Core 9",
            FeaturedImageUrl = "https://picsum.photos/seed/post1/800/400",
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-10),
            ViewCount = random.Next(100, 500),
            AuthorId = authorUser1.Id,
            SeriesId = series[0].Id
        };
        post1.Tags.Add(tags[0]); // Technology
        post1.Tags.Add(tags[3]); // ASP.NET Core
        post1.Categories.Add(categories[0]); // Tutorials
        blogPosts.Add(post1);

        // Post 2 - Featured
        var post2 = new BlogPost
        {
            Slug = "clean-architecture-principles",
            TitleEn = "Clean Architecture Principles",
            TitleAr = "مبادئ البنية النظيفة",
            ContentEn = GenerateSampleContent("Clean Architecture is a software design philosophy that separates the elements of a design into ring levels. This separation helps create systems that are independent of frameworks, testable, and maintainable."),
            ContentAr = GenerateSampleContent("البنية النظيفة هي فلسفة تصميم برمجيات تفصل عناصر التصميم إلى مستويات حلقية. يساعد هذا الفصل في إنشاء أنظمة مستقلة عن الأطر، قابلة للاختبار، وسهلة الصيانة."),
            ExcerptEn = "Learn the fundamental principles of Clean Architecture",
            ExcerptAr = "تعلم المبادئ الأساسية للبنية النظيفة",
            FeaturedImageUrl = "https://picsum.photos/seed/post2/800/400",
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-8),
            ViewCount = random.Next(150, 600),
            AuthorId = adminUser.Id,
            SeriesId = series[1].Id
        };
        post2.Tags.Add(tags[6]); // Clean Architecture
        post2.Tags.Add(tags[7]); // Design Patterns
        post2.Categories.Add(categories[0]); // Tutorials
        blogPosts.Add(post2);

        // Post 3 - Featured
        var post3 = new BlogPost
        {
            Slug = "building-real-time-apps-signalr",
            TitleEn = "Building Real-Time Apps with SignalR",
            TitleAr = "بناء تطبيقات الوقت الفعلي باستخدام SignalR",
            ContentEn = GenerateSampleContent("SignalR is a library for ASP.NET that simplifies adding real-time web functionality to applications. Real-time web functionality enables server-side code to push content to clients instantly."),
            ContentAr = GenerateSampleContent("SignalR هي مكتبة لـ ASP.NET تبسط إضافة وظائف الويب في الوقت الفعلي إلى التطبيقات. تمكن وظائف الويب في الوقت الفعلي كود الخادم من دفع المحتوى إلى العملاء على الفور."),
            ExcerptEn = "Master real-time communication with SignalR",
            ExcerptAr = "أتقن الاتصال في الوقت الفعلي باستخدام SignalR",
            FeaturedImageUrl = "https://picsum.photos/seed/post3/800/400",
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-6),
            ViewCount = random.Next(200, 700),
            AuthorId = authorUser2.Id
        };
        post3.Tags.Add(tags[3]); // ASP.NET Core
        post3.Tags.Add(tags[2]); // Web Development
        post3.Categories.Add(categories[0]); // Tutorials
        blogPosts.Add(post3);

        // Add more posts (7 more to reach 10 total)
        for (int i = 4; i <= 10; i++)
        {
            var author = i % 3 == 0 ? adminUser : (i % 2 == 0 ? authorUser1 : authorUser2);
            var post = new BlogPost
            {
                Slug = $"sample-blog-post-{i}",
                TitleEn = $"Sample Blog Post {i}",
                TitleAr = $"مقالة مدونة نموذجية {i}",
                ContentEn = GenerateSampleContent($"This is sample blog post number {i}. It contains interesting content about web development and technology."),
                ContentAr = GenerateSampleContent($"هذه مقالة مدونة نموذجية رقم {i}. تحتوي على محتوى مثير للاهتمام حول تطوير الويب والتكنولوجيا."),
                ExcerptEn = $"Sample excerpt for blog post {i}",
                ExcerptAr = $"مقتطف نموذجي لمقالة المدونة {i}",
                FeaturedImageUrl = $"https://picsum.photos/seed/post{i}/800/400",
                IsFeatured = false,
                PublishedAt = DateTime.UtcNow.AddDays(-i),
                ViewCount = random.Next(50, 300),
                AuthorId = author.Id
            };

            // Add random tags and categories
            post.Tags.Add(tags[random.Next(tags.Count)]);
            post.Tags.Add(tags[random.Next(tags.Count)]);
            post.Categories.Add(categories[random.Next(categories.Count)]);

            blogPosts.Add(post);
        }

        await _context.BlogPosts.AddRangeAsync(blogPosts);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} blog posts", blogPosts.Count);

        // Seed comments and reactions
        await SeedCommentsAndReactionsAsync(blogPosts, adminUser, authorUser1, authorUser2);
    }

    private async Task SeedCommentsAndReactionsAsync(List<BlogPost> blogPosts, User adminUser, User authorUser1, User authorUser2)
    {
        var random = new Random();
        var users = new[] { adminUser, authorUser1, authorUser2 };

        foreach (var post in blogPosts.Take(5)) // Add comments to first 5 posts
        {
            // Add 2-4 comments per post
            var commentCount = random.Next(2, 5);
            for (int i = 0; i < commentCount; i++)
            {
                var user = users[random.Next(users.Length)];
                var comment = new Comment
                {
                    BlogPostId = post.Id,
                    UserId = user.Id,
                    Content = $"This is a great post! I learned a lot from reading it. Comment {i + 1}",
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 5))
                };

                await _context.Comments.AddAsync(comment);
            }

            // Add reactions
            foreach (var user in users)
            {
                var reactionTypes = Enum.GetValues<ReactionType>();
                var reaction = new Reaction
                {
                    BlogPostId = post.Id,
                    UserId = user.Id,
                    ReactionType = reactionTypes[random.Next(reactionTypes.Length)],
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 5))
                };

                await _context.Reactions.AddAsync(reaction);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Created comments and reactions for blog posts");
    }

    private string GenerateSampleContent(string intro)
    {
        return $@"{intro}

## Key Features

This section covers the main features and capabilities that make this topic important for modern web development.

### Feature 1: Performance

Performance is crucial for any web application. We'll explore various optimization techniques and best practices.

### Feature 2: Scalability

Learn how to build applications that can scale to handle millions of users without compromising performance.

### Feature 3: Security

Security should never be an afterthought. We'll discuss common vulnerabilities and how to protect against them.

## Getting Started

To get started with this technology, you'll need to have the following prerequisites installed:

1. .NET 9 SDK
2. Visual Studio 2024 or VS Code
3. SQL Server or PostgreSQL

## Code Example

```csharp
public class Example
{{
    public async Task<IActionResult> Index()
    {{
        var data = await _service.GetDataAsync();
        return View(data);
    }}
}}
```

## Conclusion

This is just the beginning of your journey. Keep learning and experimenting with new technologies to stay ahead in the ever-evolving world of web development.

## Resources

- Official Documentation
- Community Forums
- GitHub Repository
- Video Tutorials

Happy coding!";
    }
}
