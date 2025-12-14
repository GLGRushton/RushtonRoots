namespace RushtonRoots.Domain.UI.Requests;

public class CreatePhotoTagRequest
{
    public int PersonPhotoId { get; set; }
    public int PersonId { get; set; }
    public string? Notes { get; set; }
    public int? XPosition { get; set; }
    public int? YPosition { get; set; }
}
