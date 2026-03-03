using VersePress.Application.DTOs;

namespace VersePress.Web.Models;

public class SearchResultsViewModel
{
    public string Query { get; set; } = string.Empty;
    public List<BlogPostDto> Results { get; set; } = new();
    public int TotalResults { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
