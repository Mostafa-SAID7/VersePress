using VersePress.Domain.Enums;

namespace VersePress.Application.DTOs;

public class ShareDto
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public string BlogPostTitle { get; set; } = string.Empty;
    public Platform Platform { get; set; }
    public DateTime SharedAt { get; set; }
}
