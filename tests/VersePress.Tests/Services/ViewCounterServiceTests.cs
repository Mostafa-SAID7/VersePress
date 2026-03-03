using Microsoft.Extensions.Caching.Memory;
using Moq;
using VersePress.Application.Services;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;
using Xunit;

namespace VersePress.Tests.Services;

public class ViewCounterServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<PostView>> _mockPostViewRepository;
    private readonly Mock<IBlogPostRepository> _mockBlogPostRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly ViewCounterService _viewCounterService;

    public ViewCounterServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPostViewRepository = new Mock<IRepository<PostView>>();
        _mockBlogPostRepository = new Mock<IBlogPostRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _mockUnitOfWork.Setup(u => u.PostViews).Returns(_mockPostViewRepository.Object);
        _mockUnitOfWork.Setup(u => u.BlogPosts).Returns(_mockBlogPostRepository.Object);

        _viewCounterService = new ViewCounterService(_mockUnitOfWork.Object, _memoryCache);
    }

    [Fact]
    public async Task IncrementViewCountAsync_WithNewSession_ReturnsTrue()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "test-session-123";
        var blogPost = new BlogPost
        {
            Id = blogPostId,
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "Content",
            ContentAr = "محتوى",
            ViewCount = 5,
            AuthorId = Guid.NewGuid()
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView>());

        _mockBlogPostRepository
            .Setup(r => r.GetByIdAsync(blogPostId))
            .ReturnsAsync(blogPost);

        // Act
        var result = await _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IncrementViewCountAsync_WithRecentView_ReturnsFalse()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "test-session-123";
        var recentView = new PostView
        {
            Id = Guid.NewGuid(),
            BlogPostId = blogPostId,
            SessionId = sessionId,
            ViewedAt = DateTime.UtcNow.AddHours(-1) // Viewed 1 hour ago
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView> { recentView });

        // Act
        var result = await _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IncrementViewCountAsync_WithOldView_ReturnsTrue()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "test-session-123";
        var oldView = new PostView
        {
            Id = Guid.NewGuid(),
            BlogPostId = blogPostId,
            SessionId = sessionId,
            ViewedAt = DateTime.UtcNow.AddHours(-25) // Viewed 25 hours ago (outside 24-hour window)
        };
        var blogPost = new BlogPost
        {
            Id = blogPostId,
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "Content",
            ContentAr = "محتوى",
            ViewCount = 10,
            AuthorId = Guid.NewGuid()
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView> { oldView });

        _mockBlogPostRepository
            .Setup(r => r.GetByIdAsync(blogPostId))
            .ReturnsAsync(blogPost);

        // Act
        var result = await _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IncrementViewCountAsync_WithNullSessionId_ThrowsArgumentException()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        string? sessionId = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId!)
        );
    }

    [Fact]
    public async Task IncrementViewCountAsync_WithEmptySessionId_ThrowsArgumentException()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId)
        );
    }

    [Fact]
    public async Task IncrementViewCountAsync_WithWhitespaceSessionId_ThrowsArgumentException()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "   ";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId)
        );
    }

    [Fact]
    public async Task IncrementViewCountAsync_UsesCacheForSubsequentCalls()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "test-session-123";
        var blogPost = new BlogPost
        {
            Id = blogPostId,
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "Content",
            ContentAr = "محتوى",
            ViewCount = 5,
            AuthorId = Guid.NewGuid()
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView>());

        _mockBlogPostRepository
            .Setup(r => r.GetByIdAsync(blogPostId))
            .ReturnsAsync(blogPost);

        // Act - First call
        var result1 = await _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId);
        
        // Wait a bit for the async recording to complete
        await Task.Delay(100);
        
        // Act - Second call (should use cache)
        var result2 = await _viewCounterService.IncrementViewCountAsync(blogPostId, sessionId);

        // Assert
        Assert.True(result1);
        Assert.False(result2); // Second call should return false (already counted)
        
        // Verify GetAllAsync was only called once (first call checks DB, second uses cache)
        _mockPostViewRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task HasViewedRecentlyAsync_WithRecentView_ReturnsTrue()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "test-session-123";
        var recentView = new PostView
        {
            Id = Guid.NewGuid(),
            BlogPostId = blogPostId,
            SessionId = sessionId,
            ViewedAt = DateTime.UtcNow.AddHours(-1)
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView> { recentView });

        // Act
        var result = await _viewCounterService.HasViewedRecentlyAsync(blogPostId, sessionId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasViewedRecentlyAsync_WithOldView_ReturnsFalse()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "test-session-123";
        var oldView = new PostView
        {
            Id = Guid.NewGuid(),
            BlogPostId = blogPostId,
            SessionId = sessionId,
            ViewedAt = DateTime.UtcNow.AddHours(-25)
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView> { oldView });

        // Act
        var result = await _viewCounterService.HasViewedRecentlyAsync(blogPostId, sessionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasViewedRecentlyAsync_WithNoView_ReturnsFalse()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "test-session-123";

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView>());

        // Act
        var result = await _viewCounterService.HasViewedRecentlyAsync(blogPostId, sessionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasViewedRecentlyAsync_WithNullSessionId_ReturnsFalse()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        string? sessionId = null;

        // Act
        var result = await _viewCounterService.HasViewedRecentlyAsync(blogPostId, sessionId!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasViewedRecentlyAsync_WithEmptySessionId_ReturnsFalse()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId = "";

        // Act
        var result = await _viewCounterService.HasViewedRecentlyAsync(blogPostId, sessionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasViewedRecentlyAsync_WithDifferentSession_ReturnsFalse()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var sessionId1 = "session-1";
        var sessionId2 = "session-2";
        var view = new PostView
        {
            Id = Guid.NewGuid(),
            BlogPostId = blogPostId,
            SessionId = sessionId1,
            ViewedAt = DateTime.UtcNow.AddHours(-1)
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView> { view });

        // Act
        var result = await _viewCounterService.HasViewedRecentlyAsync(blogPostId, sessionId2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasViewedRecentlyAsync_WithDifferentPost_ReturnsFalse()
    {
        // Arrange
        var blogPostId1 = Guid.NewGuid();
        var blogPostId2 = Guid.NewGuid();
        var sessionId = "test-session";
        var view = new PostView
        {
            Id = Guid.NewGuid(),
            BlogPostId = blogPostId1,
            SessionId = sessionId,
            ViewedAt = DateTime.UtcNow.AddHours(-1)
        };

        _mockPostViewRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PostView> { view });

        // Act
        var result = await _viewCounterService.HasViewedRecentlyAsync(blogPostId2, sessionId);

        // Assert
        Assert.False(result);
    }
}
