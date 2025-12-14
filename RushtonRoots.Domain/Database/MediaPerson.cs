namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents an association between a media file (video/audio) and a person.
/// Allows tracking which people appear in or are related to a media file.
/// </summary>
public class MediaPerson : BaseEntity
{
    public int MediaId { get; set; }
    public int PersonId { get; set; }
    public string? Notes { get; set; } // Optional notes about the association
    public int? AppearanceTimeSeconds { get; set; } // When the person appears in the media (optional)
    
    // Navigation properties
    public Media? Media { get; set; }
    public Person? Person { get; set; }
}
