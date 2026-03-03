using VersePress.Application.DTOs;
using VersePress.Application.Interfaces;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for managing notification operations with real-time delivery
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<NotificationDto> CreateNotificationAsync(Guid userId, NotificationType type, string content, Guid? relatedEntityId = null)
    {
        // Create notification entity
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Content = content,
            RelatedEntityId = relatedEntityId,
            IsRead = false
        };

        // Save to repository
        var createdNotification = await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        // TODO: Send notification via SignalR hub (will be implemented in task 7)
        // await _notificationHub.SendNotification(userId, createdNotification);

        // Return DTO
        return MapToDto(createdNotification);
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false)
    {
        // Retrieve notifications with filtering by read status
        var notifications = await _unitOfWork.Notifications.GetUserNotificationsAsync(userId, unreadOnly);

        var dtos = new List<NotificationDto>();
        foreach (var notification in notifications)
        {
            dtos.Add(MapToDto(notification));
        }

        return dtos;
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        // Retrieve notification
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null)
        {
            return false;
        }

        // Update read status
        await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        // Count unread notifications for user
        return await _unitOfWork.Notifications.GetUnreadCountAsync(userId);
    }

    /// <summary>
    /// Maps a Notification entity to NotificationDto
    /// </summary>
    private NotificationDto MapToDto(Notification notification)
    {
        var dto = new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Content = notification.Content,
            RelatedEntityId = notification.RelatedEntityId,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };

        return dto;
    }
}
