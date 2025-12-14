namespace RushtonRoots.Domain.UI.Models;

public class PhotoAlbumViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? CreatedByUserName { get; set; }
    public DateTime? AlbumDate { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public int PhotoCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
}
