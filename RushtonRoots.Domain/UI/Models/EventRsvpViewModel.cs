namespace RushtonRoots.Domain.UI.Models;

public class EventRsvpViewModel
{
    public int Id { get; set; }
    public int FamilyEventId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? GuestCount { get; set; }
    public string? Notes { get; set; }
    public DateTime? ResponseDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
