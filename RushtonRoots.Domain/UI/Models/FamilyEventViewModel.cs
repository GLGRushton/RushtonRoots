namespace RushtonRoots.Domain.UI.Models;

public class FamilyEventViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string? Location { get; set; }
    public bool IsAllDay { get; set; }
    public string EventType { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? CreatedByUserName { get; set; }
    public int? HouseholdId { get; set; }
    public string? HouseholdName { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<EventRsvpViewModel> Rsvps { get; set; } = new();
    public int AttendingCount { get; set; }
    public int NotAttendingCount { get; set; }
    public int MaybeCount { get; set; }
}
