namespace VersePress.Application.Commands;

public class CreateCommentCommand
{
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}
