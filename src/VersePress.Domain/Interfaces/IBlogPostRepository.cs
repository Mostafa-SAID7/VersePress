using VersePress.Domain.Entities;

namespace VersePress.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for BlogPost entity with domain-specific queries
/// </summary>
public interface IBlogPostRepository : IRepository<BlogPost>
{
    /// <summary>
    /// Retrieves a blog post by its URL slug
    /// </summary>
    /// <param name="slug">URL-friendly post identifier</param>
    /// <returns>BlogPost if found, null otherwise</returns>
    Task<BlogPost?> GetBySlugAsync(string slug);

    /// <summary>
    /// Retrieves published blog posts with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of posts per page</param>
    /// <returns>Collection of published blog posts</returns>
    Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page, int pageSize);

    /// <summary>
    /// Retrieves featured blog posts
    /// </summary>
    /// <param name="count">Maximum number of featured posts to retrieve</param>
    /// <returns>Collection of featured blog posts</returns>
    Task<IEnumerable<BlogPost>> GetFeaturedPostsAsync(int count);

    /// <summary>
    /// Retrieves all blog posts authored by a specific user
    /// </summary>
    /// <param name="authorId">Author's user identifier</param>
    /// <returns>Collection of blog posts by the author</returns>
    Task<IEnumerable<BlogPost>> GetPostsByAuthorAsync(Guid authorId);

    /// <summary>
    /// Searches blog posts by query string across titles and content in both languages
    /// </summary>
    /// <param name="query">Search query</param>
    /// <returns>Collection of matching blog posts</returns>
    Task<IEnumerable<BlogPost>> SearchPostsAsync(string query);

    /// <summary>
    /// Checks if a slug already exists in the repository
    /// </summary>
    /// <param name="slug">Slug to check</param>
    /// <returns>True if slug exists, false otherwise</returns>
    Task<bool> SlugExistsAsync(string slug);
}
