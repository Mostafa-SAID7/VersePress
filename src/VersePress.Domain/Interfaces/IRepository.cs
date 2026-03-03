namespace VersePress.Domain.Interfaces;

/// <summary>
/// Generic repository interface providing async CRUD operations for entities
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its unique identifier
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all entities of type T
    /// </summary>
    /// <returns>Collection of all entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Adds a new entity to the repository
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity with generated values</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity from the repository
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Checks if an entity with the specified identifier exists
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>True if entity exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id);
}
