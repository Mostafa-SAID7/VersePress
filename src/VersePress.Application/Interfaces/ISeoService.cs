using VersePress.Application.DTOs;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service interface for SEO operations including meta tags, structured data, sitemap, and RSS feed generation
/// </summary>
public interface ISeoService
{
    /// <summary>
    /// Generates meta tags for a blog post including title, description, keywords, and hreflang tags
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <param name="language">Current language (en or ar)</param>
    /// <param name="baseUrl">Base URL of the application</param>
    /// <returns>Meta tags DTO</returns>
    Task<MetaTagsDto> GenerateMetaTagsAsync(Guid blogPostId, string language, string baseUrl);

    /// <summary>
    /// Generates OpenGraph tags for social media sharing
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <param name="language">Current language (en or ar)</param>
    /// <param name="baseUrl">Base URL of the application</param>
    /// <returns>OpenGraph DTO</returns>
    Task<OpenGraphDto> GenerateOpenGraphTagsAsync(Guid blogPostId, string language, string baseUrl);

    /// <summary>
    /// Generates JSON-LD structured data for search engines
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <param name="language">Current language (en or ar)</param>
    /// <param name="baseUrl">Base URL of the application</param>
    /// <returns>JSON-LD DTO</returns>
    Task<JsonLdDto> GenerateJsonLdAsync(Guid blogPostId, string language, string baseUrl);

    /// <summary>
    /// Generates XML sitemap of all published blog posts
    /// </summary>
    /// <param name="baseUrl">Base URL of the application</param>
    /// <returns>XML sitemap as string</returns>
    Task<string> GenerateSitemapAsync(string baseUrl);

    /// <summary>
    /// Generates RSS feed with latest published blog posts
    /// </summary>
    /// <param name="baseUrl">Base URL of the application</param>
    /// <param name="count">Number of posts to include in feed (default: 20)</param>
    /// <returns>RSS feed XML as string</returns>
    Task<string> GenerateRssFeedAsync(string baseUrl, int count = 20);
}
