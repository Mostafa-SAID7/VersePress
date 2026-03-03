using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Infrastructure.Data;
using VersePress.Infrastructure.Repositories;

namespace VersePress.Tests.Repositories;

/// <summary>
/// Unit tests for BlogPostRepository implementation.
/// Tests CRUD operations and specialized query methods with in-memory database.
/// </summary>
public class BlogPostRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BlogPostRepository _repository;
    private readonly User _testAuthor;

    public BlogPostRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BlogPostRepository(_context);

        // Create test author
        _testAuthor = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testauthor",
            Email = "test@example.com",
            FullName = "Test Author"
        };
        _context.Users.Add(_testAuthor);
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddAsync_ShouldAddBlogPost()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "This is a test post content with more than 100 characters to meet the validation requirements for blog post content.",
            ContentAr = "هذا محتوى منشور تجريبي يحتوي على أكثر من 100 حرف لتلبية متطلبات التحقق من صحة محتوى منشور المدونة.",
            AuthorId = _testAuthor.Id,
            PublishedAt = DateTime.UtcNow
        };

        // Act
        await _repository.AddAsync(blogPost);
        await _context.SaveChangesAsync();

        // Assert
        var savedPost = await _context.BlogPosts.FindAsync(blogPost.Id);
        Assert.NotNull(savedPost);
        Assert.Equal("test-post", savedPost.Slug);
        Assert.Equal("Test Post", savedPost.TitleEn);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBlogPost()
    {
        // Arrange
        var blogPost = CreateTestBlogPost("get-by-id-test");
        await _repository.AddAsync(blogPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(blogPost.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(blogPost.Id, result.Id);
        Assert.Equal("get-by-id-test", result.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnBlogPostWithRelatedEntities()
    {
        // Arrange
        var tag = new Tag { NameEn = "Technology", NameAr = "تقنية", Slug = "technology" };
        var category = new Category { NameEn = "Programming", NameAr = "برمجة", Slug = "programming" };
        _context.Tags.Add(tag);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var blogPost = CreateTestBlogPost("slug-test");
        blogPost.Tags.Add(tag);
        blogPost.Categories.Add(category);
        await _repository.AddAsync(blogPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySlugAsync("slug-test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("slug-test", result.Slug);
        Assert.NotNull(result.Author);
        Assert.Single(result.Tags);
        Assert.Single(result.Categories);
    }

    [Fact]
    public async Task GetPublishedPostsAsync_ShouldReturnOnlyPublishedPosts()
    {
        // Arrange
        var publishedPost1 = CreateTestBlogPost("published-1");
        publishedPost1.PublishedAt = DateTime.UtcNow.AddDays(-2);
        
        var publishedPost2 = CreateTestBlogPost("published-2");
        publishedPost2.PublishedAt = DateTime.UtcNow.AddDays(-1);
        
        var unpublishedPost = CreateTestBlogPost("unpublished");
        unpublishedPost.PublishedAt = null;
        
        var futurePost = CreateTestBlogPost("future");
        futurePost.PublishedAt = DateTime.UtcNow.AddDays(1);

        await _repository.AddAsync(publishedPost1);
        await _repository.AddAsync(publishedPost2);
        await _repository.AddAsync(unpublishedPost);
        await _repository.AddAsync(futurePost);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetPublishedPostsAsync(page: 1, pageSize: 10);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("published-2", resultList[0].Slug); // Most recent first
        Assert.Equal("published-1", resultList[1].Slug);
    }

    [Fact]
    public async Task GetPublishedPostsAsync_ShouldPaginateCorrectly()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
        {
            var post = CreateTestBlogPost($"post-{i}");
            post.PublishedAt = DateTime.UtcNow.AddDays(-i);
            await _repository.AddAsync(post);
        }
        await _context.SaveChangesAsync();

        // Act
        var page1 = await _repository.GetPublishedPostsAsync(page: 1, pageSize: 10);
        var page2 = await _repository.GetPublishedPostsAsync(page: 2, pageSize: 10);

        // Assert
        Assert.Equal(10, page1.Count());
        Assert.Equal(5, page2.Count());
    }

    [Fact]
    public async Task GetFeaturedPostsAsync_ShouldReturnOnlyFeaturedPosts()
    {
        // Arrange
        var featuredPost1 = CreateTestBlogPost("featured-1");
        featuredPost1.IsFeatured = true;
        featuredPost1.PublishedAt = DateTime.UtcNow.AddDays(-2);
        
        var featuredPost2 = CreateTestBlogPost("featured-2");
        featuredPost2.IsFeatured = true;
        featuredPost2.PublishedAt = DateTime.UtcNow.AddDays(-1);
        
        var normalPost = CreateTestBlogPost("normal");
        normalPost.IsFeatured = false;
        normalPost.PublishedAt = DateTime.UtcNow;

        await _repository.AddAsync(featuredPost1);
        await _repository.AddAsync(featuredPost2);
        await _repository.AddAsync(normalPost);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetFeaturedPostsAsync(count: 3);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, post => Assert.True(post.IsFeatured));
    }

    [Fact]
    public async Task GetPostsByAuthorAsync_ShouldReturnAuthorPosts()
    {
        // Arrange
        var author2 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "author2",
            Email = "author2@example.com",
            FullName = "Author Two"
        };
        _context.Users.Add(author2);
        await _context.SaveChangesAsync();

        var post1 = CreateTestBlogPost("author1-post1");
        var post2 = CreateTestBlogPost("author1-post2");
        var post3 = CreateTestBlogPost("author2-post");
        post3.AuthorId = author2.Id;

        await _repository.AddAsync(post1);
        await _repository.AddAsync(post2);
        await _repository.AddAsync(post3);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetPostsByAuthorAsync(_testAuthor.Id);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, post => Assert.Equal(_testAuthor.Id, post.AuthorId));
    }

    [Fact]
    public async Task SearchPostsAsync_ShouldSearchInEnglishContent()
    {
        // Arrange
        var post1 = CreateTestBlogPost("search-1");
        post1.TitleEn = "ASP.NET Core Tutorial";
        post1.ContentEn = "Learn how to build web applications with ASP.NET Core framework.";
        post1.PublishedAt = DateTime.UtcNow;

        var post2 = CreateTestBlogPost("search-2");
        post2.TitleEn = "Python Programming";
        post2.ContentEn = "Introduction to Python programming language.";
        post2.PublishedAt = DateTime.UtcNow;

        await _repository.AddAsync(post1);
        await _repository.AddAsync(post2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchPostsAsync("ASP.NET");

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Equal("search-1", resultList[0].Slug);
    }

    [Fact]
    public async Task SearchPostsAsync_ShouldSearchInArabicContent()
    {
        // Arrange
        var post1 = CreateTestBlogPost("arabic-search-1");
        post1.TitleAr = "تعلم البرمجة";
        post1.ContentAr = "دليل شامل لتعلم البرمجة باستخدام لغة سي شارب.";
        post1.PublishedAt = DateTime.UtcNow;

        var post2 = CreateTestBlogPost("arabic-search-2");
        post2.TitleAr = "تطوير الويب";
        post2.ContentAr = "مقدمة في تطوير تطبيقات الويب الحديثة.";
        post2.PublishedAt = DateTime.UtcNow;

        await _repository.AddAsync(post1);
        await _repository.AddAsync(post2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchPostsAsync("البرمجة");

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Equal("arabic-search-1", resultList[0].Slug);
    }

    [Fact]
    public async Task SlugExistsAsync_ShouldReturnTrueForExistingSlug()
    {
        // Arrange
        var post = CreateTestBlogPost("existing-slug");
        await _repository.AddAsync(post);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.SlugExistsAsync("existing-slug");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task SlugExistsAsync_ShouldReturnFalseForNonExistingSlug()
    {
        // Act
        var exists = await _repository.SlugExistsAsync("non-existing-slug");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBlogPost()
    {
        // Arrange
        var post = CreateTestBlogPost("update-test");
        await _repository.AddAsync(post);
        await _context.SaveChangesAsync();

        // Act
        post.TitleEn = "Updated Title";
        post.ViewCount = 100;
        await _repository.UpdateAsync(post);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(post.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated.TitleEn);
        Assert.Equal(100, updated.ViewCount);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteBlogPost()
    {
        // Arrange
        var post = CreateTestBlogPost("delete-test");
        await _repository.AddAsync(post);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(post);
        await _context.SaveChangesAsync();

        // Assert
        // Verify it's soft deleted, not hard deleted
        var allPosts = await _context.BlogPosts.IgnoreQueryFilters().ToListAsync();
        var softDeleted = allPosts.FirstOrDefault(p => p.Id == post.Id);
        Assert.NotNull(softDeleted);
        Assert.True(softDeleted.IsDeleted);
        
        // Verify it's filtered out by query filter
        var filteredPosts = await _context.BlogPosts.ToListAsync();
        Assert.DoesNotContain(filteredPosts, p => p.Id == post.Id);
    }

    [Fact]
    public async Task CascadeDelete_ShouldDeleteCommentsWhenBlogPostDeleted()
    {
        // Arrange
        var post = CreateTestBlogPost("cascade-test");
        await _repository.AddAsync(post);
        await _context.SaveChangesAsync();

        var comment = new Comment
        {
            BlogPostId = post.Id,
            UserId = _testAuthor.Id,
            Content = "Test comment",
            IsApproved = true
        };
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(post);
        await _context.SaveChangesAsync();

        // Assert
        var deletedComment = await _context.Comments.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == comment.Id);
        Assert.NotNull(deletedComment);
        Assert.True(deletedComment.IsDeleted); // Comment should be soft deleted via cascade
    }

    private BlogPost CreateTestBlogPost(string slug)
    {
        return new BlogPost
        {
            Slug = slug,
            TitleEn = $"Test Post {slug}",
            TitleAr = $"منشور تجريبي {slug}",
            ContentEn = "This is a test post content with more than 100 characters to meet the validation requirements for blog post content.",
            ContentAr = "هذا محتوى منشور تجريبي يحتوي على أكثر من 100 حرف لتلبية متطلبات التحقق من صحة محتوى منشور المدونة.",
            AuthorId = _testAuthor.Id,
            PublishedAt = DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
