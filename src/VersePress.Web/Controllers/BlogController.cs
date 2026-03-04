using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using VersePress.Application.Interfaces;
using VersePress.Web.Models;

namespace VersePress.Web.Controllers;

public class BlogController : Controller
{
    private readonly IBlogPostService _blogPostService;
    private readonly ICommentService _commentService;
    private readonly IReactionService _reactionService;
    private readonly IViewCounterService _viewCounterService;
    private readonly ISearchService _searchService;
    private readonly ISeoService _seoService;
    private readonly ILogger<BlogController> _logger;

    public BlogController(
        IBlogPostService blogPostService,
        ICommentService commentService,
        IReactionService reactionService,
        IViewCounterService viewCounterService,
        ISearchService searchService,
        ISeoService seoService,
        ILogger<BlogController> logger)
    {
        _blogPostService = blogPostService;
        _commentService = commentService;
        _reactionService = reactionService;
        _viewCounterService = viewCounterService;
        _searchService = searchService;
        _seoService = seoService;
        _logger = logger;
    }

    public async Task<IActionResult> Details(string slug)
    {
        try
        {
            var post = await _blogPostService.GetBlogPostBySlugAsync(slug);
            if (post == null)
            {
                return NotFound();
            }

            // Increment view count asynchronously
            _ = _viewCounterService.IncrementViewCountAsync(post.Id, HttpContext.Session.Id);

            var comments = await _commentService.GetCommentsByPostAsync(post.Id);
            
            // Get user's reaction if authenticated
            string? userReaction = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var reaction = await _reactionService.GetUserReactionAsync(post.Id, userId);
                userReaction = reaction?.ToString();
            }

            // Generate SEO data
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
            var language = currentCulture.StartsWith("ar") ? "ar" : "en";
            
            ViewData["MetaTags"] = await _seoService.GenerateMetaTagsAsync(post.Id, language, baseUrl);
            ViewData["OpenGraph"] = await _seoService.GenerateOpenGraphTagsAsync(post.Id, language, baseUrl);
            ViewData["JsonLd"] = await _seoService.GenerateJsonLdAsync(post.Id, language, baseUrl);

            var model = new BlogPostDetailViewModel
            {
                Post = post,
                Comments = comments.ToList(),
                ReactionCounts = post.ReactionCounts,
                UserReaction = userReaction,
                RelatedPosts = new List<Application.DTOs.BlogPostDto>() // TODO: Implement related posts logic
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading blog post: {Slug}", slug);
            return View("Error");
        }
    }

    [OutputCache(Duration = 300, VaryByQueryKeys = new[] { "page" }, VaryByHeaderNames = new[] { "Accept-Language", "Cookie" })]
    public IActionResult ByTag(string slug, int page = 1)
    {
        // TODO: Implement tag filtering
        return View("Index");
    }

    [OutputCache(Duration = 300, VaryByQueryKeys = new[] { "page" }, VaryByHeaderNames = new[] { "Accept-Language", "Cookie" })]
    public IActionResult ByCategory(string slug, int page = 1)
    {
        // TODO: Implement category filtering
        return View("Index");
    }

    [OutputCache(Duration = 300, VaryByHeaderNames = new[] { "Accept-Language", "Cookie" })]
    public IActionResult BySeries(string slug)
    {
        // TODO: Implement series filtering
        return View("Index");
    }

    [OutputCache(Duration = 300, VaryByHeaderNames = new[] { "Accept-Language", "Cookie" })]
    public IActionResult ByProject(string slug)
    {
        // TODO: Implement project filtering
        return View("Index");
    }

    public async Task<IActionResult> Search(string q, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return RedirectToAction("Index", "Home");
        }

        try
        {
            var results = await _searchService.SearchPostsAsync(q);
            
            var model = new SearchResultsViewModel
            {
                Query = q,
                Results = results.ToList(),
                TotalResults = results.Count(),
                CurrentPage = page,
                TotalPages = 1 // TODO: Implement pagination
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching posts: {Query}", q);
            return View("Error");
        }
    }
}
