using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Seeds;

/// <summary>
/// Seeds tech news categories for blog posts
/// </summary>
public class CategorySeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategorySeeder> _logger;

    public CategorySeeder(ApplicationDbContext context, ILogger<CategorySeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Category>> SeedAsync()
    {
        if (await _context.Categories.AnyAsync())
        {
            _logger.LogInformation("Categories already exist, skipping seeding");
            return await _context.Categories.ToListAsync();
        }

        var categories = new List<Category>
        {
            new()
            {
                NameEn = "Breaking News",
                NameAr = "أخبار عاجلة",
                Slug = "breaking-news",
                DescriptionEn = "Latest breaking news in technology and software development",
                DescriptionAr = "آخر الأخبار العاجلة في التكنولوجيا وتطوير البرمجيات"
            },
            new()
            {
                NameEn = "Tutorials",
                NameAr = "دروس تعليمية",
                Slug = "tutorials",
                DescriptionEn = "Step-by-step guides and hands-on tutorials",
                DescriptionAr = "أدلة ودروس تعليمية خطوة بخطوة"
            },
            new()
            {
                NameEn = "Deep Dive",
                NameAr = "تحليل معمق",
                Slug = "deep-dive",
                DescriptionEn = "In-depth technical analysis and comprehensive guides",
                DescriptionAr = "تحليل تقني معمق وأدلة شاملة"
            },
            new()
            {
                NameEn = "Opinion",
                NameAr = "رأي",
                Slug = "opinion",
                DescriptionEn = "Expert opinions and perspectives on technology trends",
                DescriptionAr = "آراء الخبراء ووجهات نظر حول اتجاهات التكنولوجيا"
            },
            new()
            {
                NameEn = "How-To",
                NameAr = "كيفية",
                Slug = "how-to",
                DescriptionEn = "Practical how-to guides for developers",
                DescriptionAr = "أدلة عملية للمطورين"
            },
            new()
            {
                NameEn = "Industry Trends",
                NameAr = "اتجاهات الصناعة",
                Slug = "industry-trends",
                DescriptionEn = "Analysis of current and emerging technology trends",
                DescriptionAr = "تحليل الاتجاهات التقنية الحالية والناشئة"
            },
            new()
            {
                NameEn = "Product Reviews",
                NameAr = "مراجعات المنتجات",
                Slug = "product-reviews",
                DescriptionEn = "Reviews of developer tools, frameworks, and platforms",
                DescriptionAr = "مراجعات لأدوات المطورين والأطر والمنصات"
            },
            new()
            {
                NameEn = "Case Studies",
                NameAr = "دراسات حالة",
                Slug = "case-studies",
                DescriptionEn = "Real-world implementation case studies and success stories",
                DescriptionAr = "دراسات حالة للتطبيقات الواقعية وقصص النجاح"
            },
            new()
            {
                NameEn = "Best Practices",
                NameAr = "أفضل الممارسات",
                Slug = "best-practices",
                DescriptionEn = "Industry best practices and coding standards",
                DescriptionAr = "أفضل ممارسات الصناعة ومعايير البرمجة"
            },
            new()
            {
                NameEn = "Career & Skills",
                NameAr = "المهنة والمهارات",
                Slug = "career-skills",
                DescriptionEn = "Career advice and skill development for developers",
                DescriptionAr = "نصائح مهنية وتطوير المهارات للمطورين"
            }
        };

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} tech news categories", categories.Count);

        return categories;
    }
}
