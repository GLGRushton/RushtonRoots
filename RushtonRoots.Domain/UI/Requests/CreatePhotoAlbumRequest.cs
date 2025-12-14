namespace RushtonRoots.Domain.UI.Requests;

public class CreatePhotoAlbumRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? AlbumDate { get; set; }
    public bool IsPublic { get; set; }
}
