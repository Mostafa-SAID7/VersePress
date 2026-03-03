using VersePress.Application.DTOs;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service interface for notification management and real-time delivery
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Creates a new notification and sends it via SignalR hub
    /// </summary>
    /// <param name="userId">User identifier to receive the notification</param>
    /// <param name="type">Type of notification</param>
    /// <param name="content">Notification content message</param>
    /// <param name="relatedEntityId">Optional related entity identifier</param>
    /// <returns>Created notification DTO</returns>
    Task<NotificationDto> CreateNotificationAsync(Guid userId, Domain.Enums.NotificationType type, string content, Guid? relatedEntityId = null);

    /// <summary>
    /// Retrieves notifications for a user with optional filtering by read status
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="unreadOnly">If true, returns only unread notifications</param>
    /// <returns>Collection of notifications</returns>
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <returns>True if marked successfully</returns>
    Task<bool> MarkAsReadAsync(Guid notificationId);

    /// <summary>
    /// Gets the count of unread notifications for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Number of unread notifications</returns>
    Task<int> GetUnreadCountAsync(Guid userId);
}
