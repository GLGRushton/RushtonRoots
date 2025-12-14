namespace RushtonRoots.Domain.UI.Requests;

public class UploadPhotoRequest
{
    public int PersonId { get; set; }
    public int? PhotoAlbumId { get; set; }
    public string? Caption { get; set; }
    public DateTime? PhotoDate { get; set; }
    public bool IsPrimary { get; set; }
}
