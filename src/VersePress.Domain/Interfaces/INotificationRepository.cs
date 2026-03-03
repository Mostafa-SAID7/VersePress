using VersePress.Domain.Entities;

namespace VersePress.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Notification entity with user-specific queries
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    /// <summary>
    /// Retrieves notifications for a specific user with optional filtering
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="unreadOnly">If true, returns only unread notifications</param>
    /// <returns>Collection of user notifications</returns>
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly);

    /// <summary>
    /// Gets the count of unread notifications for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Number of unread notifications</returns>
    Task<int> GetUnreadCountAsync(Guid userId);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    Task MarkAsReadAsync(Guid notificationId);
}
