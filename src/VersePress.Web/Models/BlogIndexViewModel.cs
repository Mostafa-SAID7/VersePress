using VersePress.Application.DTOs;

namespace VersePress.Web.Models;

public class BlogIndexViewModel
{
    public List<BlogPostDto> Posts { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? FilterType { get; set; }
    public string? FilterValue { get; set; }
}
