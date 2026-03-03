using VersePress.Application.DTOs;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service interface for analytics and dashboard statistics
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Gets dashboard statistics including total counts for posts, comments, users, and reactions
    /// </summary>
    Task<DashboardStatsDto> GetDashboardStatsAsync();

    /// <summary>
    /// Gets top posts ranked by view count
    /// </summary>
    /// <param name="count">Number of top posts to retrieve</param>
    Task<IEnumerable<TopPostDto>> GetTopPostsByViewsAsync(int count = 10);

    /// <summary>
    /// Gets top posts ranked by reaction count
    /// </summary>
    /// <param name="count">Number of top posts to retrieve</param>
    Task<IEnumerable<TopPostDto>> GetTopPostsByReactionsAsync(int count = 10);

    /// <summary>
    /// Gets top posts ranked by comment count
    /// </summary>
    /// <param name="count">Number of top posts to retrieve</param>
    Task<IEnumerable<TopPostDto>> GetTopPostsByCommentsAsync(int count = 10);

    /// <summary>
    /// Gets recent share events with platform breakdown
    /// </summary>
    /// <param name="count">Number of recent shares to retrieve</param>
    Task<IEnumerable<RecentShareDto>> GetRecentSharesAsync(int count = 20);

    /// <summary>
    /// Gets publication trends over time for chart data
    /// </summary>
    /// <param name="days">Number of days to include in the trend</param>
    Task<IEnumerable<PublicationTrendDto>> GetPublicationTrendsAsync(int days = 30);
}
