using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;

namespace VersePress.Infrastructure.Data.Seeds;

/// <summary>
/// Seeds tech news blog posts with real technology content
/// </summary>
public class BlogPostSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BlogPostSeeder> _logger;

    public BlogPostSeeder(ApplicationDbContext context, ILogger<BlogPostSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(
        User adminUser,
        User authorUser1,
        User authorUser2,
        List<Tag> tags,
        List<Category> categories,
        List<Series> series,
        List<Project> projects)
    {
        if (await _context.BlogPosts.AnyAsync())
        {
            _logger.LogInformation("Blog posts already exist, skipping seeding");
            return;
        }

        var blogPosts = new List<BlogPost>();
        var random = new Random();

        // Post 1: AI/ML Featured Post
        var post1 = CreateAIPost(authorUser1, tags, categories, series);
        blogPosts.Add(post1);

        // Post 2: Cloud Computing Featured Post
        var post2 = CreateCloudPost(authorUser2, tags, categories, series);
        blogPosts.Add(post2);

        // Post 3: DevOps Featured Post
        var post3 = CreateDevOpsPost(adminUser, tags, categories, projects);
        blogPosts.Add(post3);

        // Post 4: Cybersecurity
        var post4 = CreateCybersecurityPost(authorUser1, tags, categories);
        blogPosts.Add(post4);

        // Post 5: Web3/Blockchain
        var post5 = CreateWeb3Post(authorUser2, tags, categories, series);
        blogPosts.Add(post5);

        // Post 6: Mobile Development
        var post6 = CreateMobilePost(adminUser, tags, categories);
        blogPosts.Add(post6);

        // Post 7: ASP.NET Core
        var post7 = CreateAspNetPost(authorUser1, tags, categories, projects);
        blogPosts.Add(post7);

        // Post 8: Microservices
        var post8 = CreateMicroservicesPost(authorUser2, tags, categories, series);
        blogPosts.Add(post8);

        // Post 9: Performance Optimization
        var post9 = CreatePerformancePost(adminUser, tags, categories);
        blogPosts.Add(post9);

        // Post 10: Open Source
        var post10 = CreateOpenSourcePost(authorUser1, tags, categories, projects);
        blogPosts.Add(post10);

        await _context.BlogPosts.AddRangeAsync(blogPosts);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created {Count} tech news blog posts", blogPosts.Count);

        // Seed comments and reactions
        await SeedCommentsAndReactionsAsync(blogPosts, adminUser, authorUser1, authorUser2);
    }

    private BlogPost CreateAIPost(User author, List<Tag> tags, List<Category> categories, List<Series> series)
    {
        var post = new BlogPost
        {
            Slug = "gpt-5-breakthrough-multimodal-ai",
            TitleEn = "GPT-5 Breakthrough: The Next Generation of Multimodal AI",
            TitleAr = "اختراق GPT-5: الجيل القادم من الذكاء الاصطناعي متعدد الوسائط",
            ContentEn = @"# GPT-5: A New Era in Artificial Intelligence

OpenAI has unveiled GPT-5, marking a significant leap forward in artificial intelligence capabilities. This latest iteration brings unprecedented improvements in multimodal understanding, reasoning, and real-time processing.

## Key Features

### Enhanced Multimodal Capabilities
GPT-5 seamlessly processes and generates content across text, images, audio, and video. The model demonstrates remarkable understanding of context across different media types, enabling more natural and intuitive interactions.

### Advanced Reasoning
The new architecture incorporates improved chain-of-thought reasoning, allowing the model to break down complex problems and provide step-by-step solutions with greater accuracy.

### Real-Time Processing
With optimized inference engines, GPT-5 delivers responses 3x faster than its predecessor while maintaining higher quality outputs.

## Impact on Software Development

Developers are already leveraging GPT-5 for:
- Automated code review and optimization
- Natural language to code generation
- Intelligent debugging assistance
- Documentation generation

## Getting Started

```csharp
// Example: Using GPT-5 API in C#
var client = new OpenAIClient(apiKey);
var response = await client.CompleteChatAsync(new ChatRequest
{
    Model = ""gpt-5"",
    Messages = new[]
    {
        new Message { Role = ""user"", Content = ""Explain Clean Architecture"" }
    }
});
```

## Conclusion

GPT-5 represents a paradigm shift in how we interact with AI systems. As developers, understanding and leveraging these capabilities will be crucial for building the next generation of intelligent applications.",
            ContentAr = @"# GPT-5: عصر جديد في الذكاء الاصطناعي

كشفت OpenAI عن GPT-5، مما يمثل قفزة كبيرة إلى الأمام في قدرات الذكاء الاصطناعي. يجلب هذا الإصدار الأحدث تحسينات غير مسبوقة في الفهم متعدد الوسائط والاستدلال والمعالجة في الوقت الفعلي.

## الميزات الرئيسية

### قدرات محسنة متعددة الوسائط
يعالج GPT-5 بسلاسة وينشئ محتوى عبر النص والصور والصوت والفيديو. يُظهر النموذج فهمًا ملحوظًا للسياق عبر أنواع الوسائط المختلفة، مما يتيح تفاعلات أكثر طبيعية وبديهية.

### استدلال متقدم
تتضمن البنية الجديدة استدلالًا محسنًا لسلسلة الأفكار، مما يسمح للنموذج بتقسيم المشاكل المعقدة وتقديم حلول خطوة بخطوة بدقة أكبر.

### معالجة في الوقت الفعلي
مع محركات الاستدلال المحسنة، يقدم GPT-5 استجابات أسرع 3 مرات من سابقه مع الحفاظ على مخرجات ذات جودة أعلى.

## التأثير على تطوير البرمجيات

يستفيد المطورون بالفعل من GPT-5 في:
- مراجعة الكود الآلية والتحسين
- توليد الكود من اللغة الطبيعية
- مساعدة التصحيح الذكية
- توليد الوثائق

## البدء

```csharp
// مثال: استخدام GPT-5 API في C#
var client = new OpenAIClient(apiKey);
var response = await client.CompleteChatAsync(new ChatRequest
{
    Model = ""gpt-5"",
    Messages = new[]
    {
        new Message { Role = ""user"", Content = ""اشرح البنية النظيفة"" }
    }
});
```

## الخلاصة

يمثل GPT-5 تحولًا نموذجيًا في كيفية تفاعلنا مع أنظمة الذكاء الاصطناعي. كمطورين، سيكون فهم واستغلال هذه القدرات أمرًا حاسمًا لبناء الجيل القادم من التطبيقات الذكية.",
            ExcerptEn = "OpenAI's GPT-5 brings revolutionary multimodal AI capabilities with 3x faster processing and enhanced reasoning",
            ExcerptAr = "يجلب GPT-5 من OpenAI قدرات ذكاء اصطناعي متعددة الوسائط ثورية مع معالجة أسرع 3 مرات واستدلال محسن",
            FeaturedImageUrl = "https://picsum.photos/seed/gpt5/800/400",
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-2),
            ViewCount = 1247,
            AuthorId = author.Id,
            SeriesId = series.First(s => s.Slug == "ai-revolution-2026").Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "artificial-intelligence"));
        post.Tags.Add(tags.First(t => t.Slug == "machine-learning"));
        post.Tags.Add(tags.First(t => t.Slug == "llm"));
        post.Categories.Add(categories.First(c => c.Slug == "breaking-news"));
        post.Categories.Add(categories.First(c => c.Slug == "deep-dive"));

        return post;
    }

    private BlogPost CreateCloudPost(User author, List<Tag> tags, List<Category> categories, List<Series> series)
    {
        var post = new BlogPost
        {
            Slug = "azure-container-apps-serverless-kubernetes",
            TitleEn = "Azure Container Apps: Serverless Kubernetes Made Simple",
            TitleAr = "Azure Container Apps: Kubernetes بدون خادم بشكل مبسط",
            ContentEn = @"# Simplifying Cloud-Native Development with Azure Container Apps

Microsoft's Azure Container Apps service is revolutionizing how developers deploy containerized applications without the complexity of managing Kubernetes clusters.

## What Are Azure Container Apps?

Azure Container Apps is a fully managed serverless container service that enables you to run microservices and containerized applications on a serverless platform.

## Key Benefits

### Simplified Deployment
- No cluster management required
- Automatic scaling based on HTTP traffic or events
- Built-in load balancing and service discovery

### Cost-Effective
Pay only for the resources your containers consume. Scale to zero when idle to minimize costs.

### Developer-Friendly
```bash
# Deploy a container app with a single command
az containerapp create \
  --name my-app \
  --resource-group my-rg \
  --image myregistry.azurecr.io/myapp:latest \
  --target-port 80 \
  --ingress external
```

## Use Cases

1. **Microservices**: Deploy and manage microservices without Kubernetes complexity
2. **Event-Driven Apps**: Build applications that scale based on events
3. **Background Jobs**: Run scheduled tasks and batch processing
4. **APIs**: Host RESTful APIs with automatic scaling

## Integration with Azure Services

Azure Container Apps seamlessly integrates with:
- Azure Service Bus for messaging
- Azure Cosmos DB for data storage
- Azure Key Vault for secrets management
- Application Insights for monitoring

## Conclusion

Azure Container Apps strikes the perfect balance between simplicity and power, making it an excellent choice for teams wanting container benefits without Kubernetes overhead.",
            ContentAr = @"# تبسيط تطوير التطبيقات السحابية الأصلية مع Azure Container Apps

تُحدث خدمة Azure Container Apps من Microsoft ثورة في كيفية نشر المطورين للتطبيقات الحاوية دون تعقيد إدارة مجموعات Kubernetes.

## ما هي Azure Container Apps؟

Azure Container Apps هي خدمة حاويات بدون خادم مُدارة بالكامل تمكنك من تشغيل الخدمات المصغرة والتطبيقات الحاوية على منصة بدون خادم.

## الفوائد الرئيسية

### نشر مبسط
- لا حاجة لإدارة المجموعات
- توسع تلقائي بناءً على حركة HTTP أو الأحداث
- موازنة تحميل مدمجة واكتشاف الخدمات

### فعال من حيث التكلفة
ادفع فقط مقابل الموارد التي تستهلكها حاوياتك. قم بالتوسع إلى الصفر عند الخمول لتقليل التكاليف.

### صديق للمطورين
```bash
# نشر تطبيق حاوية بأمر واحد
az containerapp create \
  --name my-app \
  --resource-group my-rg \
  --image myregistry.azurecr.io/myapp:latest \
  --target-port 80 \
  --ingress external
```

## حالات الاستخدام

1. **الخدمات المصغرة**: نشر وإدارة الخدمات المصغرة دون تعقيد Kubernetes
2. **التطبيقات الموجهة بالأحداث**: بناء تطبيقات تتوسع بناءً على الأحداث
3. **المهام الخلفية**: تشغيل المهام المجدولة ومعالجة الدفعات
4. **واجهات برمجة التطبيقات**: استضافة واجهات RESTful مع توسع تلقائي

## التكامل مع خدمات Azure

يتكامل Azure Container Apps بسلاسة مع:
- Azure Service Bus للرسائل
- Azure Cosmos DB لتخزين البيانات
- Azure Key Vault لإدارة الأسرار
- Application Insights للمراقبة

## الخلاصة

يحقق Azure Container Apps التوازن المثالي بين البساطة والقوة، مما يجعله خيارًا ممتازًا للفرق التي تريد فوائد الحاويات دون عبء Kubernetes.",
            ExcerptEn = "Deploy containerized applications without Kubernetes complexity using Azure Container Apps serverless platform",
            ExcerptAr = "نشر التطبيقات الحاوية دون تعقيد Kubernetes باستخدام منصة Azure Container Apps بدون خادم",
            FeaturedImageUrl = "https://picsum.photos/seed/azure-containers/800/400",
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-5),
            ViewCount = 892,
            AuthorId = author.Id,
            SeriesId = series.First(s => s.Slug == "cloud-native-architecture").Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "cloud-computing"));
        post.Tags.Add(tags.First(t => t.Slug == "azure"));
        post.Tags.Add(tags.First(t => t.Slug == "docker"));
        post.Tags.Add(tags.First(t => t.Slug == "serverless"));
        post.Categories.Add(categories.First(c => c.Slug == "tutorials"));
        post.Categories.Add(categories.First(c => c.Slug == "how-to"));

        return post;
    }

    private BlogPost CreateDevOpsPost(User author, List<Tag> tags, List<Category> categories, List<Project> projects)
    {
        var post = new BlogPost
        {
            Slug = "github-actions-advanced-cicd-patterns",
            TitleEn = "Advanced CI/CD Patterns with GitHub Actions",
            TitleAr = "أنماط CI/CD المتقدمة مع GitHub Actions",
            ContentEn = @"# Mastering GitHub Actions for Enterprise CI/CD

GitHub Actions has evolved into a powerful CI/CD platform. This guide explores advanced patterns for building robust deployment pipelines.

## Matrix Builds

```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
    dotnet: ['7.0', '8.0', '9.0']
runs-on: ${{ matrix.os }}
```

## Reusable Workflows

Create modular, reusable workflows across repositories:

```yaml
jobs:
  call-workflow:
    uses: org/repo/.github/workflows/deploy.yml@main
    with:
      environment: production
    secrets: inherit
```

## Deployment Strategies

### Blue-Green Deployment
- Zero-downtime deployments
- Instant rollback capability
- Traffic shifting strategies

### Canary Releases
- Gradual rollout to subset of users
- Automated health checks
- Progressive traffic increase

## Security Best Practices

1. Use OIDC for cloud authentication
2. Implement secret scanning
3. Enable dependency review
4. Use environment protection rules

## Conclusion

GitHub Actions provides enterprise-grade CI/CD capabilities with the flexibility to implement sophisticated deployment strategies.",
            ContentAr = @"# إتقان GitHub Actions لـ CI/CD على مستوى المؤسسات

تطورت GitHub Actions لتصبح منصة CI/CD قوية. يستكشف هذا الدليل الأنماط المتقدمة لبناء خطوط نشر قوية.

## بناء المصفوفة

```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
    dotnet: ['7.0', '8.0', '9.0']
runs-on: ${{ matrix.os }}
```

## سير العمل القابل لإعادة الاستخدام

إنشاء سير عمل معياري قابل لإعادة الاستخدام عبر المستودعات.

## استراتيجيات النشر

### النشر الأزرق-الأخضر
- نشر بدون توقف
- قدرة التراجع الفوري
- استراتيجيات تحويل حركة المرور

### إصدارات Canary
- طرح تدريجي لمجموعة فرعية من المستخدمين
- فحوصات صحية آلية
- زيادة تدريجية لحركة المرور

## أفضل ممارسات الأمان

1. استخدام OIDC للمصادقة السحابية
2. تنفيذ فحص الأسرار
3. تمكين مراجعة التبعيات
4. استخدام قواعد حماية البيئة

## الخلاصة

توفر GitHub Actions قدرات CI/CD على مستوى المؤسسات مع المرونة لتنفيذ استراتيجيات نشر متطورة.",
            ExcerptEn = "Learn advanced CI/CD patterns including matrix builds, reusable workflows, and deployment strategies with GitHub Actions",
            ExcerptAr = "تعلم أنماط CI/CD المتقدمة بما في ذلك بناء المصفوفة وسير العمل القابل لإعادة الاستخدام واستراتيجيات النشر مع GitHub Actions",
            FeaturedImageUrl = "https://picsum.photos/seed/github-actions/800/400",
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-7),
            ViewCount = 1056,
            AuthorId = author.Id,
            ProjectId = projects.First(p => p.Slug == "versepress-development").Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "devops"));
        post.Tags.Add(tags.First(t => t.Slug == "cicd"));
        post.Tags.Add(tags.First(t => t.Slug == "best-practices"));
        post.Categories.Add(categories.First(c => c.Slug == "tutorials"));
        post.Categories.Add(categories.First(c => c.Slug == "best-practices"));

        return post;
    }

    private BlogPost CreateCybersecurityPost(User author, List<Tag> tags, List<Category> categories)
    {
        var post = new BlogPost
        {
            Slug = "zero-trust-architecture-implementation",
            TitleEn = "Implementing Zero Trust Architecture in Modern Applications",
            TitleAr = "تنفيذ بنية الثقة المعدومة في التطبيقات الحديثة",
            ContentEn = @"# Zero Trust: Never Trust, Always Verify

Zero Trust Architecture (ZTA) is becoming the security standard for modern applications. This guide covers practical implementation strategies.

## Core Principles

1. **Verify Explicitly**: Always authenticate and authorize
2. **Least Privilege Access**: Limit user access with Just-In-Time and Just-Enough-Access
3. **Assume Breach**: Minimize blast radius and segment access

## Implementation in ASP.NET Core

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

services.AddAuthorization(options =>
{
    options.AddPolicy(""RequireMFA"", policy =>
        policy.RequireClaim(""amr"", ""mfa""));
});
```

## Key Components

- Multi-Factor Authentication (MFA)
- Conditional Access Policies
- Continuous Monitoring
- Micro-Segmentation

## Conclusion

Zero Trust is not a product but a security philosophy that requires continuous implementation and monitoring.",
            ContentAr = @"# الثقة المعدومة: لا تثق أبدًا، تحقق دائمًا

أصبحت بنية الثقة المعدومة (ZTA) معيار الأمان للتطبيقات الحديثة. يغطي هذا الدليل استراتيجيات التنفيذ العملية.

## المبادئ الأساسية

1. **التحقق بشكل صريح**: المصادقة والتفويض دائمًا
2. **الوصول بأقل امتياز**: تحديد وصول المستخدم
3. **افترض الاختراق**: تقليل نطاق الانفجار وتقسيم الوصول

## التنفيذ في ASP.NET Core

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });
```

## المكونات الرئيسية

- المصادقة متعددة العوامل (MFA)
- سياسات الوصول المشروط
- المراقبة المستمرة
- التقسيم الدقيق

## الخلاصة

الثقة المعدومة ليست منتجًا ولكنها فلسفة أمنية تتطلب التنفيذ والمراقبة المستمرة.",
            ExcerptEn = "Practical guide to implementing Zero Trust Architecture with multi-factor authentication and least privilege access",
            ExcerptAr = "دليل عملي لتنفيذ بنية الثقة المعدومة مع المصادقة متعددة العوامل والوصول بأقل امتياز",
            FeaturedImageUrl = "https://picsum.photos/seed/zero-trust/800/400",
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-10),
            ViewCount = 734,
            AuthorId = author.Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "cybersecurity"));
        post.Tags.Add(tags.First(t => t.Slug == "security"));
        post.Tags.Add(tags.First(t => t.Slug == "zero-trust"));
        post.Categories.Add(categories.First(c => c.Slug == "deep-dive"));
        post.Categories.Add(categories.First(c => c.Slug == "best-practices"));

        return post;
    }

    private BlogPost CreateWeb3Post(User author, List<Tag> tags, List<Category> categories, List<Series> series)
    {
        var post = new BlogPost
        {
            Slug = "ethereum-smart-contracts-solidity-2026",
            TitleEn = "Building Secure Smart Contracts with Solidity in 2026",
            TitleAr = "بناء عقود ذكية آمنة باستخدام Solidity في 2026",
            ContentEn = "# Smart Contract Development Best Practices\n\nSolidity development has matured significantly. This guide covers security patterns, gas optimization, and testing strategies for production-ready smart contracts.\n\n## Security First\n\n```solidity\ncontract SecureVault {\n    mapping(address => uint256) private balances;\n    \n    function withdraw(uint256 amount) external {\n        require(balances[msg.sender] >= amount, \"Insufficient balance\");\n        balances[msg.sender] -= amount;\n        (bool success, ) = msg.sender.call{value: amount}(\"\");\n        require(success, \"Transfer failed\");\n    }\n}\n```\n\n## Gas Optimization Techniques\n- Use `calldata` instead of `memory` for external functions\n- Pack storage variables efficiently\n- Batch operations when possible\n\n## Testing with Hardhat\n\n```javascript\ndescribe(\"SecureVault\", function () {\n    it(\"Should handle withdrawals correctly\", async function () {\n        const [owner] = await ethers.getSigners();\n        const vault = await Vault.deploy();\n        await vault.deposit({value: ethers.parseEther(\"1.0\")});\n        await expect(vault.withdraw(ethers.parseEther(\"0.5\")))\n            .to.changeEtherBalance(owner, ethers.parseEther(\"0.5\"));\n    });\n});\n```",
            ContentAr = "# أفضل ممارسات تطوير العقود الذكية\n\nنضج تطوير Solidity بشكل كبير. يغطي هذا الدليل أنماط الأمان وتحسين الغاز واستراتيجيات الاختبار للعقود الذكية الجاهزة للإنتاج.\n\n## الأمان أولاً\n\nاستخدام أنماط الأمان المثبتة لحماية الأموال.\n\n## تقنيات تحسين الغاز\n- استخدام `calldata` بدلاً من `memory`\n- حزم متغيرات التخزين بكفاءة\n- عمليات الدفعات عند الإمكان\n\n## الاختبار مع Hardhat\n\nاختبار شامل لضمان أمان العقد.",
            ExcerptEn = "Master Solidity smart contract development with security patterns, gas optimization, and comprehensive testing",
            ExcerptAr = "أتقن تطوير العقود الذكية Solidity مع أنماط الأمان وتحسين الغاز والاختبار الشامل",
            FeaturedImageUrl = "https://picsum.photos/seed/solidity/800/400",
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-12),
            ViewCount = 623,
            AuthorId = author.Id,
            SeriesId = series.First(s => s.Slug == "web3-blockchain-fundamentals").Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "web3"));
        post.Tags.Add(tags.First(t => t.Slug == "blockchain"));
        post.Tags.Add(tags.First(t => t.Slug == "smart-contracts"));
        post.Categories.Add(categories.First(c => c.Slug == "tutorials"));

        return post;
    }

    private BlogPost CreateMobilePost(User author, List<Tag> tags, List<Category> categories)
    {
        var post = new BlogPost
        {
            Slug = "flutter-3-20-performance-improvements",
            TitleEn = "Flutter 3.20: Revolutionary Performance Improvements",
            TitleAr = "Flutter 3.20: تحسينات أداء ثورية",
            ContentEn = "# Flutter 3.20 Performance Breakthrough\n\nFlutter 3.20 introduces Impeller rendering engine as default, delivering 2x faster rendering and smoother animations.\n\n## Key Improvements\n\n### Impeller Rendering Engine\n- Hardware-accelerated graphics\n- Reduced jank and frame drops\n- Better battery efficiency\n\n### Hot Reload Enhancements\n```dart\nclass MyWidget extends StatelessWidget {\n  @override\n  Widget build(BuildContext context) {\n    return MaterialApp(\n      home: Scaffold(\n        body: Center(\n          child: Text('Hot reload is now 40% faster!'),\n        ),\n      ),\n    );\n  }\n}\n```\n\n## Migration Guide\n\nMost apps work with Impeller out of the box. For custom shaders:\n\n```yaml\nflutter:\n  shaders:\n    - shaders/my_shader.frag\n```",
            ContentAr = "# اختراق أداء Flutter 3.20\n\nيقدم Flutter 3.20 محرك عرض Impeller كإعداد افتراضي، مما يوفر عرضًا أسرع مرتين ورسوم متحركة أكثر سلاسة.\n\n## التحسينات الرئيسية\n\n### محرك عرض Impeller\n- رسومات معجلة بالأجهزة\n- تقليل التقطيع وانخفاض الإطارات\n- كفاءة أفضل للبطارية\n\n## دليل الترحيل\n\nتعمل معظم التطبيقات مع Impeller مباشرة.",
            ExcerptEn = "Flutter 3.20 brings Impeller rendering engine with 2x performance boost and smoother animations",
            ExcerptAr = "يجلب Flutter 3.20 محرك عرض Impeller مع تعزيز أداء 2x ورسوم متحركة أكثر سلاسة",
            FeaturedImageUrl = "https://picsum.photos/seed/flutter/800/400",
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-14),
            ViewCount = 567,
            AuthorId = author.Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "mobile-development"));
        post.Tags.Add(tags.First(t => t.Slug == "flutter"));
        post.Tags.Add(tags.First(t => t.Slug == "performance"));
        post.Categories.Add(categories.First(c => c.Slug == "breaking-news"));

        return post;
    }

    private BlogPost CreateAspNetPost(User author, List<Tag> tags, List<Category> categories, List<Project> projects)
    {
        var post = new BlogPost
        {
            Slug = "aspnet-core-9-minimal-apis-advanced",
            TitleEn = "ASP.NET Core 9: Advanced Minimal APIs Patterns",
            TitleAr = "ASP.NET Core 9: أنماط Minimal APIs المتقدمة",
            ContentEn = "# Mastering Minimal APIs in ASP.NET Core 9\n\nMinimal APIs have evolved into a powerful alternative to controllers. Learn advanced patterns for building production-ready APIs.\n\n## Endpoint Filters\n\n```csharp\napp.MapGet(\"/api/posts/{id}\", async (int id, BlogPostService service) =>\n{\n    var post = await service.GetByIdAsync(id);\n    return post is not null ? Results.Ok(post) : Results.NotFound();\n})\n.AddEndpointFilter<ValidationFilter>()\n.AddEndpointFilter<CachingFilter>();\n```\n\n## Typed Results\n\n```csharp\napp.MapPost(\"/api/posts\", async Task<Results<Created<BlogPost>, ValidationProblem>> \n    (CreatePostRequest request, BlogPostService service) =>\n{\n    if (!request.IsValid())\n        return TypedResults.ValidationProblem(request.Errors);\n    \n    var post = await service.CreateAsync(request);\n    return TypedResults.Created($\"/api/posts/{post.Id}\", post);\n});\n```",
            ContentAr = "# إتقان Minimal APIs في ASP.NET Core 9\n\nتطورت Minimal APIs لتصبح بديلاً قويًا للمتحكمات. تعلم الأنماط المتقدمة لبناء واجهات برمجة تطبيقات جاهزة للإنتاج.\n\n## مرشحات نقاط النهاية\n\nاستخدام المرشحات لإضافة وظائف متقاطعة.\n\n## النتائج المكتوبة\n\nنتائج مكتوبة بقوة لأمان أفضل للنوع.",
            ExcerptEn = "Build production-ready APIs with ASP.NET Core 9 Minimal APIs using endpoint filters and typed results",
            ExcerptAr = "بناء واجهات برمجة تطبيقات جاهزة للإنتاج مع ASP.NET Core 9 Minimal APIs",
            FeaturedImageUrl = "https://picsum.photos/seed/aspnet9/800/400",
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-16),
            ViewCount = 845,
            AuthorId = author.Id,
            ProjectId = projects.First(p => p.Slug == "versepress-development").Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "aspnet-core"));
        post.Tags.Add(tags.First(t => t.Slug == "csharp"));
        post.Tags.Add(tags.First(t => t.Slug == "web-development"));
        post.Categories.Add(categories.First(c => c.Slug == "tutorials"));

        return post;
    }

    private BlogPost CreateMicroservicesPost(User author, List<Tag> tags, List<Category> categories, List<Series> series)
    {
        var post = new BlogPost
        {
            Slug = "microservices-communication-patterns-2026",
            TitleEn = "Microservices Communication Patterns: gRPC vs REST vs Message Queues",
            TitleAr = "أنماط اتصال الخدمات المصغرة: gRPC مقابل REST مقابل قوائم الانتظار",
            ContentEn = "# Choosing the Right Communication Pattern\n\nSelecting the appropriate communication pattern is crucial for microservices success. This guide compares gRPC, REST, and message queues.\n\n## gRPC for High Performance\n\n```csharp\nservice BlogService {\n    rpc GetPost (GetPostRequest) returns (Post);\n    rpc StreamPosts (StreamRequest) returns (stream Post);\n}\n```\n\n**Pros:**\n- Binary protocol (faster than JSON)\n- Built-in streaming\n- Strong typing with Protocol Buffers\n\n## REST for Public APIs\n\n```csharp\n[HttpGet(\"/api/posts/{id}\")]\npublic async Task<ActionResult<BlogPost>> GetPost(int id)\n{\n    var post = await _service.GetByIdAsync(id);\n    return post is not null ? Ok(post) : NotFound();\n}\n```\n\n**Pros:**\n- Universal compatibility\n- Easy debugging\n- Caching support\n\n## Message Queues for Async Operations\n\n```csharp\nawait _bus.Publish(new PostPublishedEvent\n{\n    PostId = post.Id,\n    Title = post.Title,\n    PublishedAt = DateTime.UtcNow\n});\n```\n\n**Pros:**\n- Decoupling\n- Reliability\n- Load leveling",
            ContentAr = "# اختيار نمط الاتصال الصحيح\n\nيعد اختيار نمط الاتصال المناسب أمرًا بالغ الأهمية لنجاح الخدمات المصغرة. يقارن هذا الدليل gRPC و REST وقوائم انتظار الرسائل.\n\n## gRPC للأداء العالي\n\nبروتوكول ثنائي أسرع من JSON.\n\n## REST لواجهات برمجة التطبيقات العامة\n\nتوافق عالمي وسهولة التصحيح.\n\n## قوائم انتظار الرسائل للعمليات غير المتزامنة\n\nفصل وموثوقية وتسوية الحمل.",
            ExcerptEn = "Compare gRPC, REST, and message queues to choose the right communication pattern for your microservices",
            ExcerptAr = "قارن gRPC و REST وقوائم انتظار الرسائل لاختيار نمط الاتصال المناسب للخدمات المصغرة",
            FeaturedImageUrl = "https://picsum.photos/seed/microservices/800/400",
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-18),
            ViewCount = 712,
            AuthorId = author.Id,
            SeriesId = series.First(s => s.Slug == "cloud-native-architecture").Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "microservices"));
        post.Tags.Add(tags.First(t => t.Slug == "aspnet-core"));
        post.Tags.Add(tags.First(t => t.Slug == "best-practices"));
        post.Categories.Add(categories.First(c => c.Slug == "deep-dive"));

        return post;
    }

    private BlogPost CreatePerformancePost(User author, List<Tag> tags, List<Category> categories)
    {
        var post = new BlogPost
        {
            Slug = "dotnet-performance-optimization-techniques",
            TitleEn = ".NET Performance Optimization: From Good to Great",
            TitleAr = "تحسين أداء .NET: من الجيد إلى الممتاز",
            ContentEn = "# Squeezing Every Ounce of Performance\n\nLearn advanced .NET performance optimization techniques used by high-traffic applications.\n\n## Memory Allocation Reduction\n\n```csharp\n// Bad: Allocates new array\npublic string[] GetNames() => _users.Select(u => u.Name).ToArray();\n\n// Good: Uses ArrayPool\npublic void GetNames(Span<string> destination)\n{\n    for (int i = 0; i < _users.Count && i < destination.Length; i++)\n        destination[i] = _users[i].Name;\n}\n```\n\n## Async Optimization\n\n```csharp\n// Use ValueTask for hot paths\npublic ValueTask<User> GetCachedUserAsync(int id)\n{\n    if (_cache.TryGetValue(id, out var user))\n        return new ValueTask<User>(user);\n    \n    return new ValueTask<User>(LoadUserAsync(id));\n}\n```\n\n## Database Query Optimization\n\n```csharp\n// Use compiled queries\nprivate static readonly Func<AppDbContext, int, Task<Post>> GetPostQuery =\n    EF.CompileAsyncQuery((AppDbContext db, int id) =>\n        db.Posts.FirstOrDefault(p => p.Id == id));\n```",
            ContentAr = "# استخراج كل أونصة من الأداء\n\nتعلم تقنيات تحسين أداء .NET المتقدمة المستخدمة من قبل التطبيقات ذات حركة المرور العالية.\n\n## تقليل تخصيص الذاكرة\n\nاستخدام ArrayPool وSpan لتقليل التخصيصات.\n\n## تحسين Async\n\nاستخدام ValueTask للمسارات الساخنة.\n\n## تحسين استعلام قاعدة البيانات\n\nاستخدام الاستعلامات المجمعة.",
            ExcerptEn = "Master advanced .NET performance optimization with memory allocation reduction, async patterns, and query optimization",
            ExcerptAr = "أتقن تحسين أداء .NET المتقدم مع تقليل تخصيص الذاكرة وأنماط async وتحسين الاستعلام",
            FeaturedImageUrl = "https://picsum.photos/seed/performance/800/400",
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-20),
            ViewCount = 934,
            AuthorId = author.Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "csharp"));
        post.Tags.Add(tags.First(t => t.Slug == "performance"));
        post.Tags.Add(tags.First(t => t.Slug == "best-practices"));
        post.Categories.Add(categories.First(c => c.Slug == "deep-dive"));

        return post;
    }

    private BlogPost CreateOpenSourcePost(User author, List<Tag> tags, List<Category> categories, List<Project> projects)
    {
        var post = new BlogPost
        {
            Slug = "contributing-to-open-source-guide",
            TitleEn = "The Developer's Guide to Open Source Contributions",
            TitleAr = "دليل المطور للمساهمة في المصادر المفتوحة",
            ContentEn = "# Making Your First Open Source Contribution\n\nOpen source contributions can accelerate your career. This guide walks you through the process.\n\n## Finding the Right Project\n\n1. Look for \"good first issue\" labels\n2. Check project activity and maintainer responsiveness\n3. Read CONTRIBUTING.md carefully\n\n## Making Quality Contributions\n\n```bash\n# Fork and clone\ngit clone https://github.com/yourusername/project.git\ncd project\n\n# Create feature branch\ngit checkout -b fix/issue-123\n\n# Make changes and commit\ngit add .\ngit commit -m \"fix: resolve issue #123\"\n\n# Push and create PR\ngit push origin fix/issue-123\n```\n\n## PR Best Practices\n\n- Write clear commit messages\n- Add tests for new features\n- Update documentation\n- Respond to feedback promptly\n\n## Building Your Reputation\n\n- Start small, think big\n- Be consistent\n- Help others\n- Document your journey",
            ContentAr = "# إجراء مساهمتك الأولى في المصادر المفتوحة\n\nيمكن أن تسرع مساهمات المصادر المفتوحة من مسيرتك المهنية. يرشدك هذا الدليل خلال العملية.\n\n## العثور على المشروع المناسب\n\n1. ابحث عن تسميات \"good first issue\"\n2. تحقق من نشاط المشروع واستجابة المشرفين\n3. اقرأ CONTRIBUTING.md بعناية\n\n## إجراء مساهمات عالية الجودة\n\nاتبع أفضل ممارسات Git و PR.\n\n## بناء سمعتك\n\n- ابدأ صغيرًا، فكر كبيرًا\n- كن متسقًا\n- ساعد الآخرين\n- وثق رحلتك",
            ExcerptEn = "Complete guide to making meaningful open source contributions and building your developer reputation",
            ExcerptAr = "دليل كامل لإجراء مساهمات مفتوحة المصدر ذات مغزى وبناء سمعتك كمطور",
            FeaturedImageUrl = "https://picsum.photos/seed/opensource/800/400",
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-22),
            ViewCount = 456,
            AuthorId = author.Id,
            ProjectId = projects.First(p => p.Slug == "open-source-contributions").Id
        };

        post.Tags.Add(tags.First(t => t.Slug == "open-source"));
        post.Tags.Add(tags.First(t => t.Slug == "best-practices"));
        post.Categories.Add(categories.First(c => c.Slug == "career-skills"));
        post.Categories.Add(categories.First(c => c.Slug == "how-to"));

        return post;
    }

    private async Task SeedCommentsAndReactionsAsync(List<BlogPost> blogPosts, User adminUser, User authorUser1, User authorUser2)
    {
        var random = new Random();
        var users = new[] { adminUser, authorUser1, authorUser2 };

        var techComments = new[]
        {
            "Great article! This really helped me understand the concept better.",
            "Thanks for sharing this. I've been looking for a solution like this.",
            "Excellent explanation. The code examples are very clear.",
            "This is exactly what I needed for my current project!",
            "Well written and comprehensive. Looking forward to more posts like this.",
            "مقال رائع! ساعدني هذا حقًا في فهم المفهوم بشكل أفضل.",
            "شكرًا على المشاركة. كنت أبحث عن حل مثل هذا.",
            "شرح ممتاز. أمثلة الكود واضحة جدًا.",
            "هذا بالضبط ما احتاجه لمشروعي الحالي!",
            "مكتوب بشكل جيد وشامل. أتطلع إلى المزيد من المقالات مثل هذه."
        };

        foreach (var post in blogPosts.Take(7)) // Add comments to first 7 posts
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
                    Content = techComments[random.Next(techComments.Length)],
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 5))
                };

                await _context.Comments.AddAsync(comment);
            }

            // Add reactions from all users
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
}
