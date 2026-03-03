namespace VersePress.Domain.Entities;

/// <summary>
/// Tracks unique views of blog posts per session for 24-hour deduplication.
/// </summary>
public class PostView : BaseEntity
{
    /// <summary>
    /// The ID of the blog post that was viewed.
    /// </summary>
    public Guid BlogPostId { get; set; }

    /// <summary>
    /// The unique session identifier (from cookie or generated).
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the view was recorded.
    /// </summary>
    public DateTime ViewedAt { get; set; }

    // Navigation properties
    public BlogPost BlogPost { get; set; } = null!;
}
