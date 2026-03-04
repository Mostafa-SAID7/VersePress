using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Seeds;

/// <summary>
/// Seeds tech-focused tags for blog posts
/// </summary>
public class TagSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TagSeeder> _logger;

    public TagSeeder(ApplicationDbContext context, ILogger<TagSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Tag>> SeedAsync()
    {
        if (await _context.Tags.AnyAsync())
        {
            _logger.LogInformation("Tags already exist, skipping seeding");
            return await _context.Tags.ToListAsync();
        }

        var tags = new List<Tag>
        {
            // AI & Machine Learning
            new() { NameEn = "Artificial Intelligence", NameAr = "الذكاء الاصطناعي", Slug = "artificial-intelligence" },
            new() { NameEn = "Machine Learning", NameAr = "تعلم الآلة", Slug = "machine-learning" },
            new() { NameEn = "Deep Learning", NameAr = "التعلم العميق", Slug = "deep-learning" },
            new() { NameEn = "Neural Networks", NameAr = "الشبكات العصبية", Slug = "neural-networks" },
            new() { NameEn = "LLM", NameAr = "نماذج اللغة الكبيرة", Slug = "llm" },
            
            // Cloud Computing
            new() { NameEn = "Cloud Computing", NameAr = "الحوسبة السحابية", Slug = "cloud-computing" },
            new() { NameEn = "Azure", NameAr = "أزور", Slug = "azure" },
            new() { NameEn = "AWS", NameAr = "أمازون ويب سيرفيسز", Slug = "aws" },
            new() { NameEn = "Google Cloud", NameAr = "جوجل كلاود", Slug = "google-cloud" },
            new() { NameEn = "Serverless", NameAr = "بدون خادم", Slug = "serverless" },
            
            // DevOps & Infrastructure
            new() { NameEn = "DevOps", NameAr = "DevOps", Slug = "devops" },
            new() { NameEn = "Docker", NameAr = "دوكر", Slug = "docker" },
            new() { NameEn = "Kubernetes", NameAr = "كوبرنيتس", Slug = "kubernetes" },
            new() { NameEn = "CI/CD", NameAr = "التكامل والنشر المستمر", Slug = "cicd" },
            new() { NameEn = "Infrastructure as Code", NameAr = "البنية التحتية كرمز", Slug = "infrastructure-as-code" },
            
            // Web Development
            new() { NameEn = "Web Development", NameAr = "تطوير الويب", Slug = "web-development" },
            new() { NameEn = "ASP.NET Core", NameAr = "ASP.NET Core", Slug = "aspnet-core" },
            new() { NameEn = "React", NameAr = "React", Slug = "react" },
            new() { NameEn = "Angular", NameAr = "Angular", Slug = "angular" },
            new() { NameEn = "Vue.js", NameAr = "Vue.js", Slug = "vuejs" },
            new() { NameEn = "Next.js", NameAr = "Next.js", Slug = "nextjs" },
            
            // Programming Languages
            new() { NameEn = "C#", NameAr = "C#", Slug = "csharp" },
            new() { NameEn = "JavaScript", NameAr = "JavaScript", Slug = "javascript" },
            new() { NameEn = "TypeScript", NameAr = "TypeScript", Slug = "typescript" },
            new() { NameEn = "Python", NameAr = "Python", Slug = "python" },
            new() { NameEn = "Go", NameAr = "Go", Slug = "golang" },
            new() { NameEn = "Rust", NameAr = "Rust", Slug = "rust" },
            
            // Mobile Development
            new() { NameEn = "Mobile Development", NameAr = "تطوير تطبيقات الجوال", Slug = "mobile-development" },
            new() { NameEn = "iOS", NameAr = "iOS", Slug = "ios" },
            new() { NameEn = "Android", NameAr = "Android", Slug = "android" },
            new() { NameEn = "React Native", NameAr = "React Native", Slug = "react-native" },
            new() { NameEn = "Flutter", NameAr = "Flutter", Slug = "flutter" },
            
            // Cybersecurity
            new() { NameEn = "Cybersecurity", NameAr = "الأمن السيبراني", Slug = "cybersecurity" },
            new() { NameEn = "Security", NameAr = "الأمان", Slug = "security" },
            new() { NameEn = "Encryption", NameAr = "التشفير", Slug = "encryption" },
            new() { NameEn = "Zero Trust", NameAr = "الثقة المعدومة", Slug = "zero-trust" },
            
            // Web3 & Blockchain
            new() { NameEn = "Web3", NameAr = "ويب 3", Slug = "web3" },
            new() { NameEn = "Blockchain", NameAr = "بلوكتشين", Slug = "blockchain" },
            new() { NameEn = "Cryptocurrency", NameAr = "العملات الرقمية", Slug = "cryptocurrency" },
            new() { NameEn = "Smart Contracts", NameAr = "العقود الذكية", Slug = "smart-contracts" },
            
            // Architecture & Patterns
            new() { NameEn = "Microservices", NameAr = "الخدمات المصغرة", Slug = "microservices" },
            new() { NameEn = "Clean Architecture", NameAr = "البنية النظيفة", Slug = "clean-architecture" },
            new() { NameEn = "Design Patterns", NameAr = "أنماط التصميم", Slug = "design-patterns" },
            new() { NameEn = "Domain-Driven Design", NameAr = "التصميم الموجه بالمجال", Slug = "domain-driven-design" },
            
            // Databases
            new() { NameEn = "SQL Server", NameAr = "SQL Server", Slug = "sql-server" },
            new() { NameEn = "PostgreSQL", NameAr = "PostgreSQL", Slug = "postgresql" },
            new() { NameEn = "MongoDB", NameAr = "MongoDB", Slug = "mongodb" },
            new() { NameEn = "Redis", NameAr = "Redis", Slug = "redis" },
            
            // General
            new() { NameEn = "Best Practices", NameAr = "أفضل الممارسات", Slug = "best-practices" },
            new() { NameEn = "Performance", NameAr = "الأداء", Slug = "performance" },
            new() { NameEn = "Tutorial", NameAr = "درس تعليمي", Slug = "tutorial" },
            new() { NameEn = "Open Source", NameAr = "مفتوح المصدر", Slug = "open-source" }
        };

        await _context.Tags.AddRangeAsync(tags);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} tech-focused tags", tags.Count);

        return tags;
    }
}
