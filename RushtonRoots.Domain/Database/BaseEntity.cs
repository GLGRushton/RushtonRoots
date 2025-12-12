namespace RushtonRoots.Domain.Database;

/// <summary>
/// Base entity class that all database entities inherit from.
/// Provides common audit fields.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
