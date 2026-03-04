using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace VersePress.Infrastructure.Hubs;

/// <summary>
/// SignalR hub for real-time blog post interactions (reactions and comments).
/// Manages client groups by BlogPostId for efficient broadcasting.
/// </summary>
public class InteractionHub : Hub
{
    private readonly ILogger<InteractionHub> _logger;

    public InteractionHub(ILogger<InteractionHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Connection {ConnectionId} connected to InteractionHub", 
            Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Joins a blog post group to receive real-time updates for that post.
    /// </summary>
    /// <param name="blogPostId">ID of the blog post to subscribe to</param>
    public async Task JoinPostGroup(string blogPostId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"post_{blogPostId}");
        _logger.LogInformation("Connection {ConnectionId} joined post group {BlogPostId}", 
            Context.ConnectionId, blogPostId);
    }

    /// <summary>
    /// Leaves a blog post group to stop receiving updates for that post.
    /// </summary>
    /// <param name="blogPostId">ID of the blog post to unsubscribe from</param>
    public async Task LeavePostGroup(string blogPostId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"post_{blogPostId}");
        _logger.LogInformation("Connection {ConnectionId} left post group {BlogPostId}", 
            Context.ConnectionId, blogPostId);
    }

    /// <summary>
    /// Broadcasts a reaction update to all clients viewing the blog post.
    /// Requirements: 4.2, 4.6
    /// </summary>
    /// <param name="blogPostId">ID of the blog post</param>
    /// <param name="reaction">Reaction data including type and user information</param>
    public async Task BroadcastReaction(string blogPostId, object reaction)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            
            // Broadcast to all clients in the post group
            await Clients.Group($"post_{blogPostId}").SendAsync("ReactionUpdated", reaction);
            
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogInformation("Reaction broadcast to post {BlogPostId} completed in {Duration}ms", 
                blogPostId, duration);
            
            // Log warning if broadcast exceeds 500ms requirement
            if (duration > 500)
            {
                _logger.LogWarning("Reaction broadcast to post {BlogPostId} exceeded 500ms threshold: {Duration}ms", 
                    blogPostId, duration);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting reaction to post {BlogPostId}", blogPostId);
            throw;
        }
    }

    /// <summary>
    /// Broadcasts a comment update to all clients viewing the blog post.
    /// Requirements: 5.4
    /// </summary>
    /// <param name="blogPostId">ID of the blog post</param>
    /// <param name="comment">Comment data including content, author, and timestamp</param>
    public async Task BroadcastComment(string blogPostId, object comment)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            
            // Broadcast to all clients in the post group
            await Clients.Group($"post_{blogPostId}").SendAsync("CommentAdded", comment);
            
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogInformation("Comment broadcast to post {BlogPostId} completed in {Duration}ms", 
                blogPostId, duration);
            
            // Log warning if broadcast exceeds 500ms requirement
            if (duration > 500)
            {
                _logger.LogWarning("Comment broadcast to post {BlogPostId} exceeded 500ms threshold: {Duration}ms", 
                    blogPostId, duration);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting comment to post {BlogPostId}", blogPostId);
            throw;
        }
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// Automatically removes the client from all groups.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "Connection {ConnectionId} disconnected with error", 
                Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Connection {ConnectionId} disconnected", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
