using System.ComponentModel.DataAnnotations;

namespace VersePress.Web.Models;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Subject is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Subject must be between 5 and 200 characters")]
    [Display(Name = "Subject")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 5000 characters")]
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
}
