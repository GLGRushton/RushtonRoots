namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a human in the family tree.
/// Each person must belong to exactly one household.
/// </summary>
public class Person : BaseEntity
{
    public int HouseholdId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public string? PlaceOfDeath { get; set; }
    public bool IsDeceased { get; set; }
    public string? Biography { get; set; }
    public string? Occupation { get; set; }
    public string? Education { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    
    // Soft delete and archive support
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedDateTime { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedDateTime { get; set; }
    
    // Navigation properties
    public Household? Household { get; set; }
    public ICollection<ParentChild> ParentRelationships { get; set; } = new List<ParentChild>();
    public ICollection<ParentChild> ChildRelationships { get; set; } = new List<ParentChild>();
    public ICollection<Partnership> PartnershipsAsPersonA { get; set; } = new List<Partnership>();
    public ICollection<Partnership> PartnershipsAsPersonB { get; set; } = new List<Partnership>();
    public ICollection<HouseholdPermission> HouseholdPermissions { get; set; } = new List<HouseholdPermission>();
    public ICollection<LifeEvent> LifeEvents { get; set; } = new List<LifeEvent>();
    public ICollection<PersonPhoto> Photos { get; set; } = new List<PersonPhoto>();
    public ICollection<BiographicalNote> BiographicalNotes { get; set; } = new List<BiographicalNote>();
    public ICollection<StoryPerson> StoryPeople { get; set; } = new List<StoryPerson>();
}
