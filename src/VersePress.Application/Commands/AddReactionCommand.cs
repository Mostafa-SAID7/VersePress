using VersePress.Domain.Enums;

namespace VersePress.Application.Commands;

public class AddReactionCommand
{
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
}
