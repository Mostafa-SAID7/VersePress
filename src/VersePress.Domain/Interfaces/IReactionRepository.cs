using VersePress.Domain.Entities;
using VersePress.Domain.Enums;

namespace VersePress.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Reaction entity with aggregation queries
/// </summary>
public interface IReactionRepository : IRepository<Reaction>
{
    /// <summary>
    /// Retrieves a user's reaction to a specific blog post
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>User's reaction if exists, null otherwise</returns>
    Task<Reaction?> GetUserReactionAsync(Guid blogPostId, Guid userId);

    /// <summary>
    /// Gets aggregated reaction counts by type for a blog post
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <returns>Dictionary mapping reaction types to their counts</returns>
    Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid blogPostId);
}
