using VersePress.Application.DTOs;
using VersePress.Application.Interfaces;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for searching blog posts across bilingual content
/// </summary>
public class SearchService : ISearchService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int SearchTimeoutSeconds = 5;

    public SearchService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IEnumerable<BlogPostDto>> SearchPostsAsync(string query, CancellationToken cancellationToken = default)
    {
        // Validate and sanitize input
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<BlogPostDto>();
        }

        // Sanitize query to prevent SQL injection
        var sanitizedQuery = SanitizeQuery(query);

        if (string.IsNullOrWhiteSpace(sanitizedQuery))
        {
            return Enumerable.Empty<BlogPostDto>();
        }

        // Create timeout cancellation token (5 seconds)
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(SearchTimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            // Execute search via repository
            var blogPosts = await _unitOfWork.BlogPosts.SearchPostsAsync(sanitizedQuery);

            // Map to DTOs
            var dtos = new List<BlogPostDto>();
            foreach (var post in blogPosts)
            {
                // Check for cancellation
                linkedCts.Token.ThrowIfCancellationRequested();

                dtos.Add(await MapToDto(post));
            }

            return dtos;
        }
        catch (OperationCanceledException)
        {
            // Handle timeout or cancellation
            if (timeoutCts.IsCancellationRequested)
            {
                throw new TimeoutException($"Search operation timed out after {SearchTimeoutSeconds} seconds.");
            }
            throw;
        }
    }

    /// <summary>
    /// Sanitizes search query to prevent SQL injection
    /// Removes potentially dangerous characters while preserving search functionality
    /// </summary>
    private string SanitizeQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return string.Empty;
        }

        // Trim whitespace
        query = query.Trim();

        // Remove SQL injection patterns and dangerous characters
        // Keep alphanumeric, spaces, and common punctuation for search
        var sanitized = new string(query
            .Where(c => char.IsLetterOrDigit(c) || 
                       char.IsWhiteSpace(c) || 
                       c == '-' || 
                       c == '_' || 
                       c == '\'' ||
                       c == '"')
            .ToArray());

        // Limit length to prevent abuse
        const int maxQueryLength = 200;
        if (sanitized.Length > maxQueryLength)
        {
            sanitized = sanitized.Substring(0, maxQueryLength);
        }

        return sanitized.Trim();
    }

    /// <summary>
    /// Maps a BlogPost entity to BlogPostDto
    /// </summary>
    private async Task<BlogPostDto> MapToDto(Domain.Entities.BlogPost blogPost)
    {
        // Get reaction counts
        var reactionCounts = await _unitOfWork.Reactions.GetReactionCountsAsync(blogPost.Id);

        var dto = new BlogPostDto
        {
            Id = blogPost.Id,
            Slug = blogPost.Slug,
            TitleEn = blogPost.TitleEn,
            TitleAr = blogPost.TitleAr,
            ContentEn = blogPost.ContentEn,
            ContentAr = blogPost.ContentAr,
            ExcerptEn = blogPost.ExcerptEn,
            ExcerptAr = blogPost.ExcerptAr,
            FeaturedImageUrl = blogPost.FeaturedImageUrl,
            ViewCount = blogPost.ViewCount,
            IsFeatured = blogPost.IsFeatured,
            PublishedAt = blogPost.PublishedAt,
            AuthorId = blogPost.AuthorId,
            AuthorName = blogPost.Author?.UserName ?? string.Empty,
            SeriesId = blogPost.SeriesId,
            SeriesNameEn = blogPost.Series?.NameEn,
            SeriesNameAr = blogPost.Series?.NameAr,
            ProjectId = blogPost.ProjectId,
            ProjectNameEn = blogPost.Project?.NameEn,
            ProjectNameAr = blogPost.Project?.NameAr,
            CommentCount = blogPost.Comments?.Count ?? 0,
            ReactionCount = reactionCounts.Values.Sum(),
            ReactionCounts = reactionCounts.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value
            ),
            Tags = blogPost.Tags?.Select(t => new TagDto
            {
                Id = t.Id,
                NameEn = t.NameEn,
                NameAr = t.NameAr,
                Slug = t.Slug
            }).ToList() ?? new List<TagDto>(),
            Categories = blogPost.Categories?.Select(c => new CategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                Slug = c.Slug,
                DescriptionEn = c.DescriptionEn,
                DescriptionAr = c.DescriptionAr
            }).ToList() ?? new List<CategoryDto>()
        };

        return dto;
    }
}
