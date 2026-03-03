using VersePress.Domain.Enums;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service for tracking blog post shares to social media platforms.
/// </summary>
public interface IShareTrackingService
{
    /// <summary>
    /// Records a share event for a blog post to a specific platform.
    /// Executes asynchronously without blocking user interaction.
    /// </summary>
    /// <param name="blogPostId">The ID of the blog post being shared</param>
    /// <param name="platform">The social media platform (Twitter, Facebook, LinkedIn, WhatsApp)</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task RecordShareAsync(Guid blogPostId, Platform platform);

    /// <summary>
    /// Gets aggregated share counts by platform for a specific blog post.
    /// </summary>
    /// <param name="blogPostId">The ID of the blog post</param>
    /// <returns>Dictionary mapping each platform to its share count</returns>
    Task<Dictionary<Platform, int>> GetShareCountsAsync(Guid blogPostId);

    /// <summary>
    /// Gets the total share count across all platforms for a specific blog post.
    /// </summary>
    /// <param name="blogPostId">The ID of the blog post</param>
    /// <returns>Total number of shares across all platforms</returns>
    Task<int> GetTotalShareCountAsync(Guid blogPostId);
}
