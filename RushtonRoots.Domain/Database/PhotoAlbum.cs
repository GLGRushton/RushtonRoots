namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a photo album for organizing photos into collections.
/// Albums can be created by users to group related photos together.
/// </summary>
public class PhotoAlbum : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime? AlbumDate { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation properties
    public ApplicationUser? CreatedBy { get; set; }
    public ICollection<PersonPhoto> Photos { get; set; } = new List<PersonPhoto>();
}
