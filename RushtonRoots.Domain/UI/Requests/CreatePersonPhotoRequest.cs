namespace RushtonRoots.Domain.UI.Requests;

public class CreatePersonPhotoRequest
{
    public int PersonId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime? PhotoDate { get; set; }
    public bool IsPrimary { get; set; }
}
