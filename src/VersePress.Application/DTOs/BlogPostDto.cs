namespace VersePress.Application.DTOs;

public class BlogPostDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public string ContentAr { get; set; } = string.Empty;
    public string? ExcerptEn { get; set; }
    public string? ExcerptAr { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public int ViewCount { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid? SeriesId { get; set; }
    public string? SeriesNameEn { get; set; }
    public string? SeriesNameAr { get; set; }
    public Guid? ProjectId { get; set; }
    public string? ProjectNameEn { get; set; }
    public string? ProjectNameAr { get; set; }

    // Computed fields
    public int CommentCount { get; set; }
    public int ReactionCount { get; set; }
    public Dictionary<string, int> ReactionCounts { get; set; } = new();

    // Related data
    public List<TagDto> Tags { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
}
