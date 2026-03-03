using VersePress.Application.Commands;
using VersePress.Application.DTOs;
using VersePress.Application.Interfaces;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for managing comment operations and moderation workflow
/// </summary>
public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentCommand command)
    {
        // Validate that the blog post exists
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(command.BlogPostId);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {command.BlogPostId} not found.");
        }

        // Validate parent comment if provided
        if (command.ParentCommentId.HasValue)
        {
            var parentComment = await _unitOfWork.Comments.GetByIdAsync(command.ParentCommentId.Value);
            if (parentComment == null)
            {
                throw new InvalidOperationException($"Parent comment with ID {command.ParentCommentId.Value} not found.");
            }

            // Ensure parent comment belongs to the same blog post
            if (parentComment.BlogPostId != command.BlogPostId)
            {
                throw new InvalidOperationException("Parent comment does not belong to the specified blog post.");
            }
        }

        // Create comment entity with IsApproved = false
        var comment = new Comment
        {
            BlogPostId = command.BlogPostId,
            UserId = command.UserId,
            Content = command.Content,
            ParentCommentId = command.ParentCommentId,
            IsApproved = false
        };

        // Save to repository
        var createdComment = await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        // Trigger notification to blog post author
        await CreateCommentNotificationAsync(createdComment, blogPost);

        // Return DTO
        return MapToDto(createdComment);
    }

    public async Task<CommentDto> ApproveCommentAsync(Guid commentId)
    {
        // Retrieve comment
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null)
        {
            throw new InvalidOperationException($"Comment with ID {commentId} not found.");
        }

        // Set IsApproved to true
        comment.IsApproved = true;

        await _unitOfWork.Comments.UpdateAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        // TODO: Broadcast via SignalR (will be implemented in task 7)
        // await _interactionHub.BroadcastComment(comment);

        return MapToDto(comment);
    }

    public async Task<bool> RejectCommentAsync(Guid commentId)
    {
        // Retrieve comment
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null)
        {
            return false;
        }

        // Delete comment from database
        await _unitOfWork.Comments.DeleteAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByPostAsync(Guid blogPostId)
    {
        // Get all comments for the post with nested structure
        var comments = await _unitOfWork.Comments.GetCommentsByPostAsync(blogPostId);

        // Build nested structure
        var commentDtos = new List<CommentDto>();
        var commentDict = new Dictionary<Guid, CommentDto>();

        // First pass: create DTOs for all comments
        foreach (var comment in comments)
        {
            var dto = MapToDto(comment);
            commentDict[comment.Id] = dto;
        }

        // Second pass: build nested structure
        foreach (var comment in comments)
        {
            var dto = commentDict[comment.Id];

            if (comment.ParentCommentId.HasValue && commentDict.ContainsKey(comment.ParentCommentId.Value))
            {
                // Add as reply to parent
                commentDict[comment.ParentCommentId.Value].Replies.Add(dto);
            }
            else
            {
                // Top-level comment
                commentDtos.Add(dto);
            }
        }

        return commentDtos;
    }

    public async Task<IEnumerable<CommentDto>> GetPendingCommentsAsync()
    {
        var comments = await _unitOfWork.Comments.GetPendingCommentsAsync();
        var dtos = new List<CommentDto>();

        foreach (var comment in comments)
        {
            dtos.Add(MapToDto(comment));
        }

        return dtos;
    }

    public async Task<int> GetPendingCommentCountAsync()
    {
        return await _unitOfWork.Comments.GetPendingCommentCountAsync();
    }

    /// <summary>
    /// Creates a notification for the blog post author when a comment is added
    /// </summary>
    private async Task CreateCommentNotificationAsync(Comment comment, BlogPost blogPost)
    {
        // Don't notify if the commenter is the author
        if (comment.UserId == blogPost.AuthorId)
        {
            return;
        }

        // Create notification for blog post author
        var notification = new Notification
        {
            UserId = blogPost.AuthorId,
            Type = NotificationType.NewComment,
            Content = $"New comment on your post: {blogPost.TitleEn}",
            RelatedEntityId = comment.Id,
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        // TODO: Send notification via SignalR (will be implemented in task 7)
        // await _notificationHub.SendNotification(blogPost.AuthorId, notification);
    }

    /// <summary>
    /// Maps a Comment entity to CommentDto
    /// </summary>
    private CommentDto MapToDto(Comment comment)
    {
        var dto = new CommentDto
        {
            Id = comment.Id,
            BlogPostId = comment.BlogPostId,
            UserId = comment.UserId,
            UserName = comment.User?.UserName ?? string.Empty,
            UserProfileImageUrl = comment.User?.ProfileImageUrl,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            IsApproved = comment.IsApproved,
            Replies = new List<CommentDto>()
        };

        return dto;
    }
}
