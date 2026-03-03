using VersePress.Domain.Entities;

namespace VersePress.Domain.Interfaces;

/// <summary>
/// Unit of Work interface coordinating multiple repository operations as a single transaction.
/// Provides access to all repositories and manages transaction boundaries.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository for BlogPost entities with specialized query methods.
    /// </summary>
    IBlogPostRepository BlogPosts { get; }

    /// <summary>
    /// Repository for Comment entities with specialized query methods.
    /// </summary>
    ICommentRepository Comments { get; }

    /// <summary>
    /// Repository for Reaction entities with specialized query methods.
    /// </summary>
    IReactionRepository Reactions { get; }

    /// <summary>
    /// Repository for Notification entities with specialized query methods.
    /// </summary>
    INotificationRepository Notifications { get; }

    /// <summary>
    /// Generic repository for Tag entities.
    /// </summary>
    IRepository<Tag> Tags { get; }

    /// <summary>
    /// Generic repository for Category entities.
    /// </summary>
    IRepository<Category> Categories { get; }

    /// <summary>
    /// Generic repository for Series entities.
    /// </summary>
    IRepository<Series> Series { get; }

    /// <summary>
    /// Generic repository for Project entities.
    /// </summary>
    IRepository<Project> Projects { get; }

    /// <summary>
    /// Generic repository for Share entities.
    /// </summary>
    IRepository<Share> Shares { get; }

    /// <summary>
    /// Generic repository for PostView entities.
    /// </summary>
    IRepository<PostView> PostViews { get; }

    /// <summary>
    /// Saves all pending changes to the database as a single atomic operation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of entities affected</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction, persisting all changes.
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction, discarding all changes.
    /// </summary>
    Task RollbackTransactionAsync();
}
