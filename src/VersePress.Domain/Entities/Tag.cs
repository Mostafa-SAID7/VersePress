namespace VersePress.Domain.Entities;

public class Tag : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
