using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using VersePress.Application.Interfaces;
using VersePress.Web.Models;

namespace VersePress.Web.Controllers;

public class HomeController : Controller
{
    private readonly IBlogPostService _blogPostService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IBlogPostService blogPostService,
        ILogger<HomeController> logger)
    {
        _blogPostService = blogPostService;
        _logger = logger;
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
    public IActionResult Contact(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // TODO: Implement email sending via IEmailService
        TempData["SuccessMessage"] = "Thank you for your message. We'll get back to you soon!";
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode.HasValue)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }
        }

        return View();
    }
}
