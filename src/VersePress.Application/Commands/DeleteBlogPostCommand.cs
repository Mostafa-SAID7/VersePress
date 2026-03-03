namespace VersePress.Application.Commands;

public class DeleteBlogPostCommand
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
}
