using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using VersePress.Application.Interfaces;

namespace VersePress.Web.Controllers;

/// <summary>
/// Controller for generating RSS feed
/// </summary>
public class RssController : Controller
{
    private readonly ISeoService _seoService;
    private readonly ILogger<RssController> _logger;

    public RssController(ISeoService seoService, ILogger<RssController> logger)
    {
        _seoService = seoService ?? throw new ArgumentNullException(nameof(seoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates RSS feed with latest blog posts
    /// Cached for 15 minutes to improve performance
    /// </summary>
    [HttpGet]
    [Route("rss")]
    [OutputCache(Duration = 900)] // Cache for 15 minutes (900 seconds)
    public async Task<IActionResult> Index()
    {
        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var rssFeed = await _seoService.GenerateRssFeedAsync(baseUrl, 20);

            return Content(rssFeed, "application/rss+xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating RSS feed");
            return StatusCode(500, "Error generating RSS feed");
        }
    }
}
