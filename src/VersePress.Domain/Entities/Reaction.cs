using VersePress.Domain.Enums;

namespace VersePress.Domain.Entities;

public class Reaction : BaseEntity
{
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }

    // Navigation properties
    public BlogPost BlogPost { get; set; } = null!;
    public User User { get; set; } = null!;
}
