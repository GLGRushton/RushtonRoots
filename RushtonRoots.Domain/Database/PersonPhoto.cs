namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a photo associated with a person.
/// Each person can have multiple photos to track their appearance over time.
/// </summary>
public class PersonPhoto : BaseEntity
{
    public int PersonId { get; set; }
    public int? PhotoAlbumId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public DateTime? PhotoDate { get; set; }
    public bool IsPrimary { get; set; } // Primary photo shown in profile
    public int DisplayOrder { get; set; } // Order for displaying in gallery
    public string? BlobName { get; set; } // Blob storage filename
    public long FileSize { get; set; } // File size in bytes
    public string? ContentType { get; set; } // MIME type
    
    // Navigation properties
    public Person? Person { get; set; }
    public PhotoAlbum? PhotoAlbum { get; set; }
    public ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
    public ICollection<PhotoPermission> PhotoPermissions { get; set; } = new List<PhotoPermission>();
}
