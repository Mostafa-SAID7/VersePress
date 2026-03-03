namespace VersePress.Application.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserProfileImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; }

    // Nested replies
    public List<CommentDto> Replies { get; set; } = new();
}
