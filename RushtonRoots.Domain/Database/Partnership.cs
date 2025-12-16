namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a partnership or marriage between two persons.
/// </summary>
public class Partnership : BaseEntity
{
    public int PersonAId { get; set; }
    public int PersonBId { get; set; }
    public string PartnershipType { get; set; } = string.Empty; // e.g. 'Married', 'Partnered'
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedDateTime { get; set; }
    
    // Navigation properties
    public Person? PersonA { get; set; }
    public Person? PersonB { get; set; }
}
