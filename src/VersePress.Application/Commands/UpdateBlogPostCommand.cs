namespace VersePress.Application.Commands;

public class UpdateBlogPostCommand
{
    public Guid Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public string ContentAr { get; set; } = string.Empty;
    public string? ExcerptEn { get; set; }
    public string? ExcerptAr { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public Guid? SeriesId { get; set; }
    public Guid? ProjectId { get; set; }
    public List<Guid> TagIds { get; set; } = new();
    public List<Guid> CategoryIds { get; set; } = new();
}
