namespace RushtonRoots.Domain.UI.Models;

public class PhotoTagViewModel
{
    public int Id { get; set; }
    public int PersonPhotoId { get; set; }
    public int PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Notes { get; set; }
    public int? XPosition { get; set; }
    public int? YPosition { get; set; }
}
