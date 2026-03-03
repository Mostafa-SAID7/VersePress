namespace VersePress.Application.DTOs;

/// <summary>
/// DTO for dashboard statistics overview
/// </summary>
public class DashboardStatsDto
{
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalUsers { get; set; }
    public int TotalReactions { get; set; }
}
