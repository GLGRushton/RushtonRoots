namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents permission settings for documents.
/// Controls who can view, edit, or delete documents.
/// </summary>
public class DocumentPermission : BaseEntity
{
    public int DocumentId { get; set; }
    public string? UserId { get; set; } // Specific user (optional)
    public int? HouseholdId { get; set; } // Or entire household (optional)
    public string PermissionLevel { get; set; } = string.Empty; // View, Edit, Delete
    
    // Navigation properties
    public Document? Document { get; set; }
    public ApplicationUser? User { get; set; }
    public Household? Household { get; set; }
}
