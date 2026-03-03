using VersePress.Application.Commands;
using VersePress.Application.DTOs;
using VersePress.Application.Interfaces;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for managing reaction operations with real-time updates
/// </summary>
public class ReactionService : IReactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ReactionDto> AddReactionAsync(AddReactionCommand command)
    {
        // Validate that the blog post exists
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(command.BlogPostId);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {command.BlogPostId} not found.");
        }

        // Check for existing reaction from this user
        var existingReaction = await _unitOfWork.Reactions.GetUserReactionAsync(command.BlogPostId, command.UserId);

        Reaction reaction;

        if (existingReaction != null)
        {
            // Replace existing reaction with new type
            existingReaction.ReactionType = command.ReactionType;
            await _unitOfWork.Reactions.UpdateAsync(existingReaction);
            reaction = existingReaction;
        }
        else
        {
            // Create new reaction
            reaction = new Reaction
            {
                BlogPostId = command.BlogPostId,
                UserId = command.UserId,
                ReactionType = command.ReactionType
            };

            reaction = await _unitOfWork.Reactions.AddAsync(reaction);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        // Create notification for blog post author
        await CreateReactionNotificationAsync(reaction, blogPost);

        // TODO: Broadcast update via SignalR (will be implemented in task 7)
        // await _interactionHub.BroadcastReaction(command.BlogPostId, reactionCounts);

        // Return DTO
        return MapToDto(reaction);
    }

    public async Task<bool> RemoveReactionAsync(RemoveReactionCommand command)
    {
        // Check for existing reaction
        var existingReaction = await _unitOfWork.Reactions.GetUserReactionAsync(command.BlogPostId, command.UserId);

        if (existingReaction == null)
        {
            return false;
        }

        // Delete reaction
        await _unitOfWork.Reactions.DeleteAsync(existingReaction);
        await _unitOfWork.SaveChangesAsync();

        // TODO: Broadcast update via SignalR (will be implemented in task 7)
        // await _interactionHub.BroadcastReaction(command.BlogPostId, reactionCounts);

        return true;
    }

    public async Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid blogPostId)
    {
        return await _unitOfWork.Reactions.GetReactionCountsAsync(blogPostId);
    }

    public async Task<ReactionDto?> GetUserReactionAsync(Guid blogPostId, Guid userId)
    {
        var reaction = await _unitOfWork.Reactions.GetUserReactionAsync(blogPostId, userId);

        if (reaction == null)
        {
            return null;
        }

        return MapToDto(reaction);
    }

    /// <summary>
    /// Creates a notification for the blog post author when a reaction is added
    /// </summary>
    private async Task CreateReactionNotificationAsync(Reaction reaction, BlogPost blogPost)
    {
        // Don't notify if the user reacting is the author
        if (reaction.UserId == blogPost.AuthorId)
        {
            return;
        }

        // Create notification for blog post author
        var notification = new Notification
        {
            UserId = blogPost.AuthorId,
            Type = NotificationType.NewReaction,
            Content = $"New {reaction.ReactionType} reaction on your post: {blogPost.TitleEn}",
            RelatedEntityId = reaction.Id,
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        // TODO: Send notification via SignalR (will be implemented in task 7)
        // await _notificationHub.SendNotification(blogPost.AuthorId, notification);
    }

    /// <summary>
    /// Maps a Reaction entity to ReactionDto
    /// </summary>
    private ReactionDto MapToDto(Reaction reaction)
    {
        var dto = new ReactionDto
        {
            Id = reaction.Id,
            BlogPostId = reaction.BlogPostId,
            UserId = reaction.UserId,
            UserName = reaction.User?.UserName ?? string.Empty,
            ReactionType = reaction.ReactionType,
            CreatedAt = reaction.CreatedAt
        };

        return dto;
    }
}
