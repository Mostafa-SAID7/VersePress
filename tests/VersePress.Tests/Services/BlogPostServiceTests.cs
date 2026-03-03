using Moq;
using VersePress.Application.Commands;
using VersePress.Application.Services;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;
using Xunit;

namespace VersePress.Tests.Services;

public class BlogPostServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBlogPostRepository> _mockBlogPostRepository;
    private readonly Mock<IReactionRepository> _mockReactionRepository;
    private readonly Mock<IRepository<Tag>> _mockTagRepository;
    private readonly Mock<IRepository<Category>> _mockCategoryRepository;
    private readonly Mock<IRepository<Series>> _mockSeriesRepository;
    private readonly Mock<IRepository<Project>> _mockProjectRepository;
    private readonly BlogPostService _service;
    private readonly User _testAuthor;

    public BlogPostServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBlogPostRepository = new Mock<IBlogPostRepository>();
        _mockReactionRepository = new Mock<IReactionRepository>();
        _mockTagRepository = new Mock<IRepository<Tag>>();
        _mockCategoryRepository = new Mock<IRepository<Category>>();
        _mockSeriesRepository = new Mock<IRepository<Series>>();
        _mockProjectRepository = new Mock<IRepository<Project>>();

        _mockUnitOfWork.Setup(u => u.BlogPosts).Returns(_mockBlogPostRepository.Object);
        _mockUnitOfWork.Setup(u => u.Reactions).Returns(_mockReactionRepository.Object);
        _mockUnitOfWork.Setup(u => u.Tags).Returns(_mockTagRepository.Object);
        _mockUnitOfWork.Setup(u => u.Categories).Returns(_mockCategoryRepository.Object);
        _mockUnitOfWork.Setup(u => u.Series).Returns(_mockSeriesRepository.Object);
        _mockUnitOfWork.Setup(u => u.Projects).Returns(_mockProjectRepository.Object);

        _service = new BlogPostService(_mockUnitOfWork.Object);

        _testAuthor = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testauthor",
            Email = "test@example.com",
            FullName = "Test Author"
        };
    }

    [Fact]
    public async Task CreateBlogPostAsync_ShouldCreatePostWithUniqueSlug()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "This is a test post content with more than 100 characters to meet the validation requirements.",
            ContentAr = "هذا محتوى منشور تجريبي يحتوي على أكثر من 100 حرف لتلبية متطلبات التحقق.",
            AuthorId = _testAuthor.Id
        };

        var createdPost = new BlogPost
        {
            Id = Guid.NewGuid(),
            Slug = "test-post",
            TitleEn = command.TitleEn,
            TitleAr = command.TitleAr,
            ContentEn = command.ContentEn,
            ContentAr = command.ContentAr,
            AuthorId = command.AuthorId,
            PublishedAt = DateTime.UtcNow
        };

        _mockBlogPostRepository.Setup(r => r.SlugExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        BlogPost? capturedPost = null;
        _mockBlogPostRepository.Setup(r => r.AddAsync(It.IsAny<BlogPost>()))
            .Callback<BlogPost>(p => capturedPost = p)
            .ReturnsAsync((BlogPost p) => p);
        
        _mockReactionRepository.Setup(r => r.GetReactionCountsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Dictionary<ReactionType, int>());
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateBlogPostAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-post", result.Slug);
        _mockBlogPostRepository.Verify(r => r.AddAsync(It.IsAny<BlogPost>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBlogPostAsync_ShouldGenerateUniqueSlugWhenDuplicate()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "This is a test post content with more than 100 characters to meet the validation requirements.",
            ContentAr = "هذا محتوى منشور تجريبي يحتوي على أكثر من 100 حرف لتلبية متطلبات التحقق.",
            AuthorId = _testAuthor.Id
        };

        var createdPost = new BlogPost
        {
            Id = Guid.NewGuid(),
            Slug = "test-post-1",
            TitleEn = command.TitleEn,
            TitleAr = command.TitleAr,
            ContentEn = command.ContentEn,
            ContentAr = command.ContentAr,
            AuthorId = command.AuthorId,
            PublishedAt = DateTime.UtcNow
        };

        _mockBlogPostRepository.SetupSequence(r => r.SlugExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true)  // First slug exists
            .ReturnsAsync(false); // Second slug is unique
        
        _mockBlogPostRepository.Setup(r => r.AddAsync(It.IsAny<BlogPost>()))
            .ReturnsAsync((BlogPost p) => p);
        
        _mockReactionRepository.Setup(r => r.GetReactionCountsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Dictionary<ReactionType, int>());
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateBlogPostAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("test-post", result.Slug); // Should have suffix
        _mockBlogPostRepository.Verify(r => r.SlugExistsAsync(It.IsAny<string>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task GetBlogPostBySlugAsync_ShouldReturnPostWithReactionCounts()
    {
        // Arrange
        var slug = "test-post";
        var blogPost = new BlogPost
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "Test content",
            ContentAr = "محتوى تجريبي",
            AuthorId = _testAuthor.Id,
            PublishedAt = DateTime.UtcNow
        };

        var reactionCounts = new Dictionary<ReactionType, int>
        {
            { ReactionType.Like, 5 },
            { ReactionType.Love, 3 }
        };

        _mockBlogPostRepository.Setup(r => r.GetBySlugAsync(slug))
            .ReturnsAsync(blogPost);
        _mockReactionRepository.Setup(r => r.GetReactionCountsAsync(blogPost.Id))
            .ReturnsAsync(reactionCounts);

        // Act
        var result = await _service.GetBlogPostBySlugAsync(slug);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slug, result.Slug);
        Assert.Equal(5, result.ReactionCounts["Like"]);
        Assert.Equal(3, result.ReactionCounts["Love"]);
    }

    [Fact]
    public async Task UpdateBlogPostAsync_ShouldUpdatePost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new UpdateBlogPostCommand
        {
            Id = postId,
            TitleEn = "Updated Title",
            TitleAr = "عنوان محدث",
            ContentEn = "Updated content with more than 100 characters to meet validation requirements.",
            ContentAr = "محتوى محدث يحتوي على أكثر من 100 حرف لتلبية متطلبات التحقق."
        };

        var existingPost = new BlogPost
        {
            Id = postId,
            Slug = "test-post",
            TitleEn = "Old Title",
            TitleAr = "عنوان قديم",
            ContentEn = "Old content",
            ContentAr = "محتوى قديم",
            AuthorId = _testAuthor.Id,
            Tags = new List<Tag>(),
            Categories = new List<Category>()
        };

        _mockBlogPostRepository.Setup(r => r.GetByIdAsync(postId))
            .ReturnsAsync(existingPost);
        
        _mockBlogPostRepository.Setup(r => r.UpdateAsync(It.IsAny<BlogPost>()))
            .Callback<BlogPost>(p => { /* Update happens in place */ })
            .Returns(Task.CompletedTask);
        
        _mockReactionRepository.Setup(r => r.GetReactionCountsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Dictionary<ReactionType, int>());
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateBlogPostAsync(command, _testAuthor.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.TitleEn);
        _mockBlogPostRepository.Verify(r => r.UpdateAsync(It.IsAny<BlogPost>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBlogPostAsync_ShouldDeletePost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new DeleteBlogPostCommand
        {
            Id = postId,
            AuthorId = _testAuthor.Id
        };

        var existingPost = new BlogPost
        {
            Id = postId,
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "Test content",
            ContentAr = "محتوى تجريبي",
            AuthorId = _testAuthor.Id
        };

        _mockBlogPostRepository.Setup(r => r.GetByIdAsync(postId))
            .ReturnsAsync(existingPost);
        
        _mockBlogPostRepository.Setup(r => r.DeleteAsync(It.IsAny<BlogPost>()))
            .Callback<BlogPost>(p => { /* Delete happens */ })
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteBlogPostAsync(command);

        // Assert
        Assert.True(result);
        _mockBlogPostRepository.Verify(r => r.DeleteAsync(existingPost), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleFeaturedAsync_ShouldToggleFeaturedStatus()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var existingPost = new BlogPost
        {
            Id = postId,
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "Test content",
            ContentAr = "محتوى تجريبي",
            AuthorId = _testAuthor.Id,
            IsFeatured = false
        };

        _mockBlogPostRepository.Setup(r => r.GetByIdAsync(postId))
            .ReturnsAsync(existingPost);
        
        _mockBlogPostRepository.Setup(r => r.UpdateAsync(It.IsAny<BlogPost>()))
            .Callback<BlogPost>(p => { /* Update happens in place */ })
            .Returns(Task.CompletedTask);
        
        _mockReactionRepository.Setup(r => r.GetReactionCountsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Dictionary<ReactionType, int>());
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.ToggleFeaturedAsync(postId);

        // Assert
        Assert.NotNull(result);
        Assert.True(existingPost.IsFeatured);
        _mockBlogPostRepository.Verify(r => r.UpdateAsync(existingPost), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}