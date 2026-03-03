using VersePress.Application.Interfaces;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for tracking blog post shares to social media platforms.
/// Records share events asynchronously without blocking user interaction.
/// </summary>
public class ShareTrackingService : IShareTrackingService
{
    private readonly IUnitOfWork _unitOfWork;

    public ShareTrackingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task RecordShareAsync(Guid blogPostId, Platform platform)
    {
        if (blogPostId == Guid.Empty)
        {
            throw new ArgumentException("Blog post ID cannot be empty.", nameof(blogPostId));
        }

        // Verify the blog post exists
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(blogPostId);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {blogPostId} not found.");
        }

        // Record the share asynchronously without blocking
        _ = Task.Run(async () =>
        {
            try
            {
                var share = new Share
                {
                    BlogPostId = blogPostId,
                    Platform = platform,
                    SharedAt = DateTime.UtcNow
                };

                await _unitOfWork.Shares.AddAsync(share);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Log error but don't throw - share tracking should not break user interaction
                // In production, this should be logged via ILogger
            }
        });

        // Return immediately without waiting for the background task
        await Task.CompletedTask;
    }

    public async Task<Dictionary<Platform, int>> GetShareCountsAsync(Guid blogPostId)
    {
        if (blogPostId == Guid.Empty)
        {
            throw new ArgumentException("Blog post ID cannot be empty.", nameof(blogPostId));
        }

        var allShares = await _unitOfWork.Shares.GetAllAsync();
        var postShares = allShares.Where(s => s.BlogPostId == blogPostId);

        // Aggregate shares by platform
        var shareCounts = postShares
            .GroupBy(s => s.Platform)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all platforms are represented (even with 0 count)
        var result = new Dictionary<Platform, int>();
        foreach (Platform platform in Enum.GetValues(typeof(Platform)))
        {
            result[platform] = shareCounts.ContainsKey(platform) ? shareCounts[platform] : 0;
        }

        return result;
    }

    public async Task<int> GetTotalShareCountAsync(Guid blogPostId)
    {
        if (blogPostId == Guid.Empty)
        {
            throw new ArgumentException("Blog post ID cannot be empty.", nameof(blogPostId));
        }

        var allShares = await _unitOfWork.Shares.GetAllAsync();
        var totalCount = allShares.Count(s => s.BlogPostId == blogPostId);

        return totalCount;
    }
}
