using VersePress.Application.Commands;
using VersePress.Application.DTOs;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service interface for comment management and moderation operations
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Creates a new comment with validation and sets IsApproved to false
    /// </summary>
    /// <param name="command">Comment creation command</param>
    /// <returns>Created comment DTO</returns>
    Task<CommentDto> CreateCommentAsync(CreateCommentCommand command);

    /// <summary>
    /// Approves a pending comment and broadcasts via SignalR
    /// </summary>
    /// <param name="commentId">Comment identifier</param>
    /// <returns>Approved comment DTO</returns>
    Task<CommentDto> ApproveCommentAsync(Guid commentId);

    /// <summary>
    /// Rejects a comment by deleting it from the database
    /// </summary>
    /// <param name="commentId">Comment identifier</param>
    /// <returns>True if rejected successfully</returns>
    Task<bool> RejectCommentAsync(Guid commentId);

    /// <summary>
    /// Retrieves all comments for a blog post with nested structure
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <returns>Collection of comments with nested replies</returns>
    Task<IEnumerable<CommentDto>> GetCommentsByPostAsync(Guid blogPostId);

    /// <summary>
    /// Retrieves all pending comments requiring moderation
    /// </summary>
    /// <returns>Collection of pending comments</returns>
    Task<IEnumerable<CommentDto>> GetPendingCommentsAsync();

    /// <summary>
    /// Gets the count of pending comments
    /// </summary>
    /// <returns>Number of pending comments</returns>
    Task<int> GetPendingCommentCountAsync();
}
