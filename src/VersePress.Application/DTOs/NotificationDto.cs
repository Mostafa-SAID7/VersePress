using VersePress.Domain.Enums;

namespace VersePress.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
