using System.Text;
using System.Text.RegularExpressions;
using VersePress.Application.Commands;
using VersePress.Application.DTOs;
using VersePress.Application.Interfaces;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;

namespace VersePress.Application.Services;

/// <summary>
/// Service for managing blog post operations
/// </summary>
public class BlogPostService : IBlogPostService
{
    private readonly IUnitOfWork _unitOfWork;

    public BlogPostService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<BlogPostDto> CreateBlogPostAsync(CreateBlogPostCommand command)
    {
        // Generate unique slug from English title
        var slug = await GenerateUniqueSlugAsync(command.TitleEn);

        // Create blog post entity
        var blogPost = new BlogPost
        {
            Slug = slug,
            TitleEn = command.TitleEn,
            TitleAr = command.TitleAr,
            ContentEn = command.ContentEn,
            ContentAr = command.ContentAr,
            ExcerptEn = command.ExcerptEn,
            ExcerptAr = command.ExcerptAr,
            FeaturedImageUrl = command.FeaturedImageUrl,
            IsFeatured = command.IsFeatured,
            AuthorId = command.AuthorId,
            SeriesId = command.SeriesId,
            ProjectId = command.ProjectId,
            PublishedAt = DateTime.UtcNow,
            ViewCount = 0
        };

        // Load and associate tags
        if (command.TagIds.Any())
        {
            var tags = new List<Tag>();
            foreach (var tagId in command.TagIds)
            {
                var tag = await _unitOfWork.Tags.GetByIdAsync(tagId);
                if (tag != null)
                {
                    tags.Add(tag);
                }
            }
            blogPost.Tags = tags;
        }

        // Load and associate categories
        if (command.CategoryIds.Any())
        {
            var categories = new List<Category>();
            foreach (var categoryId in command.CategoryIds)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category != null)
                {
                    categories.Add(category);
                }
            }
            blogPost.Categories = categories;
        }

        // Save to repository
        var createdPost = await _unitOfWork.BlogPosts.AddAsync(blogPost);
        await _unitOfWork.SaveChangesAsync();

        // Return DTO
        return await MapToDto(createdPost);
    }

    public async Task<BlogPostDto> UpdateBlogPostAsync(UpdateBlogPostCommand command, Guid currentUserId)
    {
        // Retrieve existing blog post
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(command.Id);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {command.Id} not found.");
        }

        // Validate ownership (only author can update their own posts)
        if (blogPost.AuthorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this blog post.");
        }

        // Update fields
        blogPost.TitleEn = command.TitleEn;
        blogPost.TitleAr = command.TitleAr;
        blogPost.ContentEn = command.ContentEn;
        blogPost.ContentAr = command.ContentAr;
        blogPost.ExcerptEn = command.ExcerptEn;
        blogPost.ExcerptAr = command.ExcerptAr;
        blogPost.FeaturedImageUrl = command.FeaturedImageUrl;
        blogPost.IsFeatured = command.IsFeatured;
        blogPost.SeriesId = command.SeriesId;
        blogPost.ProjectId = command.ProjectId;

        // Update tags
        blogPost.Tags.Clear();
        if (command.TagIds.Any())
        {
            var tags = new List<Tag>();
            foreach (var tagId in command.TagIds)
            {
                var tag = await _unitOfWork.Tags.GetByIdAsync(tagId);
                if (tag != null)
                {
                    tags.Add(tag);
                }
            }
            blogPost.Tags = tags;
        }

        // Update categories
        blogPost.Categories.Clear();
        if (command.CategoryIds.Any())
        {
            var categories = new List<Category>();
            foreach (var categoryId in command.CategoryIds)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category != null)
                {
                    categories.Add(category);
                }
            }
            blogPost.Categories = categories;
        }

        // Save changes
        await _unitOfWork.BlogPosts.UpdateAsync(blogPost);
        await _unitOfWork.SaveChangesAsync();

        // Return DTO
        return await MapToDto(blogPost);
    }

    public async Task<bool> DeleteBlogPostAsync(DeleteBlogPostCommand command)
    {
        // Retrieve blog post
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(command.Id);
        if (blogPost == null)
        {
            return false;
        }

        // Verify ownership
        if (blogPost.AuthorId != command.AuthorId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this blog post.");
        }

        // Delete via repository (soft delete)
        await _unitOfWork.BlogPosts.DeleteAsync(blogPost);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<BlogPostDto?> GetBlogPostBySlugAsync(string slug)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetBySlugAsync(slug);
        if (blogPost == null)
        {
            return null;
        }

        return await MapToDto(blogPost);
    }

    public async Task<IEnumerable<BlogPostDto>> GetPublishedPostsAsync(int page, int pageSize)
    {
        var blogPosts = await _unitOfWork.BlogPosts.GetPublishedPostsAsync(page, pageSize);
        var dtos = new List<BlogPostDto>();

        foreach (var post in blogPosts)
        {
            dtos.Add(await MapToDto(post));
        }

        return dtos;
    }

    public async Task<IEnumerable<BlogPostDto>> GetFeaturedPostsAsync(int count)
    {
        var blogPosts = await _unitOfWork.BlogPosts.GetFeaturedPostsAsync(count);
        var dtos = new List<BlogPostDto>();

        foreach (var post in blogPosts)
        {
            dtos.Add(await MapToDto(post));
        }

        return dtos;
    }

    public async Task<BlogPostDto> ToggleFeaturedAsync(Guid postId)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(postId);
        if (blogPost == null)
        {
            throw new InvalidOperationException($"Blog post with ID {postId} not found.");
        }

        // Toggle featured status
        blogPost.IsFeatured = !blogPost.IsFeatured;

        await _unitOfWork.BlogPosts.UpdateAsync(blogPost);
        await _unitOfWork.SaveChangesAsync();

        return await MapToDto(blogPost);
    }

    /// <summary>
    /// Generates a URL-safe slug from a title
    /// </summary>
    private string GenerateSlug(string title)
    {
        // Convert to lowercase
        var slug = title.ToLowerInvariant();

        // Remove special characters and replace spaces with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Generates a unique slug by appending a number if necessary
    /// </summary>
    private async Task<string> GenerateUniqueSlugAsync(string title)
    {
        var baseSlug = GenerateSlug(title);
        var slug = baseSlug;
        var counter = 1;

        // Check if slug exists and append counter if needed
        while (await _unitOfWork.BlogPosts.SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    /// <summary>
    /// Maps a BlogPost entity to BlogPostDto
    /// </summary>
    private async Task<BlogPostDto> MapToDto(BlogPost blogPost)
    {
        // Get reaction counts
        var reactionCounts = await _unitOfWork.Reactions.GetReactionCountsAsync(blogPost.Id);

        var dto = new BlogPostDto
        {
            Id = blogPost.Id,
            Slug = blogPost.Slug,
            TitleEn = blogPost.TitleEn,
            TitleAr = blogPost.TitleAr,
            ContentEn = blogPost.ContentEn,
            ContentAr = blogPost.ContentAr,
            ExcerptEn = blogPost.ExcerptEn,
            ExcerptAr = blogPost.ExcerptAr,
            FeaturedImageUrl = blogPost.FeaturedImageUrl,
            ViewCount = blogPost.ViewCount,
            IsFeatured = blogPost.IsFeatured,
            PublishedAt = blogPost.PublishedAt,
            AuthorId = blogPost.AuthorId,
            AuthorName = blogPost.Author?.UserName ?? string.Empty,
            SeriesId = blogPost.SeriesId,
            SeriesNameEn = blogPost.Series?.NameEn,
            SeriesNameAr = blogPost.Series?.NameAr,
            ProjectId = blogPost.ProjectId,
            ProjectNameEn = blogPost.Project?.NameEn,
            ProjectNameAr = blogPost.Project?.NameAr,
            CommentCount = blogPost.Comments?.Count ?? 0,
            ReactionCount = reactionCounts.Values.Sum(),
            ReactionCounts = reactionCounts.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value
            ),
            Tags = blogPost.Tags?.Select(t => new TagDto
            {
                Id = t.Id,
                NameEn = t.NameEn,
                NameAr = t.NameAr,
                Slug = t.Slug
            }).ToList() ?? new List<TagDto>(),
            Categories = blogPost.Categories?.Select(c => new CategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                Slug = c.Slug,
                DescriptionEn = c.DescriptionEn,
                DescriptionAr = c.DescriptionAr
            }).ToList() ?? new List<CategoryDto>()
        };

        return dto;
    }
}
