namespace VersePress.Application.DTOs;

/// <summary>
/// DTO for OpenGraph tags for social media sharing
/// </summary>
public class OpenGraphDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "article";
    public string Url { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string SiteName { get; set; } = "VersePress";
    public string Locale { get; set; } = string.Empty;
    public string AlternateLocale { get; set; } = string.Empty;
    public DateTime? PublishedTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
    public string? AuthorName { get; set; }
    public List<string> Tags { get; set; } = new();
}
