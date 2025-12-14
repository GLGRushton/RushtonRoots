namespace RushtonRoots.Domain.UI.Models;

public class LifeEventViewModel
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? EventDate { get; set; }
    public int? LocationId { get; set; }
    public string? LocationName { get; set; }
    public int? SourceId { get; set; }
    public string? SourceTitle { get; set; }
}
