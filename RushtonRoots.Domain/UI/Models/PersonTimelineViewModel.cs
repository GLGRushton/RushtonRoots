namespace RushtonRoots.Domain.UI.Models;

public class PersonTimelineViewModel
{
    public int PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public List<TimelineEventViewModel> Events { get; set; } = new();
}

public class TimelineEventViewModel
{
    public int? Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? EventDate { get; set; }
    public string? Location { get; set; }
    public string? Source { get; set; }
}
