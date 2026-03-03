using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using VersePress.Application.Interfaces;
using VersePress.Web.Models;

namespace VersePress.Web.Controllers;

public class HomeController : Controller
{
    private readonly IBlogPostService _blogPostService;
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(
        IBlogPostService blogPostService,
        ILogger<HomeController> logger,
        IConfiguration configuration)
    {
        _blogPostService = blogPostService;
        _logger = logger;
        _configuration = configuration;
    }

    [OutputCache(Duration = 300, VaryByQueryKeys = new[] { "page" }, VaryByHeaderNames = new[] { "Accept-Language", "Cookie" })]
    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 10;
        
        try
        {
            var featuredPosts = await _blogPostService.GetFeaturedPostsAsync(3);
            var recentPosts = await _blogPostService.GetPublishedPostsAsync(page, pageSize);
            
            var model = new HomeViewModel
            {
                FeaturedPosts = featuredPosts.ToList(),
                RecentPosts = recentPosts.ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                // TODO: Calculate total pages from total count
                TotalPages = 1
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page");
            return View("Error");
        }
    }

    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Get email service
            var emailService = HttpContext.RequestServices.GetService<VersePress.Application.Interfaces.IEmailService>();
            
            if (emailService != null)
            {
                // Prepare email content
                var subject = $"Contact Form: {model.Subject}";
                var body = $@"
                    <h2>New Contact Form Submission</h2>
                    <p><strong>From:</strong> {model.Name} ({model.Email})</p>
                    <p><strong>Subject:</strong> {model.Subject}</p>
                    <p><strong>Message:</strong></p>
                    <p>{model.Message.Replace("\n", "<br/>")}</p>
                    <hr/>
                    <p><small>Sent from VersePress Contact Form</small></p>
                ";

                // Send email to admin (configured in appsettings)
                var adminEmail = _configuration["EmailSettings:SenderEmail"] ?? "admin@versepress.com";
                await emailService.SendEmailAsync(adminEmail, subject, body);

                _logger.LogInformation("Contact form submitted by {Name} ({Email})", model.Name, model.Email);
            }
            else
            {
                _logger.LogWarning("Email service not configured. Contact form submission logged only.");
            }

            TempData["SuccessMessage"] = "Thank you for your message. We'll get back to you soon!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing contact form submission");
            ModelState.AddModelError("", "An error occurred while sending your message. Please try again later.");
            return View(model);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        _logger.LogWarning("Error page requested with status code: {StatusCode}", statusCode);
        
        if (statusCode.HasValue)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }
            else if (statusCode == 500)
            {
                return View("ServerError");
            }
        }

        var model = new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View(model);
    }
}
