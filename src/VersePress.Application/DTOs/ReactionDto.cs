using VersePress.Domain.Enums;

namespace VersePress.Application.DTOs;

public class ReactionDto
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ReactionType ReactionType { get; set; }
    public DateTime CreatedAt { get; set; }
}
