using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VersePress.Application.Commands;
using VersePress.Application.Interfaces;
using VersePress.Web.Models;

namespace VersePress.Web.Controllers;

[Authorize(Policy = "AuthorPolicy")]
public class AuthorController : Controller
{
    private readonly IBlogPostService _blogPostService;
    private readonly ILogger<AuthorController> _logger;

    public AuthorController(
        IBlogPostService blogPostService,
        ILogger<AuthorController> logger)
    {
        _blogPostService = blogPostService;
        _logger = logger;
    }

    public IActionResult Dashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            
            // TODO: Get author's posts
            var model = new AuthorDashboardViewModel
            {
                TotalPosts = 0,
                TotalViews = 0,
                Posts = new List<Application.DTOs.BlogPostDto>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading author dashboard");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateBlogPostViewModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBlogPostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            var command = new CreateBlogPostCommand
            {
                TitleEn = model.TitleEn,
                TitleAr = model.TitleAr,
                ContentEn = model.ContentEn,
                ContentAr = model.ContentAr,
                ExcerptEn = model.ExcerptEn,
                ExcerptAr = model.ExcerptAr,
                FeaturedImageUrl = model.FeaturedImageUrl,
                IsFeatured = model.IsFeatured,
                AuthorId = userId,
                SeriesId = model.SeriesId,
                ProjectId = model.ProjectId,
                TagIds = model.TagIds,
                CategoryIds = model.CategoryIds
            };

            var result = await _blogPostService.CreateBlogPostAsync(command);

            TempData["SuccessMessage"] = "Blog post created successfully!";
            return RedirectToAction("Details", "Blog", new { slug = result.Slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blog post");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the blog post.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var post = await _blogPostService.GetBlogPostBySlugAsync(id.ToString());
            if (post == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            if (post.AuthorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var model = new CreateBlogPostViewModel
            {
                TitleEn = post.TitleEn,
                TitleAr = post.TitleAr,
                ContentEn = post.ContentEn,
                ContentAr = post.ContentAr,
                ExcerptEn = post.ExcerptEn,
                ExcerptAr = post.ExcerptAr,
                FeaturedImageUrl = post.FeaturedImageUrl,
                IsFeatured = post.IsFeatured,
                SeriesId = post.SeriesId,
                ProjectId = post.ProjectId,
                TagIds = post.Tags.Select(t => t.Id).ToList(),
                CategoryIds = post.Categories.Select(c => c.Id).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading blog post for edit: {Id}", id);
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CreateBlogPostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            var command = new UpdateBlogPostCommand
            {
                Id = id,
                TitleEn = model.TitleEn,
                TitleAr = model.TitleAr,
                ContentEn = model.ContentEn,
                ContentAr = model.ContentAr,
                ExcerptEn = model.ExcerptEn,
                ExcerptAr = model.ExcerptAr,
                FeaturedImageUrl = model.FeaturedImageUrl,
                IsFeatured = model.IsFeatured,
                SeriesId = model.SeriesId,
                ProjectId = model.ProjectId,
                TagIds = model.TagIds,
                CategoryIds = model.CategoryIds
            };

            var result = await _blogPostService.UpdateBlogPostAsync(command, userId);

            TempData["SuccessMessage"] = "Blog post updated successfully!";
            return RedirectToAction("Details", "Blog", new { slug = result.Slug });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blog post: {Id}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the blog post.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            var command = new DeleteBlogPostCommand
            {
                Id = id,
                AuthorId = userId
            };

            await _blogPostService.DeleteBlogPostAsync(command);

            TempData["SuccessMessage"] = "Blog post deleted successfully!";
            return RedirectToAction(nameof(Dashboard));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post: {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the blog post.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
