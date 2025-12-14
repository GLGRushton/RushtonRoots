namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents sharing permissions for photos.
/// Controls who can view specific photos or albums.
/// </summary>
public class PhotoPermission : BaseEntity
{
    public int? PersonPhotoId { get; set; }
    public int? PhotoAlbumId { get; set; }
    public string? UserId { get; set; }
    public int? HouseholdId { get; set; }
    public string PermissionLevel { get; set; } = "View"; // View, Edit, Delete
    
    // Navigation properties
    public PersonPhoto? PersonPhoto { get; set; }
    public PhotoAlbum? PhotoAlbum { get; set; }
    public ApplicationUser? User { get; set; }
    public Household? Household { get; set; }
}
