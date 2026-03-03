using Microsoft.AspNetCore.SignalR;

namespace VersePress.Web.Hubs;

/// <summary>
/// SignalR hub for real-time notification delivery to users.
/// Manages user groups and targeted messaging for notifications.
/// </summary>
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// Adds the user to their personal notification group.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        
        if (!string.IsNullOrEmpty(userId))
        {
            // Add user to their personal group for targeted notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("User {UserId} connected to NotificationHub with connection {ConnectionId}", 
                userId, Context.ConnectionId);
        }
        
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// Removes the user from their personal notification group.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("User {UserId} disconnected from NotificationHub", userId);
        }
        
        if (exception != null)
        {
            _logger.LogError(exception, "User {UserId} disconnected with error", userId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Sends a notification to a specific user.
    /// Requirements: 19.1, 19.2, 19.3
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notification">Notification data</param>
    public async Task SendNotification(string userId, object notification)
    {
        try
        {
            await Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notification);
            _logger.LogInformation("Notification sent to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Marks a notification as read and broadcasts the update to the user.
    /// Requirements: 19.5
    /// </summary>
    /// <param name="notificationId">ID of the notification to mark as read</param>
    public async Task MarkAsRead(string notificationId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("MarkAsRead called without authenticated user");
                return;
            }

            // Broadcast the update back to the user's connections
            await Clients.Group($"user_{userId}").SendAsync("NotificationRead", notificationId);
            _logger.LogInformation("Notification {NotificationId} marked as read for user {UserId}", 
                notificationId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            throw;
        }
    }
}
