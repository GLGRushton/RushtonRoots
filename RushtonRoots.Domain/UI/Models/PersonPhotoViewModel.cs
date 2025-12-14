namespace RushtonRoots.Domain.UI.Models;

public class PersonPhotoViewModel
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime? PhotoDate { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}
