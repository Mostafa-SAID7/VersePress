using System.Text;
using System.Xml;
using VersePress.Application.DTOs;
using VersePress.Application.Interfaces;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for SEO operations including meta tags, structured data, sitemap, and RSS feed generation
/// </summary>
public class SeoService : ISeoService
{
    private readonly IUnitOfWork _unitOfWork;

    public SeoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<MetaTagsDto> GenerateMetaTagsAsync(Guid blogPostId, string language, string baseUrl)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(blogPostId);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {blogPostId} not found.");
        }

        var isEnglish = language.StartsWith("en", StringComparison.OrdinalIgnoreCase);
        var title = isEnglish ? blogPost.TitleEn : blogPost.TitleAr;
        var description = isEnglish 
            ? (blogPost.ExcerptEn ?? TruncateContent(blogPost.ContentEn, 160))
            : (blogPost.ExcerptAr ?? TruncateContent(blogPost.ContentAr, 160));

        // Generate keywords from tags
        var keywords = string.Join(", ", blogPost.Tags?.Select(t => isEnglish ? t.NameEn : t.NameAr) ?? Enumerable.Empty<string>());

        var canonicalUrl = $"{baseUrl.TrimEnd('/')}/blog/{blogPost.Slug}";
        var alternateLanguage = isEnglish ? "ar" : "en";
        var alternateUrl = $"{baseUrl.TrimEnd('/')}/{alternateLanguage}/blog/{blogPost.Slug}";

        return new MetaTagsDto
        {
            Title = title,
            Description = description,
            Keywords = keywords,
            CanonicalUrl = canonicalUrl,
            ImageUrl = blogPost.FeaturedImageUrl,
            Language = language,
            AlternateLanguage = alternateLanguage,
            AlternateUrl = alternateUrl
        };
    }

    public async Task<OpenGraphDto> GenerateOpenGraphTagsAsync(Guid blogPostId, string language, string baseUrl)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(blogPostId);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {blogPostId} not found.");
        }

        var isEnglish = language.StartsWith("en", StringComparison.OrdinalIgnoreCase);
        var title = isEnglish ? blogPost.TitleEn : blogPost.TitleAr;
        var description = isEnglish 
            ? (blogPost.ExcerptEn ?? TruncateContent(blogPost.ContentEn, 200))
            : (blogPost.ExcerptAr ?? TruncateContent(blogPost.ContentAr, 200));

        var url = $"{baseUrl.TrimEnd('/')}/blog/{blogPost.Slug}";
        var locale = isEnglish ? "en_US" : "ar_SA";
        var alternateLocale = isEnglish ? "ar_SA" : "en_US";

        var tags = blogPost.Tags?.Select(t => isEnglish ? t.NameEn : t.NameAr).ToList() ?? new List<string>();

        return new OpenGraphDto
        {
            Title = title,
            Description = description,
            Type = "article",
            Url = url,
            ImageUrl = blogPost.FeaturedImageUrl,
            SiteName = "VersePress",
            Locale = locale,
            AlternateLocale = alternateLocale,
            PublishedTime = blogPost.PublishedAt,
            ModifiedTime = blogPost.UpdatedAt,
            AuthorName = blogPost.Author?.UserName,
            Tags = tags
        };
    }

    public async Task<JsonLdDto> GenerateJsonLdAsync(Guid blogPostId, string language, string baseUrl)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(blogPostId);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {blogPostId} not found.");
        }

        var isEnglish = language.StartsWith("en", StringComparison.OrdinalIgnoreCase);
        var headline = isEnglish ? blogPost.TitleEn : blogPost.TitleAr;
        var description = isEnglish 
            ? (blogPost.ExcerptEn ?? TruncateContent(blogPost.ContentEn, 200))
            : (blogPost.ExcerptAr ?? TruncateContent(blogPost.ContentAr, 200));

        var mainEntityOfPage = $"{baseUrl.TrimEnd('/')}/blog/{blogPost.Slug}";

        return new JsonLdDto
        {
            Context = "https://schema.org",
            Type = "BlogPosting",
            Headline = headline,
            Description = description,
            Image = blogPost.FeaturedImageUrl,
            DatePublished = blogPost.PublishedAt,
            DateModified = blogPost.UpdatedAt,
            Author = new JsonLdDto.AuthorDto
            {
                Type = "Person",
                Name = blogPost.Author?.UserName ?? "Unknown"
            },
            Publisher = new JsonLdDto.PublisherDto
            {
                Type = "Organization",
                Name = "VersePress",
                Logo = new JsonLdDto.LogoDto
                {
                    Type = "ImageObject",
                    Url = $"{baseUrl.TrimEnd('/')}/logo.png"
                }
            },
            MainEntityOfPage = mainEntityOfPage
        };
    }

    public async Task<string> GenerateSitemapAsync(string baseUrl)
    {
        var publishedPosts = await _unitOfWork.BlogPosts.GetPublishedPostsAsync(1, int.MaxValue);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"");
        sb.AppendLine("        xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

        // Add homepage
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{baseUrl.TrimEnd('/')}/</loc>");
        sb.AppendLine("    <changefreq>daily</changefreq>");
        sb.AppendLine("    <priority>1.0</priority>");
        sb.AppendLine("  </url>");

        // Add blog posts with bilingual support
        foreach (var post in publishedPosts)
        {
            var postUrl = $"{baseUrl.TrimEnd('/')}/blog/{post.Slug}";
            var lastMod = post.UpdatedAt?.ToString("yyyy-MM-dd") ?? post.PublishedAt?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd");

            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{postUrl}</loc>");
            sb.AppendLine($"    <lastmod>{lastMod}</lastmod>");
            sb.AppendLine("    <changefreq>weekly</changefreq>");
            sb.AppendLine("    <priority>0.8</priority>");
            
            // Add hreflang tags for bilingual support
            sb.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"en\" href=\"{baseUrl.TrimEnd('/')}/en/blog/{post.Slug}\" />");
            sb.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"ar\" href=\"{baseUrl.TrimEnd('/')}/ar/blog/{post.Slug}\" />");
            
            sb.AppendLine("  </url>");
        }

        sb.AppendLine("</urlset>");

        return sb.ToString();
    }

    public async Task<string> GenerateRssFeedAsync(string baseUrl, int count = 20)
    {
        var publishedPosts = await _unitOfWork.BlogPosts.GetPublishedPostsAsync(1, count);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\">");
        sb.AppendLine("  <channel>");
        sb.AppendLine("    <title>VersePress Blog</title>");
        sb.AppendLine($"    <link>{baseUrl.TrimEnd('/')}</link>");
        sb.AppendLine("    <description>Latest blog posts from VersePress - A bilingual blog platform</description>");
        sb.AppendLine("    <language>en</language>");
        sb.AppendLine($"    <lastBuildDate>{DateTime.UtcNow:R}</lastBuildDate>");
        sb.AppendLine($"    <atom:link href=\"{baseUrl.TrimEnd('/')}/rss\" rel=\"self\" type=\"application/rss+xml\" />");

        foreach (var post in publishedPosts)
        {
            var postUrl = $"{baseUrl.TrimEnd('/')}/blog/{post.Slug}";
            var pubDate = post.PublishedAt?.ToString("R") ?? DateTime.UtcNow.ToString("R");
            var description = XmlEscape(post.ExcerptEn ?? TruncateContent(post.ContentEn, 300));
            var title = XmlEscape(post.TitleEn);

            sb.AppendLine("    <item>");
            sb.AppendLine($"      <title>{title}</title>");
            sb.AppendLine($"      <link>{postUrl}</link>");
            sb.AppendLine($"      <guid isPermaLink=\"true\">{postUrl}</guid>");
            sb.AppendLine($"      <pubDate>{pubDate}</pubDate>");
            sb.AppendLine($"      <description>{description}</description>");
            
            if (!string.IsNullOrEmpty(post.Author?.UserName))
            {
                sb.AppendLine($"      <author>{XmlEscape(post.Author.UserName)}</author>");
            }

            // Add categories
            if (post.Categories != null)
            {
                foreach (var category in post.Categories)
                {
                    sb.AppendLine($"      <category>{XmlEscape(category.NameEn)}</category>");
                }
            }

            sb.AppendLine("    </item>");
        }

        sb.AppendLine("  </channel>");
        sb.AppendLine("</rss>");

        return sb.ToString();
    }

    /// <summary>
    /// Truncates content to specified length and adds ellipsis
    /// </summary>
    private string TruncateContent(string content, int maxLength)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        // Remove HTML tags if present
        var plainText = System.Text.RegularExpressions.Regex.Replace(content, "<.*?>", string.Empty);

        if (plainText.Length <= maxLength)
        {
            return plainText;
        }

        // Truncate at word boundary
        var truncated = plainText.Substring(0, maxLength);
        var lastSpace = truncated.LastIndexOf(' ');
        
        if (lastSpace > 0)
        {
            truncated = truncated.Substring(0, lastSpace);
        }

        return truncated + "...";
    }

    /// <summary>
    /// Escapes special XML characters
    /// </summary>
    private string XmlEscape(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}
