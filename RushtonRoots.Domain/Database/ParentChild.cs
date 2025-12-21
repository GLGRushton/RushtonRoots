namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a parent-child relationship between two persons.
/// </summary>
public class ParentChild : BaseEntity
{
    public int ParentPersonId { get; set; }
    public int ChildPersonId { get; set; }
    public string RelationshipType { get; set; } = string.Empty; // e.g. 'Biological', 'Adopted', 'Step', 'Guardian'
    
    // Additional relationship metadata
    public string? Notes { get; set; }
    public int? ConfidenceScore { get; set; } // 0-100, for AI features
    
    // Soft delete support
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedDateTime { get; set; }
    
    // Disputed relationship support
    public bool IsDisputed { get; set; } = false;
    public DateTime? DisputedDateTime { get; set; }
    public string? DisputeReason { get; set; }
    
    // Navigation properties
    public Person? ParentPerson { get; set; }
    public Person? ChildPerson { get; set; }
}
