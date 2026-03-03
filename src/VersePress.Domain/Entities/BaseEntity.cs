namespace VersePress.Domain.Entities;

/// <summary>
/// Base entity class providing common properties for all domain entities.
/// Implements soft delete pattern with audit timestamps.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier using GUID for distributed system support.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the entity was last modified.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag. When true, entity is logically deleted but remains in database.
    /// </summary>
    public bool IsDeleted { get; set; }
}
