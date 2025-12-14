namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a human in the family tree.
/// Each person must belong to exactly one household.
/// </summary>
public class Person : BaseEntity
{
    public int HouseholdId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public bool IsDeceased { get; set; }
    public string? PhotoUrl { get; set; }
    
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
