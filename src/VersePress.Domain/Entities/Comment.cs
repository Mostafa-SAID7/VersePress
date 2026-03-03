namespace VersePress.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public bool IsApproved { get; set; }

    // Navigation properties
    public BlogPost BlogPost { get; set; } = null!;
    public User User { get; set; } = null!;
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
