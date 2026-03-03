using VersePress.Application.DTOs;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service interface for searching blog posts
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Searches blog posts by query string across titles and content in both languages
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <param name="cancellationToken">Cancellation token for timeout handling</param>
    /// <returns>Collection of matching blog posts ranked by relevance</returns>
    Task<IEnumerable<BlogPostDto>> SearchPostsAsync(string query, CancellationToken cancellationToken = default);
}
