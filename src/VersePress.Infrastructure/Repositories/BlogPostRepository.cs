using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;
using VersePress.Infrastructure.Data;

namespace VersePress.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for BlogPost entities with specialized query methods.
/// </summary>
public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
{
    /// <summary>
    /// Initializes a new instance of the BlogPostRepository class.
    /// </summary>
    /// <param name="context">Application database context</param>
    public BlogPostRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<BlogPost?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Include(p => p.Categories)
            .Include(p => p.Series)
            .Include(p => p.Project)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page, int pageSize)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Include(p => p.Categories)
            .Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BlogPost>> GetFeaturedPostsAsync(int count)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Include(p => p.Categories)
            .Where(p => p.IsFeatured && p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow)
            .OrderByDescending(p => p.PublishedAt)
            .Take(count)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BlogPost>> GetPostsByAuthorAsync(Guid authorId)
    {
        return await _dbSet
            .Include(p => p.Tags)
            .Include(p => p.Categories)
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BlogPost>> SearchPostsAsync(string query)
    {
        var lowerQuery = query.ToLower();
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Include(p => p.Categories)
            .Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow &&
                       (p.TitleEn.ToLower().Contains(lowerQuery) ||
                        p.TitleAr.ToLower().Contains(lowerQuery) ||
                        p.ContentEn.ToLower().Contains(lowerQuery) ||
                        p.ContentAr.ToLower().Contains(lowerQuery)))
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _dbSet.AnyAsync(p => p.Slug == slug);
    }
}
