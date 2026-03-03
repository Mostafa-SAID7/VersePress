using VersePress.Domain.Enums;

namespace VersePress.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
