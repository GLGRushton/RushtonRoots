namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a family calendar event (reunion, gathering, etc.)
/// </summary>
public class FamilyEvent : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string? Location { get; set; }
    public bool IsAllDay { get; set; }
    public string EventType { get; set; } = string.Empty; // Reunion, Birthday, Anniversary, Meeting, etc.
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly, Yearly
    public string CreatedByUserId { get; set; } = string.Empty;
    public int? HouseholdId { get; set; }
    public bool IsCancelled { get; set; }

    // Navigation properties
    public ApplicationUser? CreatedByUser { get; set; }
    public Household? Household { get; set; }
    public ICollection<EventRsvp> Rsvps { get; set; } = new List<EventRsvp>();
}
