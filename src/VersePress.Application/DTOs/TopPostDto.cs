namespace VersePress.Application.DTOs;

/// <summary>
/// DTO for top posts with minimal information for analytics
/// </summary>
public class TopPostDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int CommentCount { get; set; }
    public int ReactionCount { get; set; }
    public DateTime? PublishedAt { get; set; }
}
