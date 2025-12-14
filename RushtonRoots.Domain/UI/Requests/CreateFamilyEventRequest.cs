namespace RushtonRoots.Domain.UI.Requests;

public class CreateFamilyEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string? Location { get; set; }
    public bool IsAllDay { get; set; }
    public string EventType { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public int? HouseholdId { get; set; }
}
