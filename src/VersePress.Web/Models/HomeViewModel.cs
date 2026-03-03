using VersePress.Application.DTOs;

namespace VersePress.Web.Models;

public class HomeViewModel
{
    public List<BlogPostDto> FeaturedPosts { get; set; } = new();
    public List<BlogPostDto> RecentPosts { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
}
