using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Domain.Enums;
using VersePress.Infrastructure.Data;
using VersePress.Infrastructure.Repositories;

namespace VersePress.Tests.Repositories;

/// <summary>
/// Unit tests for NotificationRepository implementation.
/// Tests CRUD operations and notification query methods.
/// </summary>
public class NotificationRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly NotificationRepository _repository;
    private readonly User _testUser;

    public NotificationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new NotificationRepository(_context);

        // Create test user
        _testUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            FullName = "Test User"
        };
        _context.Users.Add(_testUser);
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddAsync_ShouldAddNotification()
    {
        // Arrange
        var notification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "Someone commented on your post",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };

        // Act
        await _repository.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Assert
        var savedNotification = await _context.Notifications.FindAsync(notification.Id);
        Assert.NotNull(savedNotification);
        Assert.Equal(NotificationType.NewComment, savedNotification.Type);
        Assert.Equal("Someone commented on your post", savedNotification.Content);
        Assert.False(savedNotification.IsRead);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnAllNotifications()
    {
        // Arrange
        var notification1 = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "Notification 1",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        var notification2 = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.CommentReply,
            Content = "Notification 2",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = true
        };
        await _repository.AddAsync(notification1);
        await _repository.AddAsync(notification2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetUserNotificationsAsync(_testUser.Id, unreadOnly: false);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnOnlyUnreadWhenFiltered()
    {
        // Arrange
        var unreadNotification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "Unread notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        var readNotification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.CommentReply,
            Content = "Read notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = true
        };
        await _repository.AddAsync(unreadNotification);
        await _repository.AddAsync(readNotification);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetUserNotificationsAsync(_testUser.Id, unreadOnly: true);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.False(resultList[0].IsRead);
        Assert.Equal("Unread notification", resultList[0].Content);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldOrderByCreatedAtDescending()
    {
        // Arrange
        var notification1 = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "First notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        await _repository.AddAsync(notification1);
        await _context.SaveChangesAsync();
        await Task.Delay(10); // Ensure different timestamps

        var notification2 = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.CommentReply,
            Content = "Second notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        await _repository.AddAsync(notification2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetUserNotificationsAsync(_testUser.Id, unreadOnly: false);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Second notification", resultList[0].Content); // Most recent first
        Assert.Equal("First notification", resultList[1].Content);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldOnlyReturnUserNotifications()
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

        var user1Notification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "User 1 notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        var user2Notification = new Notification
        {
            UserId = user2.Id,
            Type = NotificationType.NewComment,
            Content = "User 2 notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        await _repository.AddAsync(user1Notification);
        await _repository.AddAsync(user2Notification);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetUserNotificationsAsync(_testUser.Id, unreadOnly: false);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Equal("User 1 notification", resultList[0].Content);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        for (int i = 0; i < 3; i++)
        {
            await _repository.AddAsync(new Notification
            {
                UserId = _testUser.Id,
                Type = NotificationType.NewComment,
                Content = $"Unread {i}",
                RelatedEntityId = Guid.NewGuid(),
                IsRead = false
            });
        }
        for (int i = 0; i < 2; i++)
        {
            await _repository.AddAsync(new Notification
            {
                UserId = _testUser.Id,
                Type = NotificationType.CommentReply,
                Content = $"Read {i}",
                RelatedEntityId = Guid.NewGuid(),
                IsRead = true
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetUnreadCountAsync(_testUser.Id);

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldReturnZeroWhenNoUnreadNotifications()
    {
        // Arrange
        var readNotification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "Read notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = true
        };
        await _repository.AddAsync(readNotification);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetUnreadCountAsync(_testUser.Id);

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldUpdateIsReadFlag()
    {
        // Arrange
        var notification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "Unread notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        await _repository.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Act
        await _repository.MarkAsReadAsync(notification.Id);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(notification.Id);
        Assert.NotNull(updated);
        Assert.True(updated.IsRead);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldHandleNonExistentNotification()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert - Should not throw
        await _repository.MarkAsReadAsync(nonExistentId);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteNotification()
    {
        // Arrange
        var notification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "To be deleted",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        await _repository.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(notification);
        await _context.SaveChangesAsync();

        // Assert
        // Verify soft delete
        var allNotifications = await _context.Notifications.IgnoreQueryFilters().ToListAsync();
        var softDeleted = allNotifications.FirstOrDefault(n => n.Id == notification.Id);
        Assert.NotNull(softDeleted);
        Assert.True(softDeleted.IsDeleted);
        
        // Verify it's filtered out by query filter
        var filteredNotifications = await _context.Notifications.ToListAsync();
        Assert.DoesNotContain(filteredNotifications, n => n.Id == notification.Id);
    }

    [Fact]
    public async Task CascadeDelete_ShouldDeleteNotificationsWhenUserDeleted()
    {
        // Arrange
        var notification = new Notification
        {
            UserId = _testUser.Id,
            Type = NotificationType.NewComment,
            Content = "Test notification",
            RelatedEntityId = Guid.NewGuid(),
            IsRead = false
        };
        await _repository.AddAsync(notification);
        await _context.SaveChangesAsync();

        // Act
        _context.Users.Remove(_testUser);
        await _context.SaveChangesAsync();

        // Assert
        // In-memory database may not fully support cascade delete behavior
        // Verify that the notification is either deleted or soft-deleted
        var allNotifications = await _context.Notifications.IgnoreQueryFilters().ToListAsync();
        var notificationAfterDelete = allNotifications.FirstOrDefault(n => n.Id == notification.Id);
        
        // The notification should either be hard deleted (null) or soft deleted (IsDeleted = true)
        if (notificationAfterDelete != null)
        {
            Assert.True(notificationAfterDelete.IsDeleted);
        }
        // If null, it was hard deleted which is also acceptable for cascade behavior
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldHandleAllNotificationTypes()
    {
        // Arrange
        var notificationTypes = new[]
        {
            NotificationType.NewComment,
            NotificationType.CommentReply,
            NotificationType.NewReaction
        };

        foreach (var type in notificationTypes)
        {
            await _repository.AddAsync(new Notification
            {
                UserId = _testUser.Id,
                Type = type,
                Content = $"Notification of type {type}",
                RelatedEntityId = Guid.NewGuid(),
                IsRead = false
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetUserNotificationsAsync(_testUser.Id, unreadOnly: false);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(3, resultList.Count);
        Assert.Contains(resultList, n => n.Type == NotificationType.NewComment);
        Assert.Contains(resultList, n => n.Type == NotificationType.CommentReply);
        Assert.Contains(resultList, n => n.Type == NotificationType.NewReaction);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
