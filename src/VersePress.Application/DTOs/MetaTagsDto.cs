namespace VersePress.Application.DTOs;

/// <summary>
/// DTO for SEO meta tags
/// </summary>
public class MetaTagsDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string CanonicalUrl { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Language { get; set; } = string.Empty;
    public string AlternateLanguage { get; set; } = string.Empty;
    public string AlternateUrl { get; set; } = string.Empty;
}
