using Microsoft.EntityFrameworkCore.Storage;
using VersePress.Domain.Entities;
using VersePress.Domain.Interfaces;
using VersePress.Infrastructure.Data;

namespace VersePress.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation coordinating multiple repository operations as a single transaction.
/// Manages ApplicationDbContext lifecycle and ensures all changes are committed or rolled back together.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy-loaded repository instances
    private IBlogPostRepository? _blogPosts;
    private ICommentRepository? _comments;
    private IReactionRepository? _reactions;
    private INotificationRepository? _notifications;
    private IRepository<Tag>? _tags;
    private IRepository<Category>? _categories;
    private IRepository<Series>? _series;
    private IRepository<Project>? _projects;
    private IRepository<Share>? _shares;
    private IRepository<PostView>? _postViews;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class.
    /// </summary>
    /// <param name="context">Application database context</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public IBlogPostRepository BlogPosts
    {
        get
        {
            // Lazy initialization - will be implemented when BlogPostRepository is created
            _blogPosts ??= new BlogPostRepository(_context);
            return _blogPosts;
        }
    }

    /// <inheritdoc/>
    public ICommentRepository Comments
    {
        get
        {
            // Lazy initialization - will be implemented when CommentRepository is created
            _comments ??= new CommentRepository(_context);
            return _comments;
        }
    }

    /// <inheritdoc/>
    public IReactionRepository Reactions
    {
        get
        {
            // Lazy initialization - will be implemented when ReactionRepository is created
            _reactions ??= new ReactionRepository(_context);
            return _reactions;
        }
    }

    /// <inheritdoc/>
    public INotificationRepository Notifications
    {
        get
        {
            // Lazy initialization - will be implemented when NotificationRepository is created
            _notifications ??= new NotificationRepository(_context);
            return _notifications;
        }
    }

    /// <inheritdoc/>
    public IRepository<Tag> Tags
    {
        get
        {
            _tags ??= new Repository<Tag>(_context);
            return _tags;
        }
    }

    /// <inheritdoc/>
    public IRepository<Category> Categories
    {
        get
        {
            _categories ??= new Repository<Category>(_context);
            return _categories;
        }
    }

    /// <inheritdoc/>
    public IRepository<Series> Series
    {
        get
        {
            _series ??= new Repository<Series>(_context);
            return _series;
        }
    }

    /// <inheritdoc/>
    public IRepository<Project> Projects
    {
        get
        {
            _projects ??= new Repository<Project>(_context);
            return _projects;
        }
    }

    /// <inheritdoc/>
    public IRepository<Share> Shares
    {
        get
        {
            _shares ??= new Repository<Share>(_context);
            return _shares;
        }
    }

    /// <inheritdoc/>
    public IRepository<PostView> PostViews
    {
        get
        {
            _postViews ??= new Repository<PostView>(_context);
            return _postViews;
        }
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <inheritdoc/>
    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the Unit of Work and releases database connections.
    /// Note: The ApplicationDbContext is managed by dependency injection and should not be disposed here.
    /// Only the transaction is disposed if it exists.
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        // DO NOT dispose _context here - it's managed by DI container
    }
}
