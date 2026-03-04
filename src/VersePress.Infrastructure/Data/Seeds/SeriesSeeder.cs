using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Seeds;

/// <summary>
/// Seeds tech-focused blog post series
/// </summary>
public class SeriesSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SeriesSeeder> _logger;

    public SeriesSeeder(ApplicationDbContext context, ILogger<SeriesSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Series>> SeedAsync()
    {
        if (await _context.Series.AnyAsync())
        {
            _logger.LogInformation("Series already exist, skipping seeding");
            return await _context.Series.ToListAsync();
        }

        var series = new List<Series>
        {
            new()
            {
                NameEn = "AI Revolution 2026",
                NameAr = "ثورة الذكاء الاصطناعي 2026",
                Slug = "ai-revolution-2026",
                DescriptionEn = "Exploring the latest breakthroughs in artificial intelligence and their impact on software development",
                DescriptionAr = "استكشاف أحدث الاختراقات في الذكاء الاصطناعي وتأثيرها على تطوير البرمجيات"
            },
            new()
            {
                NameEn = "Cloud Native Architecture",
                NameAr = "البنية السحابية الأصلية",
                Slug = "cloud-native-architecture",
                DescriptionEn = "Building scalable and resilient applications with cloud-native technologies",
                DescriptionAr = "بناء تطبيقات قابلة للتوسع ومرنة باستخدام التقنيات السحابية الأصلية"
            },
            new()
            {
                NameEn = "Modern Web Development",
                NameAr = "تطوير الويب الحديث",
                Slug = "modern-web-development",
                DescriptionEn = "Mastering the latest web development frameworks and best practices",
                DescriptionAr = "إتقان أحدث أطر تطوير الويب وأفضل الممارسات"
            },
            new()
            {
                NameEn = "DevOps Mastery",
                NameAr = "إتقان DevOps",
                Slug = "devops-mastery",
                DescriptionEn = "Complete guide to DevOps practices, tools, and automation",
                DescriptionAr = "دليل شامل لممارسات وأدوات وأتمتة DevOps"
            },
            new()
            {
                NameEn = "Cybersecurity Essentials",
                NameAr = "أساسيات الأمن السيبراني",
                Slug = "cybersecurity-essentials",
                DescriptionEn = "Essential security practices for modern software development",
                DescriptionAr = "ممارسات الأمان الأساسية لتطوير البرمجيات الحديثة"
            },
            new()
            {
                NameEn = "Web3 & Blockchain Fundamentals",
                NameAr = "أساسيات Web3 والبلوكتشين",
                Slug = "web3-blockchain-fundamentals",
                DescriptionEn = "Understanding decentralized technologies and their applications",
                DescriptionAr = "فهم التقنيات اللامركزية وتطبيقاتها"
            }
        };

        await _context.Series.AddRangeAsync(series);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} tech-focused series", series.Count);

        return series;
    }
}
