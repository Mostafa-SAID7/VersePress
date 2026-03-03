namespace VersePress.Application.Interfaces;

/// <summary>
/// Service for tracking unique blog post views with session-based deduplication.
/// </summary>
public interface IViewCounterService
{
    /// <summary>
    /// Increments the view count for a blog post if the session hasn't viewed it within 24 hours.
    /// </summary>
    /// <param name="blogPostId">The ID of the blog post being viewed</param>
    /// <param name="sessionId">The unique session identifier (from cookie or generated)</param>
    /// <returns>True if the view was counted, false if already counted within 24 hours</returns>
    Task<bool> IncrementViewCountAsync(Guid blogPostId, string sessionId);

    /// <summary>
    /// Checks if a session has already viewed a blog post within the 24-hour window.
    /// </summary>
    /// <param name="blogPostId">The ID of the blog post</param>
    /// <param name="sessionId">The unique session identifier</param>
    /// <returns>True if the session has viewed the post within 24 hours</returns>
    Task<bool> HasViewedRecentlyAsync(Guid blogPostId, string sessionId);
}
