namespace VersePress.Application.DTOs;

/// <summary>
/// DTO for publication trend data (for charts)
/// </summary>
public class PublicationTrendDto
{
    public DateTime Date { get; set; }
    public int PostCount { get; set; }
}
