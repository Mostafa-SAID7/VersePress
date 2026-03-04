using Microsoft.Extensions.Caching.Memory;
using VersePress.Application.Interfaces;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for tracking unique blog post views with session-based deduplication.
/// Uses in-memory cache for performance and database for persistence.
/// </summary>
public class ViewCounterService : IViewCounterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan ViewWindow = TimeSpan.FromHours(24);

    public ViewCounterService(IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<bool> IncrementViewCountAsync(Guid blogPostId, string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Session ID cannot be null or empty.", nameof(sessionId));
        }

        // Check if already viewed recently (cache first for performance)
        var cacheKey = GetCacheKey(blogPostId, sessionId);
        if (_cache.TryGetValue(cacheKey, out _))
        {
            return false; // Already counted within 24 hours
        }

        // Check database for existing view within 24-hour window
        if (await HasViewedRecentlyAsync(blogPostId, sessionId))
        {
            // Cache the result to avoid repeated database queries
            _cache.Set(cacheKey, true, ViewWindow);
            return false;
        }

        // Record the view synchronously to avoid context issues
        try
        {
            await RecordViewAsync(blogPostId, sessionId);
            
            // Cache the view to prevent duplicate counting
            _cache.Set(cacheKey, true, ViewWindow);
            
            return true;
        }
        catch (Exception)
        {
            // Log error but don't throw - view counting should not break page rendering
            // Cache anyway to prevent repeated failures
            _cache.Set(cacheKey, true, ViewWindow);
            return false;
        }
    }

    public async Task<bool> HasViewedRecentlyAsync(Guid blogPostId, string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return false;
        }

        // Check cache first
        var cacheKey = GetCacheKey(blogPostId, sessionId);
        if (_cache.TryGetValue(cacheKey, out _))
        {
            return true;
        }

        // Query database for views within 24-hour window
        var cutoffTime = DateTime.UtcNow.Subtract(ViewWindow);
        var postViews = await _unitOfWork.PostViews.GetAllAsync();
        
        var hasViewed = postViews.Any(pv => 
            pv.BlogPostId == blogPostId && 
            pv.SessionId == sessionId && 
            pv.ViewedAt >= cutoffTime);

        return hasViewed;
    }

    /// <summary>
    /// Records a view in the database and increments the blog post view count.
    /// </summary>
    private async Task RecordViewAsync(Guid blogPostId, string sessionId)
    {
        // Create PostView record
        var postView = new PostView
        {
            BlogPostId = blogPostId,
            SessionId = sessionId,
            ViewedAt = DateTime.UtcNow
        };

        await _unitOfWork.PostViews.AddAsync(postView);

        // Increment the blog post view count
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(blogPostId);
        if (blogPost != null)
        {
            blogPost.ViewCount++;
            await _unitOfWork.BlogPosts.UpdateAsync(blogPost);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Generates a cache key for a blog post and session combination.
    /// </summary>
    private static string GetCacheKey(Guid blogPostId, string sessionId)
    {
        return $"PostView_{blogPostId}_{sessionId}";
    }
}
