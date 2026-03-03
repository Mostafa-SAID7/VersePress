using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using VersePress.Application.Interfaces;

namespace VersePress.Web.Controllers;

/// <summary>
/// Controller for generating XML sitemap
/// </summary>
public class SitemapController : Controller
{
    private readonly ISeoService _seoService;
    private readonly ILogger<SitemapController> _logger;

    public SitemapController(ISeoService seoService, ILogger<SitemapController> logger)
    {
        _seoService = seoService ?? throw new ArgumentNullException(nameof(seoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates XML sitemap with all published blog posts
    /// Cached for 1 hour to improve performance
    /// </summary>
    [HttpGet]
    [Route("sitemap.xml")]
    [OutputCache(Duration = 3600)] // Cache for 1 hour (3600 seconds)
    public async Task<IActionResult> Index()
    {
        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var sitemapXml = await _seoService.GenerateSitemapAsync(baseUrl);

            return Content(sitemapXml, "application/xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sitemap");
            return StatusCode(500, "Error generating sitemap");
        }
    }
}
