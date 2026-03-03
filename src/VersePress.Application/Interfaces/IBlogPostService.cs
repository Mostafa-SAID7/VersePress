using VersePress.Application.Commands;
using VersePress.Application.DTOs;

namespace VersePress.Application.Interfaces;

/// <summary>
/// Service interface for blog post management operations
/// </summary>
public interface IBlogPostService
{
    /// <summary>
    /// Creates a new blog post with validation and unique slug generation
    /// </summary>
    /// <param name="command">Blog post creation command</param>
    /// <returns>Created blog post DTO</returns>
    Task<BlogPostDto> CreateBlogPostAsync(CreateBlogPostCommand command);

    /// <summary>
    /// Updates an existing blog post with ownership validation
    /// </summary>
    /// <param name="command">Blog post update command</param>
    /// <param name="currentUserId">ID of the user performing the update</param>
    /// <returns>Updated blog post DTO</returns>
    Task<BlogPostDto> UpdateBlogPostAsync(UpdateBlogPostCommand command, Guid currentUserId);

    /// <summary>
    /// Deletes a blog post with ownership verification
    /// </summary>
    /// <param name="command">Blog post deletion command</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteBlogPostAsync(DeleteBlogPostCommand command);

    /// <summary>
    /// Retrieves a blog post by its URL slug
    /// </summary>
    /// <param name="slug">URL-friendly post identifier</param>
    /// <returns>Blog post DTO if found, null otherwise</returns>
    Task<BlogPostDto?> GetBlogPostBySlugAsync(string slug);

    /// <summary>
    /// Retrieves published blog posts with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of posts per page</param>
    /// <returns>Collection of published blog post DTOs</returns>
    Task<IEnumerable<BlogPostDto>> GetPublishedPostsAsync(int page, int pageSize);

    /// <summary>
    /// Retrieves featured blog posts
    /// </summary>
    /// <param name="count">Maximum number of featured posts to retrieve</param>
    /// <returns>Collection of featured blog post DTOs</returns>
    Task<IEnumerable<BlogPostDto>> GetFeaturedPostsAsync(int count);

    /// <summary>
    /// Toggles the featured status of a blog post
    /// </summary>
    /// <param name="postId">Blog post identifier</param>
    /// <returns>Updated blog post DTO</returns>
    Task<BlogPostDto> ToggleFeaturedAsync(Guid postId);
}
