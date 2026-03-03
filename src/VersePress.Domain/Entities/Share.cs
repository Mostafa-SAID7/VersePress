using VersePress.Domain.Enums;

namespace VersePress.Domain.Entities;

public class Share : BaseEntity
{
    public Guid BlogPostId { get; set; }
    public Platform Platform { get; set; }
    public DateTime SharedAt { get; set; }

    // Navigation properties
    public BlogPost BlogPost { get; set; } = null!;
}
