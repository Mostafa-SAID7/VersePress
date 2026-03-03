namespace VersePress.Application.DTOs;

/// <summary>
/// DTO for JSON-LD structured data
/// </summary>
public class JsonLdDto
{
    public string Context { get; set; } = "https://schema.org";
    public string Type { get; set; } = "BlogPosting";
    public string Headline { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public DateTime? DatePublished { get; set; }
    public DateTime? DateModified { get; set; }
    public AuthorDto? Author { get; set; }
    public PublisherDto? Publisher { get; set; }
    public string MainEntityOfPage { get; set; } = string.Empty;

    public class AuthorDto
    {
        public string Type { get; set; } = "Person";
        public string Name { get; set; } = string.Empty;
    }

    public class PublisherDto
    {
        public string Type { get; set; } = "Organization";
        public string Name { get; set; } = "VersePress";
        public LogoDto? Logo { get; set; }
    }

    public class LogoDto
    {
        public string Type { get; set; } = "ImageObject";
        public string Url { get; set; } = string.Empty;
    }
}
