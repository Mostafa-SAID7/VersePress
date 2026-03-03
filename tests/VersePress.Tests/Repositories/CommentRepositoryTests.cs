using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Infrastructure.Data;
using VersePress.Infrastructure.Repositories;

namespace VersePress.Tests.Repositories;

/// <summary>
/// Unit tests for CommentRepository implementation.
/// Tests CRUD operations, nested comments, and approval filtering.
/// </summary>
public class CommentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CommentRepository _repository;
    private readonly User _testUser;
    private readonly BlogPost _testPost;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new CommentRepository(_context);

        // Create test user
        _testUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            FullName = "Test User"
        };
        _context.Users.Add(_testUser);

        // Create test blog post
        _testPost = new BlogPost
        {
            Slug = "test-post",
            TitleEn = "Test Post",
            TitleAr = "منشور تجريبي",
            ContentEn = "This is a test post content with more than 100 characters to meet the validation requirements.",
            ContentAr = "هذا محتوى منشور تجريبي يحتوي على أكثر من 100 حرف لتلبية متطلبات التحقق.",
            AuthorId = _testUser.Id,
            PublishedAt = DateTime.UtcNow
        };
        _context.BlogPosts.Add(_testPost);
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddAsync_ShouldAddComment()
    {
        // Arrange
        var comment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "This is a test comment",
            IsApproved = false
        };

        // Act
        await _repository.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Assert
        var savedComment = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(savedComment);
        Assert.Equal("This is a test comment", savedComment.Content);
        Assert.False(savedComment.IsApproved);
    }

    [Fact]
    public async Task GetCommentsByPostAsync_ShouldReturnTopLevelCommentsWithReplies()
    {
        // Arrange
        var parentComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Parent comment",
            IsApproved = true
        };
        await _repository.AddAsync(parentComment);
        await _context.SaveChangesAsync();

        var replyComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Reply comment",
            ParentCommentId = parentComment.Id,
            IsApproved = true
        };
        await _repository.AddAsync(replyComment);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetCommentsByPostAsync(_testPost.Id);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList); // Only parent comment at top level
        Assert.Equal("Parent comment", resultList[0].Content);
        Assert.Single(resultList[0].Replies);
        Assert.Equal("Reply comment", resultList[0].Replies.First().Content);
    }

    [Fact]
    public async Task GetCommentsByPostAsync_ShouldIncludeUserInformation()
    {
        // Arrange
        var comment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Test comment with user",
            IsApproved = true
        };
        await _repository.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetCommentsByPostAsync(_testPost.Id);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.NotNull(resultList[0].User);
        Assert.Equal("testuser", resultList[0].User.UserName);
    }

    [Fact]
    public async Task GetCommentsByPostAsync_ShouldOrderByCreatedAt()
    {
        // Arrange
        var comment1 = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "First comment",
            IsApproved = true
        };
        await _repository.AddAsync(comment1);
        await _context.SaveChangesAsync();
        await Task.Delay(10); // Ensure different timestamps

        var comment2 = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Second comment",
            IsApproved = true
        };
        await _repository.AddAsync(comment2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetCommentsByPostAsync(_testPost.Id);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("First comment", resultList[0].Content);
        Assert.Equal("Second comment", resultList[1].Content);
    }

    [Fact]
    public async Task GetCommentsByPostAsync_ShouldHandleNestedReplies()
    {
        // Arrange
        var parentComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Parent",
            IsApproved = true
        };
        await _repository.AddAsync(parentComment);
        await _context.SaveChangesAsync();

        var childComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Child",
            ParentCommentId = parentComment.Id,
            IsApproved = true
        };
        await _repository.AddAsync(childComment);
        await _context.SaveChangesAsync();

        var grandchildComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Grandchild",
            ParentCommentId = childComment.Id,
            IsApproved = true
        };
        await _repository.AddAsync(grandchildComment);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetCommentsByPostAsync(_testPost.Id);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList); // Only parent at top level
        Assert.Single(resultList[0].Replies);
        // Note: The current implementation loads one level of replies
        // Deeper nesting would require recursive loading
    }

    [Fact]
    public async Task GetPendingCommentsAsync_ShouldReturnOnlyUnapprovedComments()
    {
        // Arrange
        var approvedComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Approved comment",
            IsApproved = true
        };
        await _repository.AddAsync(approvedComment);

        var pendingComment1 = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Pending comment 1",
            IsApproved = false
        };
        await _repository.AddAsync(pendingComment1);

        var pendingComment2 = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Pending comment 2",
            IsApproved = false
        };
        await _repository.AddAsync(pendingComment2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetPendingCommentsAsync();

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, c => Assert.False(c.IsApproved));
    }

    [Fact]
    public async Task GetPendingCommentsAsync_ShouldIncludeBlogPostAndUser()
    {
        // Arrange
        var pendingComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Pending comment",
            IsApproved = false
        };
        await _repository.AddAsync(pendingComment);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetPendingCommentsAsync();

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.NotNull(resultList[0].User);
        Assert.NotNull(resultList[0].BlogPost);
        Assert.Equal("testuser", resultList[0].User.UserName);
        Assert.Equal("test-post", resultList[0].BlogPost.Slug);
    }

    [Fact]
    public async Task GetPendingCommentCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var approvedComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Approved",
            IsApproved = true
        };
        await _repository.AddAsync(approvedComment);

        for (int i = 0; i < 5; i++)
        {
            var pendingComment = new Comment
            {
                BlogPostId = _testPost.Id,
                UserId = _testUser.Id,
                Content = $"Pending {i}",
                IsApproved = false
            };
            await _repository.AddAsync(pendingComment);
        }
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetPendingCommentCountAsync();

        // Assert
        Assert.Equal(5, count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateComment()
    {
        // Arrange
        var comment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Original content",
            IsApproved = false
        };
        await _repository.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        comment.Content = "Updated content";
        comment.IsApproved = true;
        await _repository.UpdateAsync(comment);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated content", updated.Content);
        Assert.True(updated.IsApproved);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteComment()
    {
        // Arrange
        var comment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "To be deleted",
            IsApproved = true
        };
        await _repository.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(comment);
        await _context.SaveChangesAsync();

        // Assert
        // Verify soft delete
        var allComments = await _context.Comments.IgnoreQueryFilters().ToListAsync();
        var softDeleted = allComments.FirstOrDefault(c => c.Id == comment.Id);
        Assert.NotNull(softDeleted);
        Assert.True(softDeleted.IsDeleted);
        
        // Verify it's filtered out by query filter
        var filteredComments = await _context.Comments.ToListAsync();
        Assert.DoesNotContain(filteredComments, c => c.Id == comment.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotCascadeToReplies()
    {
        // Arrange
        var parentComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Parent",
            IsApproved = true
        };
        await _repository.AddAsync(parentComment);
        await _context.SaveChangesAsync();

        var replyComment = new Comment
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Reply",
            ParentCommentId = parentComment.Id,
            IsApproved = true
        };
        await _repository.AddAsync(replyComment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(parentComment);
        await _context.SaveChangesAsync();

        // Assert
        var reply = await _repository.GetByIdAsync(replyComment.Id);
        Assert.NotNull(reply); // Reply should still exist (DeleteBehavior.Restrict)
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
