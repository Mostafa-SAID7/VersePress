using VersePress.Application.DTOs;

namespace VersePress.Web.Models;

public class BlogPostDetailViewModel
{
    public BlogPostDto Post { get; set; } = null!;
    public List<CommentDto> Comments { get; set; } = new();
    public Dictionary<string, int> ReactionCounts { get; set; } = new();
    public string? UserReaction { get; set; }
    public List<BlogPostDto> RelatedPosts { get; set; } = new();
    public BlogPostDto? PreviousInSeries { get; set; }
    public BlogPostDto? NextInSeries { get; set; }
}
