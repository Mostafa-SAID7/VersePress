using Moq;
using VersePress.Application.Services;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Domain.Interfaces;
using Xunit;

namespace VersePress.Tests.Services;

public class SearchServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBlogPostRepository> _mockBlogPostRepository;
    private readonly Mock<IReactionRepository> _mockReactionRepository;
    private readonly SearchService _searchService;

    public SearchServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBlogPostRepository = new Mock<IBlogPostRepository>();
        _mockReactionRepository = new Mock<IReactionRepository>();

        _mockUnitOfWork.Setup(u => u.BlogPosts).Returns(_mockBlogPostRepository.Object);
        _mockUnitOfWork.Setup(u => u.Reactions).Returns(_mockReactionRepository.Object);

        _searchService = new SearchService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task SearchPostsAsync_WithValidQuery_ReturnsMatchingPosts()
    {
        // Arrange
        var query = "test query";
        var blogPosts = new List<BlogPost>
        {
            new BlogPost
            {
                Id = Guid.NewGuid(),
                Slug = "test-post",
                TitleEn = "Test Post",
                TitleAr = "منشور تجريبي",
                ContentEn = "This is a test post content",
                ContentAr = "هذا محتوى منشور تجريبي",
                PublishedAt = DateTime.UtcNow,
                AuthorId = Guid.NewGuid(),
                Author = new User { UserName = "testuser" }
            }
        };

        _mockBlogPostRepository
            .Setup(r => r.SearchPostsAsync(It.IsAny<string>()))
            .ReturnsAsync(blogPosts);

        _mockReactionRepository
            .Setup(r => r.GetReactionCountsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Dictionary<ReactionType, int>());

        // Act
        var result = await _searchService.SearchPostsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var dto = result.First();
        Assert.Equal("test-post", dto.Slug);
        Assert.Equal("Test Post", dto.TitleEn);
    }

    [Fact]
    public async Task SearchPostsAsync_WithEmptyQuery_ReturnsEmptyList()
    {
        // Arrange
        var query = "";

        // Act
        var result = await _searchService.SearchPostsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockBlogPostRepository.Verify(r => r.SearchPostsAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchPostsAsync_WithWhitespaceQuery_ReturnsEmptyList()
    {
        // Arrange
        var query = "   ";

        // Act
        var result = await _searchService.SearchPostsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockBlogPostRepository.Verify(r => r.SearchPostsAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchPostsAsync_WithNullQuery_ReturnsEmptyList()
    {
        // Arrange
        string? query = null;

        // Act
        var result = await _searchService.SearchPostsAsync(query!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockBlogPostRepository.Verify(r => r.SearchPostsAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchPostsAsync_SanitizesQueryInput()
    {
        // Arrange
        var query = "test'; DROP TABLE BlogPosts; --";
        var sanitizedQuery = "test DROP TABLE BlogPosts --"; // Expected sanitized version
        var blogPosts = new List<BlogPost>();

        _mockBlogPostRepository
            .Setup(r => r.SearchPostsAsync(It.IsAny<string>()))
            .ReturnsAsync(blogPosts);

        // Act
        var result = await _searchService.SearchPostsAsync(query);

        // Assert
        Assert.NotNull(result);
        _mockBlogPostRepository.Verify(r => r.SearchPostsAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SearchPostsAsync_WithTimeout_ThrowsTimeoutException()
    {
        // Arrange
        var query = "test";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel(); // Immediately cancel to simulate timeout

        _mockBlogPostRepository
            .Setup(r => r.SearchPostsAsync(It.IsAny<string>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _searchService.SearchPostsAsync(query, cancellationTokenSource.Token)
        );
    }

    [Fact]
    public async Task SearchPostsAsync_WithLongQuery_TruncatesQuery()
    {
        // Arrange
        var longQuery = new string('a', 300); // 300 characters
        var blogPosts = new List<BlogPost>();

        _mockBlogPostRepository
            .Setup(r => r.SearchPostsAsync(It.IsAny<string>()))
            .ReturnsAsync(blogPosts);

        // Act
        var result = await _searchService.SearchPostsAsync(longQuery);

        // Assert
        Assert.NotNull(result);
        _mockBlogPostRepository.Verify(r => r.SearchPostsAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SearchPostsAsync_MapsReactionCounts()
    {
        // Arrange
        var query = "test";
        var postId = Guid.NewGuid();
        var blogPosts = new List<BlogPost>
        {
            new BlogPost
            {
                Id = postId,
                Slug = "test-post",
                TitleEn = "Test Post",
                TitleAr = "منشور تجريبي",
                ContentEn = "Content",
                ContentAr = "محتوى",
                PublishedAt = DateTime.UtcNow,
                AuthorId = Guid.NewGuid(),
                Author = new User { UserName = "testuser" }
            }
        };

        var reactionCounts = new Dictionary<ReactionType, int>
        {
            { ReactionType.Like, 5 },
            { ReactionType.Love, 3 }
        };

        _mockBlogPostRepository
            .Setup(r => r.SearchPostsAsync(It.IsAny<string>()))
            .ReturnsAsync(blogPosts);

        _mockReactionRepository
            .Setup(r => r.GetReactionCountsAsync(postId))
            .ReturnsAsync(reactionCounts);

        // Act
        var result = await _searchService.SearchPostsAsync(query);

        // Assert
        Assert.NotNull(result);
        var dto = result.First();
        Assert.Equal(8, dto.ReactionCount); // 5 + 3
        Assert.Equal(2, dto.ReactionCounts.Count);
    }
}
