namespace VersePress.Domain.Entities;

public class Category : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }

    // Navigation properties
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
