using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;
using VersePress.Infrastructure.Data;

namespace VersePress.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Notification entities with specialized query methods.
/// </summary>
public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    /// <summary>
    /// Initializes a new instance of the NotificationRepository class.
    /// </summary>
    /// <param name="context">Application database context</param>
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly)
    {
        var query = _dbSet.AsNoTracking().Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    /// <inheritdoc/>
    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _dbSet.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
        }
    }
}
