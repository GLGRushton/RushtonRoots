namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a tag linking a person to a photo.
/// This allows tagging people who appear in photos.
/// </summary>
public class PhotoTag : BaseEntity
{
    public int PersonPhotoId { get; set; }
    public int PersonId { get; set; }
    public string? Notes { get; set; }
    public int? XPosition { get; set; } // X coordinate percentage (0-100) for face tagging
    public int? YPosition { get; set; } // Y coordinate percentage (0-100) for face tagging
    
    // Navigation properties
    public PersonPhoto? PersonPhoto { get; set; }
    public Person? Person { get; set; }
}
