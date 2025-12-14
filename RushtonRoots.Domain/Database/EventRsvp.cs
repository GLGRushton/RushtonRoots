namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents an RSVP response to a family event
/// </summary>
public class EventRsvp : BaseEntity
{
    public int FamilyEventId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Attending, NotAttending, Maybe, Pending
    public int? GuestCount { get; set; } // Number of additional guests
    public string? Notes { get; set; }
    public DateTime? ResponseDateTime { get; set; }

    // Navigation properties
    public FamilyEvent? FamilyEvent { get; set; }
    public ApplicationUser? User { get; set; }
}
