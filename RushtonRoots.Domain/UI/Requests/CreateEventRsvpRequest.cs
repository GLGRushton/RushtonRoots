namespace RushtonRoots.Domain.UI.Requests;

public class CreateEventRsvpRequest
{
    public int FamilyEventId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? GuestCount { get; set; }
    public string? Notes { get; set; }
}
