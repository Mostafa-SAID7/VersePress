using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;
using VersePress.Infrastructure.Data;

namespace VersePress.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Reaction entities with specialized query methods.
/// </summary>
public class ReactionRepository : Repository<Reaction>, IReactionRepository
{
    /// <summary>
    /// Initializes a new instance of the ReactionRepository class.
    /// </summary>
    /// <param name="context">Application database context</param>
    public ReactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<Reaction?> GetUserReactionAsync(Guid blogPostId, Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.BlogPostId == blogPostId && r.UserId == userId);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid blogPostId)
    {
        var reactions = await _dbSet
            .AsNoTracking()
            .Where(r => r.BlogPostId == blogPostId)
            .GroupBy(r => r.ReactionType)
            .Select(g => new { ReactionType = g.Key, Count = g.Count() })
            .ToListAsync();

        return reactions.ToDictionary(r => r.ReactionType, r => r.Count);
    }
}
