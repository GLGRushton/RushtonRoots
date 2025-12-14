namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents permissions for accessing a media file.
/// Permissions can be granted to individual users or entire households.
/// </summary>
public class MediaPermission : BaseEntity
{
    public int MediaId { get; set; }
    public string? UserId { get; set; } // FK to ApplicationUser (optional - either user or household)
    public int? HouseholdId { get; set; } // FK to Household (optional - either user or household)
    public string PermissionLevel { get; set; } = string.Empty; // View, Edit, Delete
    
    // Navigation properties
    public Media? Media { get; set; }
    public ApplicationUser? User { get; set; }
    public Household? Household { get; set; }
}
