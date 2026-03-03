using VersePress.Domain.Entities;

namespace VersePress.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Comment entity with moderation queries
/// </summary>
public interface ICommentRepository : IRepository<Comment>
{
    /// <summary>
    /// Retrieves all comments for a specific blog post including nested replies
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <returns>Collection of comments with nested structure</returns>
    Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid blogPostId);

    /// <summary>
    /// Retrieves all comments pending approval
    /// </summary>
    /// <returns>Collection of unapproved comments</returns>
    Task<IEnumerable<Comment>> GetPendingCommentsAsync();

    /// <summary>
    /// Gets the count of comments pending approval
    /// </summary>
    /// <returns>Number of pending comments</returns>
    Task<int> GetPendingCommentCountAsync();
}
