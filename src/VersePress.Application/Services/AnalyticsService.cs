using VersePress.Application.DTOs;
using VersePress.Application.Interfaces;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for analytics and dashboard statistics
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;

    public AnalyticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        // Aggregate counts from multiple repositories
        var allPosts = await _unitOfWork.BlogPosts.GetAllAsync();
        var allComments = await _unitOfWork.Comments.GetAllAsync();
        var allReactions = await _unitOfWork.Reactions.GetAllAsync();
        
        // For users, we need to get all entities and count
        // Since User is managed by Identity, we'll use a workaround
        // We'll get distinct author IDs from blog posts as a proxy for user count
        var distinctAuthorIds = allPosts.Select(p => p.AuthorId).Distinct().Count();

        var stats = new DashboardStatsDto
        {
            TotalPosts = allPosts.Count(),
            TotalComments = allComments.Count(),
            TotalUsers = distinctAuthorIds,
            TotalReactions = allReactions.Count()
        };

        return stats;
    }

    public async Task<IEnumerable<TopPostDto>> GetTopPostsByViewsAsync(int count = 10)
    {
        var allPosts = await _unitOfWork.BlogPosts.GetAllAsync();
        
        var topPosts = allPosts
            .OrderByDescending(p => p.ViewCount)
            .Take(count)
            .ToList();

        var result = new List<TopPostDto>();
        foreach (var post in topPosts)
        {
            var commentCount = post.Comments?.Count ?? 0;
            var reactionCounts = await _unitOfWork.Reactions.GetReactionCountsAsync(post.Id);
            var reactionCount = reactionCounts.Values.Sum();

            result.Add(new TopPostDto
            {
                Id = post.Id,
                Slug = post.Slug,
                TitleEn = post.TitleEn,
                TitleAr = post.TitleAr,
                ViewCount = post.ViewCount,
                CommentCount = commentCount,
                ReactionCount = reactionCount,
                PublishedAt = post.PublishedAt
            });
        }

        return result;
    }

    public async Task<IEnumerable<TopPostDto>> GetTopPostsByReactionsAsync(int count = 10)
    {
        var allPosts = await _unitOfWork.BlogPosts.GetAllAsync();
        var allReactions = await _unitOfWork.Reactions.GetAllAsync();

        // Group reactions by blog post ID and count
        var reactionCountsByPost = allReactions
            .GroupBy(r => r.BlogPostId)
            .ToDictionary(g => g.Key, g => g.Count());

        var topPosts = allPosts
            .Select(p => new
            {
                Post = p,
                ReactionCount = reactionCountsByPost.ContainsKey(p.Id) ? reactionCountsByPost[p.Id] : 0
            })
            .OrderByDescending(x => x.ReactionCount)
            .Take(count)
            .ToList();

        var result = new List<TopPostDto>();
        foreach (var item in topPosts)
        {
            var commentCount = item.Post.Comments?.Count ?? 0;

            result.Add(new TopPostDto
            {
                Id = item.Post.Id,
                Slug = item.Post.Slug,
                TitleEn = item.Post.TitleEn,
                TitleAr = item.Post.TitleAr,
                ViewCount = item.Post.ViewCount,
                CommentCount = commentCount,
                ReactionCount = item.ReactionCount,
                PublishedAt = item.Post.PublishedAt
            });
        }

        return result;
    }

    public async Task<IEnumerable<TopPostDto>> GetTopPostsByCommentsAsync(int count = 10)
    {
        var allPosts = await _unitOfWork.BlogPosts.GetAllAsync();
        var allComments = await _unitOfWork.Comments.GetAllAsync();

        // Group comments by blog post ID and count
        var commentCountsByPost = allComments
            .GroupBy(c => c.BlogPostId)
            .ToDictionary(g => g.Key, g => g.Count());

        var topPosts = allPosts
            .Select(p => new
            {
                Post = p,
                CommentCount = commentCountsByPost.ContainsKey(p.Id) ? commentCountsByPost[p.Id] : 0
            })
            .OrderByDescending(x => x.CommentCount)
            .Take(count)
            .ToList();

        var result = new List<TopPostDto>();
        foreach (var item in topPosts)
        {
            var reactionCounts = await _unitOfWork.Reactions.GetReactionCountsAsync(item.Post.Id);
            var reactionCount = reactionCounts.Values.Sum();

            result.Add(new TopPostDto
            {
                Id = item.Post.Id,
                Slug = item.Post.Slug,
                TitleEn = item.Post.TitleEn,
                TitleAr = item.Post.TitleAr,
                ViewCount = item.Post.ViewCount,
                CommentCount = item.CommentCount,
                ReactionCount = reactionCount,
                PublishedAt = item.Post.PublishedAt
            });
        }

        return result;
    }

    public async Task<IEnumerable<RecentShareDto>> GetRecentSharesAsync(int count = 20)
    {
        var allShares = await _unitOfWork.Shares.GetAllAsync();
        
        var recentShares = allShares
            .OrderByDescending(s => s.SharedAt)
            .Take(count)
            .ToList();

        var result = new List<RecentShareDto>();
        foreach (var share in recentShares)
        {
            // Get the blog post for this share
            var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(share.BlogPostId);
            
            result.Add(new RecentShareDto
            {
                Id = share.Id,
                BlogPostId = share.BlogPostId,
                BlogPostTitleEn = blogPost?.TitleEn ?? string.Empty,
                BlogPostTitleAr = blogPost?.TitleAr ?? string.Empty,
                Platform = share.Platform.ToString(),
                SharedAt = share.SharedAt
            });
        }

        return result;
    }

    public async Task<IEnumerable<PublicationTrendDto>> GetPublicationTrendsAsync(int days = 30)
    {
        var allPosts = await _unitOfWork.BlogPosts.GetAllAsync();
        
        var startDate = DateTime.UtcNow.Date.AddDays(-days);
        
        // Filter posts published within the specified date range
        var postsInRange = allPosts
            .Where(p => p.PublishedAt.HasValue && p.PublishedAt.Value.Date >= startDate)
            .ToList();

        // Group by date and count
        var trendData = postsInRange
            .GroupBy(p => p.PublishedAt!.Value.Date)
            .Select(g => new PublicationTrendDto
            {
                Date = g.Key,
                PostCount = g.Count()
            })
            .OrderBy(t => t.Date)
            .ToList();

        // Fill in missing dates with zero counts for complete chart data
        var result = new List<PublicationTrendDto>();
        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var existingData = trendData.FirstOrDefault(t => t.Date == date);
            
            result.Add(new PublicationTrendDto
            {
                Date = date,
                PostCount = existingData?.PostCount ?? 0
            });
        }

        return result;
    }
}
