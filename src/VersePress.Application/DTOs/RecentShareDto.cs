namespace VersePress.Application.DTOs;

/// <summary>
/// DTO for recent share events with platform breakdown
/// </summary>
public class RecentShareDto
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public string BlogPostTitleEn { get; set; } = string.Empty;
    public string BlogPostTitleAr { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
}
