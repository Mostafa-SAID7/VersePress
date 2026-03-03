using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Infrastructure.Data;
using VersePress.Infrastructure.Repositories;

namespace VersePress.Tests.Repositories;

/// <summary>
/// Unit tests for ReactionRepository implementation.
/// Tests CRUD operations and reaction aggregation methods.
/// </summary>
public class ReactionRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ReactionRepository _repository;
    private readonly User _testUser;
    private readonly BlogPost _testPost;

    public ReactionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ReactionRepository(_context);

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
    public async Task AddAsync_ShouldAddReaction()
    {
        // Arrange
        var reaction = new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            ReactionType = ReactionType.Like
        };

        // Act
        await _repository.AddAsync(reaction);
        await _context.SaveChangesAsync();

        // Assert
        var savedReaction = await _context.Reactions.FindAsync(reaction.Id);
        Assert.NotNull(savedReaction);
        Assert.Equal(ReactionType.Like, savedReaction.ReactionType);
        Assert.Equal(_testUser.Id, savedReaction.UserId);
        Assert.Equal(_testPost.Id, savedReaction.BlogPostId);
    }

    [Fact]
    public async Task GetUserReactionAsync_ShouldReturnUserReaction()
    {
        // Arrange
        var reaction = new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            ReactionType = ReactionType.Love
        };
        await _repository.AddAsync(reaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserReactionAsync(_testPost.Id, _testUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ReactionType.Love, result.ReactionType);
        Assert.Equal(_testUser.Id, result.UserId);
    }

    [Fact]
    public async Task GetUserReactionAsync_ShouldReturnNullWhenNoReaction()
    {
        // Act
        var result = await _repository.GetUserReactionAsync(_testPost.Id, _testUser.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetReactionCountsAsync_ShouldReturnCorrectCounts()
    {
        // Arrange
        var user2 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "user2",
            Email = "user2@example.com",
            FullName = "User Two"
        };
        var user3 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "user3",
            Email = "user3@example.com",
            FullName = "User Three"
        };
        _context.Users.AddRange(user2, user3);
        await _context.SaveChangesAsync();

        // Add reactions
        await _repository.AddAsync(new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            ReactionType = ReactionType.Like
        });
        await _repository.AddAsync(new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = user2.Id,
            ReactionType = ReactionType.Like
        });
        await _repository.AddAsync(new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = user3.Id,
            ReactionType = ReactionType.Love
        });
        await _context.SaveChangesAsync();

        // Act
        var counts = await _repository.GetReactionCountsAsync(_testPost.Id);

        // Assert
        Assert.Equal(2, counts[ReactionType.Like]);
        Assert.Equal(1, counts[ReactionType.Love]);
        Assert.False(counts.ContainsKey(ReactionType.Celebrate));
    }

    [Fact]
    public async Task GetReactionCountsAsync_ShouldReturnEmptyDictionaryWhenNoReactions()
    {
        // Act
        var counts = await _repository.GetReactionCountsAsync(_testPost.Id);

        // Assert
        Assert.Empty(counts);
    }

    [Fact]
    public async Task GetReactionCountsAsync_ShouldHandleAllReactionTypes()
    {
        // Arrange
        var users = new List<User>();
        for (int i = 0; i < 5; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = $"user{i}",
                Email = $"user{i}@example.com",
                FullName = $"User {i}"
            });
        }
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Add one of each reaction type
        var reactionTypes = new[]
        {
            ReactionType.Like,
            ReactionType.Love,
            ReactionType.Celebrate,
            ReactionType.Insightful,
            ReactionType.Curious
        };

        for (int i = 0; i < reactionTypes.Length; i++)
        {
            await _repository.AddAsync(new Reaction
            {
                BlogPostId = _testPost.Id,
                UserId = users[i].Id,
                ReactionType = reactionTypes[i]
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var counts = await _repository.GetReactionCountsAsync(_testPost.Id);

        // Assert
        Assert.Equal(5, counts.Count);
        Assert.All(reactionTypes, type => Assert.Equal(1, counts[type]));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateReactionType()
    {
        // Arrange
        var reaction = new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            ReactionType = ReactionType.Like
        };
        await _repository.AddAsync(reaction);
        await _context.SaveChangesAsync();

        // Act
        reaction.ReactionType = ReactionType.Love;
        await _repository.UpdateAsync(reaction);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(reaction.Id);
        Assert.NotNull(updated);
        Assert.Equal(ReactionType.Love, updated.ReactionType);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteReaction()
    {
        // Arrange
        var reaction = new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            ReactionType = ReactionType.Like
        };
        await _repository.AddAsync(reaction);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(reaction);
        await _context.SaveChangesAsync();

        // Assert
        // Verify soft delete
        var allReactions = await _context.Reactions.IgnoreQueryFilters().ToListAsync();
        var softDeleted = allReactions.FirstOrDefault(r => r.Id == reaction.Id);
        Assert.NotNull(softDeleted);
        Assert.True(softDeleted.IsDeleted);
        
        // Verify it's filtered out by query filter
        var filteredReactions = await _context.Reactions.ToListAsync();
        Assert.DoesNotContain(filteredReactions, r => r.Id == reaction.Id);
    }

    [Fact]
    public async Task CascadeDelete_ShouldDeleteReactionsWhenBlogPostDeleted()
    {
        // Arrange
        var reaction = new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            ReactionType = ReactionType.Like
        };
        await _repository.AddAsync(reaction);
        await _context.SaveChangesAsync();

        // Act
        _context.BlogPosts.Remove(_testPost);
        await _context.SaveChangesAsync();

        // Assert
        var deletedReaction = await _context.Reactions.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == reaction.Id);
        Assert.NotNull(deletedReaction);
        Assert.True(deletedReaction.IsDeleted); // Reaction should be soft deleted via cascade
    }

    [Fact]
    public async Task GetReactionCountsAsync_ShouldNotIncludeSoftDeletedReactions()
    {
        // Arrange
        var user2 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "user2",
            Email = "user2@example.com",
            FullName = "User Two"
        };
        _context.Users.Add(user2);
        await _context.SaveChangesAsync();

        var reaction1 = new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = _testUser.Id,
            ReactionType = ReactionType.Like
        };
        var reaction2 = new Reaction
        {
            BlogPostId = _testPost.Id,
            UserId = user2.Id,
            ReactionType = ReactionType.Like
        };
        await _repository.AddAsync(reaction1);
        await _repository.AddAsync(reaction2);
        await _context.SaveChangesAsync();

        // Soft delete one reaction
        await _repository.DeleteAsync(reaction1);
        await _context.SaveChangesAsync();

        // Act
        var counts = await _repository.GetReactionCountsAsync(_testPost.Id);

        // Assert
        Assert.Equal(1, counts[ReactionType.Like]); // Only one reaction should be counted
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
