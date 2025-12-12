namespace RushtonRoots.Domain.Database;

/// <summary>
/// Stores which persons in a household have ADMIN/EDITOR roles.
/// Only the anchor admin can grant permissions.
/// </summary>
public class HouseholdPermission : BaseEntity
{
    public int HouseholdId { get; set; }
    public int PersonId { get; set; }
    public string Role { get; set; } = string.Empty; // 'ADMIN' or 'EDITOR'
    
    // Navigation properties
    public Household? Household { get; set; }
    public Person? Person { get; set; }
}
