using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VersePress.Application.Interfaces;
using VersePress.Web.Models;

namespace VersePress.Web.Controllers;

[Authorize(Policy = "AdminPolicy")]
public class AdminController : Controller
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ICommentService _commentService;
    private readonly IBlogPostService _blogPostService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IAnalyticsService analyticsService,
        ICommentService commentService,
        IBlogPostService blogPostService,
        ILogger<AdminController> logger)
    {
        _analyticsService = analyticsService;
        _commentService = commentService;
        _blogPostService = blogPostService;
        _logger = logger;
    }

    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var stats = await _analyticsService.GetDashboardStatsAsync();
            var topPosts = await _analyticsService.GetTopPostsByViewsAsync(5);
            var recentShares = await _analyticsService.GetRecentSharesAsync(10);
            var trends = await _analyticsService.GetPublicationTrendsAsync(30);

            var model = new AdminDashboardViewModel
            {
                Stats = stats,
                TopPosts = topPosts.ToList(),
                RecentShares = recentShares.ToList(),
                PublicationTrends = trends.ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            return View("Errors/Error");
        }
    }

    public async Task<IActionResult> Posts(int page = 1)
    {
        try
        {
            var posts = await _blogPostService.GetPublishedPostsAsync(page, 20);
            return View(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading posts");
            return View("Errors/Error");
        }
    }

    public async Task<IActionResult> Comments()
    {
        try
        {
            var pendingComments = await _commentService.GetPendingCommentsAsync();
            return View(pendingComments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading comments");
            return View("Errors/Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveComment(Guid id)
    {
        try
        {
            await _commentService.ApproveCommentAsync(id);
            TempData["SuccessMessage"] = "Comment approved successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving comment: {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while approving the comment.";
        }

        return RedirectToAction(nameof(Comments));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectComment(Guid id)
    {
        try
        {
            await _commentService.RejectCommentAsync(id);
            TempData["SuccessMessage"] = "Comment rejected successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting comment: {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while rejecting the comment.";
        }

        return RedirectToAction(nameof(Comments));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFeatured(Guid id)
    {
        try
        {
            await _blogPostService.ToggleFeaturedAsync(id);
            TempData["SuccessMessage"] = "Featured status updated successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling featured status: {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while updating featured status.";
        }

        return RedirectToAction(nameof(Posts));
    }

    public IActionResult Analytics()
    {
        return View();
    }

    public IActionResult Users()
    {
        // TODO: Implement user management
        return View();
    }
}
