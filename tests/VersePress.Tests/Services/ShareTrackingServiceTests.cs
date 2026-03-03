using Moq;
using VersePress.Application.Services;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;
using Xunit;

namespace VersePress.Tests.Services;

public class ShareTrackingServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Share>> _mockShareRepository;
    private readonly Mock<IBlogPostRepository> _mockBlogPostRepository;
    private readonly ShareTrackingService _shareTrackingService;

    public ShareTrackingServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockShareRepository = new Mock<IRepository<Share>>();
        _mockBlogPostRepository = new Mock<IBlogPostRepository>();

        _mockUnitOfWork.Setup(u => u.Shares).Returns(_mockShareRepository.Object);
        _mockUnitOfWork.Setup(u => u.BlogPosts).Returns(_mockBlogPostRepository.Object);

        _shareTrackingService = new ShareTrackingService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task RecordShareAsync_WithValidBlogPost_CompletesSuccessfully()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var platform = Platform.Twitter;
        var blogPost = new BlogPost
        {
            Id = blogPostId,
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "Content",
            ContentAr = "محتوى",
            AuthorId = Guid.NewGuid()
        };

        _mockBlogPostRepository
            .Setup(r => r.GetByIdAsync(blogPostId))
            .ReturnsAsync(blogPost);

        // Act
        await _shareTrackingService.RecordShareAsync(blogPostId, platform);

        // Assert - method should complete without throwing
        _mockBlogPostRepository.Verify(r => r.GetByIdAsync(blogPostId), Times.Once);
    }

    [Fact]
    public async Task RecordShareAsync_WithEmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var blogPostId = Guid.Empty;
        var platform = Platform.Facebook;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _shareTrackingService.RecordShareAsync(blogPostId, platform));
    }

    [Fact]
    public async Task RecordShareAsync_WithNonExistentBlogPost_ThrowsInvalidOperationException()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var platform = Platform.LinkedIn;

        _mockBlogPostRepository
            .Setup(r => r.GetByIdAsync(blogPostId))
            .ReturnsAsync((BlogPost?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _shareTrackingService.RecordShareAsync(blogPostId, platform));
    }

    [Fact]
    public async Task GetShareCountsAsync_WithNoShares_ReturnsZeroForAllPlatforms()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();

        _mockShareRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Share>());

        // Act
        var result = await _shareTrackingService.GetShareCountsAsync(blogPostId);

        // Assert
        Assert.Equal(4, result.Count); // 4 platforms
        Assert.Equal(0, result[Platform.Twitter]);
        Assert.Equal(0, result[Platform.Facebook]);
        Assert.Equal(0, result[Platform.LinkedIn]);
        Assert.Equal(0, result[Platform.WhatsApp]);
    }

    [Fact]
    public async Task GetShareCountsAsync_WithMultipleShares_ReturnsCorrectCounts()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var shares = new List<Share>
        {
            new Share { Id = Guid.NewGuid(), BlogPostId = blogPostId, Platform = Platform.Twitter, SharedAt = DateTime.UtcNow },
            new Share { Id = Guid.NewGuid(), BlogPostId = blogPostId, Platform = Platform.Twitter, SharedAt = DateTime.UtcNow },
            new Share { Id = Guid.NewGuid(), BlogPostId = blogPostId, Platform = Platform.Facebook, SharedAt = DateTime.UtcNow },
            new Share { Id = Guid.NewGuid(), BlogPostId = blogPostId, Platform = Platform.LinkedIn, SharedAt = DateTime.UtcNow },
            new Share { Id = Guid.NewGuid(), BlogPostId = Guid.NewGuid(), Platform = Platform.Twitter, SharedAt = DateTime.UtcNow } // Different post
        };

        _mockShareRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(shares);

        // Act
        var result = await _shareTrackingService.GetShareCountsAsync(blogPostId);

        // Assert
        Assert.Equal(2, result[Platform.Twitter]);
        Assert.Equal(1, result[Platform.Facebook]);
        Assert.Equal(1, result[Platform.LinkedIn]);
        Assert.Equal(0, result[Platform.WhatsApp]);
    }

    [Fact]
    public async Task GetShareCountsAsync_WithEmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var blogPostId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _shareTrackingService.GetShareCountsAsync(blogPostId));
    }

    [Fact]
    public async Task GetTotalShareCountAsync_WithNoShares_ReturnsZero()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();

        _mockShareRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Share>());

        // Act
        var result = await _shareTrackingService.GetTotalShareCountAsync(blogPostId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetTotalShareCountAsync_WithMultipleShares_ReturnsCorrectTotal()
    {
        // Arrange
        var blogPostId = Guid.NewGuid();
        var shares = new List<Share>
        {
            new Share { Id = Guid.NewGuid(), BlogPostId = blogPostId, Platform = Platform.Twitter, SharedAt = DateTime.UtcNow },
            new Share { Id = Guid.NewGuid(), BlogPostId = blogPostId, Platform = Platform.Facebook, SharedAt = DateTime.UtcNow },
            new Share { Id = Guid.NewGuid(), BlogPostId = blogPostId, Platform = Platform.LinkedIn, SharedAt = DateTime.UtcNow },
            new Share { Id = Guid.NewGuid(), BlogPostId = Guid.NewGuid(), Platform = Platform.Twitter, SharedAt = DateTime.UtcNow } // Different post
        };

        _mockShareRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(shares);

        // Act
        var result = await _shareTrackingService.GetTotalShareCountAsync(blogPostId);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetTotalShareCountAsync_WithEmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var blogPostId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _shareTrackingService.GetTotalShareCountAsync(blogPostId));
    }
}
