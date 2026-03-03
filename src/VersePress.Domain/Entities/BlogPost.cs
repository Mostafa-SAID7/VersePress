namespace VersePress.Domain.Entities;

public class BlogPost : BaseEntity
{
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
    public Guid? SeriesId { get; set; }
    public Guid? ProjectId { get; set; }

    // Navigation properties
    public User Author { get; set; } = null!;
    public Series? Series { get; set; }
    public Project? Project { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<Share> Shares { get; set; } = new List<Share>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
