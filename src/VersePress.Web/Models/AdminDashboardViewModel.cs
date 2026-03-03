using VersePress.Application.DTOs;

namespace VersePress.Web.Models;

public class AdminDashboardViewModel
{
    public DashboardStatsDto Stats { get; set; } = null!;
    public List<TopPostDto> TopPosts { get; set; } = new();
    public List<RecentShareDto> RecentShares { get; set; } = new();
    public List<PublicationTrendDto> PublicationTrends { get; set; } = new();
}
