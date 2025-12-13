namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a photo associated with a person.
/// Each person can have multiple photos to track their appearance over time.
/// </summary>
public class PersonPhoto : BaseEntity
{
    public int PersonId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime? PhotoDate { get; set; }
    public bool IsPrimary { get; set; } // Primary photo shown in profile
    public int DisplayOrder { get; set; } // Order for displaying in gallery
    
    // Navigation properties
    public Person? Person { get; set; }
}
