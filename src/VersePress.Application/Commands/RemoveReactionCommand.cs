namespace VersePress.Application.Commands;

public class RemoveReactionCommand
{
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
}
