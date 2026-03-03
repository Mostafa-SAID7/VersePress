using Microsoft.AspNetCore.Identity;

namespace VersePress.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Navigation properties
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
