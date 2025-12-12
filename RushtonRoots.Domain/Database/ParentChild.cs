namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a parent-child relationship between two persons.
/// </summary>
public class ParentChild : BaseEntity
{
    public int ParentPersonId { get; set; }
    public int ChildPersonId { get; set; }
    public string RelationshipType { get; set; } = string.Empty; // e.g. 'Biological', 'Adopted', 'Step', 'Guardian'
    
    // Navigation properties
    public Person? ParentPerson { get; set; }
    public Person? ChildPerson { get; set; }
}
