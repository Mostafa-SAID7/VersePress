using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Seeds;

/// <summary>
/// Seeds tech projects for blog posts
/// </summary>
public class ProjectSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjectSeeder> _logger;

    public ProjectSeeder(ApplicationDbContext context, ILogger<ProjectSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Project>> SeedAsync()
    {
        if (await _context.Projects.AnyAsync())
        {
            _logger.LogInformation("Projects already exist, skipping seeding");
            return await _context.Projects.ToListAsync();
        }

        var projects = new List<Project>
        {
            new()
            {
                NameEn = "VersePress Development",
                NameAr = "تطوير VersePress",
                Slug = "versepress-development",
                DescriptionEn = "Building a modern bilingual blog platform with ASP.NET Core 9, Clean Architecture, and real-time features",
                DescriptionAr = "بناء منصة مدونة ثنائية اللغة حديثة باستخدام ASP.NET Core 9 والبنية النظيفة والميزات في الوقت الفعلي"
            },
            new()
            {
                NameEn = "Open Source Contributions",
                NameAr = "المساهمات مفتوحة المصدر",
                Slug = "open-source-contributions",
                DescriptionEn = "Journey through contributing to major open source projects and building community",
                DescriptionAr = "رحلة المساهمة في مشاريع مفتوحة المصدر الكبرى وبناء المجتمع"
            },
            new()
            {
                NameEn = "Cloud Migration Journey",
                NameAr = "رحلة الانتقال إلى السحابة",
                Slug = "cloud-migration-journey",
                DescriptionEn = "Documenting the process of migrating enterprise applications to cloud platforms",
                DescriptionAr = "توثيق عملية ترحيل تطبيقات المؤسسات إلى المنصات السحابية"
            },
            new()
            {
                NameEn = "AI-Powered Applications",
                NameAr = "تطبيقات مدعومة بالذكاء الاصطناعي",
                Slug = "ai-powered-applications",
                DescriptionEn = "Building intelligent applications using modern AI and machine learning technologies",
                DescriptionAr = "بناء تطبيقات ذكية باستخدام تقنيات الذكاء الاصطناعي وتعلم الآلة الحديثة"
            }
        };

        await _context.Projects.AddRangeAsync(projects);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} tech projects", projects.Count);

        return projects;
    }
}
