namespace RushtonRoots.Domain.UI.Requests;

public class CreateLifeEventRequest
{
    public int PersonId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? EventDate { get; set; }
    public int? LocationId { get; set; }
    public int? SourceId { get; set; }
}
