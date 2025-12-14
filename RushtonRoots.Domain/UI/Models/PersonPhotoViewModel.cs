namespace RushtonRoots.Domain.UI.Models;

public class PersonPhotoViewModel
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int? PhotoAlbumId { get; set; }
    public string? PhotoAlbumName { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public DateTime? PhotoDate { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public List<PhotoTagViewModel> Tags { get; set; } = new List<PhotoTagViewModel>();
}
