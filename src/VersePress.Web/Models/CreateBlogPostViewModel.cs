using System.ComponentModel.DataAnnotations;

namespace VersePress.Web.Models;

public class CreateBlogPostViewModel
{
    [Required(ErrorMessage = "English title is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
    [Display(Name = "Title (English)")]
    public string TitleEn { get; set; } = string.Empty;

    [Required(ErrorMessage = "Arabic title is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
    [Display(Name = "Title (Arabic)")]
    public string TitleAr { get; set; } = string.Empty;

    [Required(ErrorMessage = "English content is required")]
    [MinLength(100, ErrorMessage = "Content must be at least 100 characters")]
    [Display(Name = "Content (English)")]
    public string ContentEn { get; set; } = string.Empty;

    [Required(ErrorMessage = "Arabic content is required")]
    [MinLength(100, ErrorMessage = "Content must be at least 100 characters")]
    [Display(Name = "Content (Arabic)")]
    public string ContentAr { get; set; } = string.Empty;

    [Display(Name = "Excerpt (English)")]
    [StringLength(500)]
    public string? ExcerptEn { get; set; }

    [Display(Name = "Excerpt (Arabic)")]
    [StringLength(500)]
    public string? ExcerptAr { get; set; }

    [Display(Name = "Featured Image URL")]
    [Url(ErrorMessage = "Invalid URL")]
    public string? FeaturedImageUrl { get; set; }

    [Display(Name = "Featured Post")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Series")]
    public Guid? SeriesId { get; set; }

    [Display(Name = "Project")]
    public Guid? ProjectId { get; set; }

    [Display(Name = "Tags")]
    public List<Guid> TagIds { get; set; } = new();

    [Display(Name = "Categories")]
    public List<Guid> CategoryIds { get; set; } = new();
}
