using VersePress.Application.DTOs;

namespace VersePress.Web.Models;

public class AuthorDashboardViewModel
{
    public int TotalPosts { get; set; }
    public int TotalViews { get; set; }
    public List<BlogPostDto> Posts { get; set; } = new();
}
