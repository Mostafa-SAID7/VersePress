using Microsoft.EntityFrameworkCore;
using VersePress.Domain.Interfaces;
using VersePress.Infrastructure.Data;

namespace VersePress.Infrastructure.Repositories;

/// <summary>
/// Generic repository base class implementing IRepository<T> with async EF Core operations.
/// Provides common CRUD operations for all entities.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    /// <param name="context">Application database context</param>
    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<T> AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity);
        return entity;
    }

    /// <inheritdoc/>
    public virtual Task UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task DeleteAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }
}
