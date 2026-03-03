using VersePress.Application.Commands;
using VersePress.Application.DTOs;
using VersePress.Domain.Enums;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service interface for reaction management operations
/// </summary>
public interface IReactionService
{
    /// <summary>
    /// Adds or updates a user's reaction to a blog post.
    /// If the user already has a reaction, it will be replaced with the new one.
    /// </summary>
    /// <param name="command">Reaction addition command</param>
    /// <returns>Created or updated reaction DTO</returns>
    Task<ReactionDto> AddReactionAsync(AddReactionCommand command);

    /// <summary>
    /// Removes a user's reaction from a blog post
    /// </summary>
    /// <param name="command">Reaction removal command</param>
    /// <returns>True if removed successfully, false if reaction not found</returns>
    Task<bool> RemoveReactionAsync(RemoveReactionCommand command);

    /// <summary>
    /// Gets aggregated reaction counts by type for a blog post
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <returns>Dictionary mapping reaction types to their counts</returns>
    Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid blogPostId);

    /// <summary>
    /// Retrieves a user's current reaction for a blog post
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>User's reaction DTO if exists, null otherwise</returns>
    Task<ReactionDto?> GetUserReactionAsync(Guid blogPostId, Guid userId);
}
