namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a family unit, anchored by a direct descendant (AnchorPerson).
/// </summary>
public class Household : BaseEntity
{
    public string HouseholdName { get; set; } = string.Empty;
    public int? AnchorPersonId { get; set; }
    
    // Soft delete and archive support
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedDateTime { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedDateTime { get; set; }
    
    // Navigation properties
    public Person? AnchorPerson { get; set; }
    public ICollection<Person> Members { get; set; } = new List<Person>();
    public ICollection<HouseholdPermission> Permissions { get; set; } = new List<HouseholdPermission>();
}
