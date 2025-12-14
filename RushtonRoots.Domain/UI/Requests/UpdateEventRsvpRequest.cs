namespace RushtonRoots.Domain.UI.Requests;

public class UpdateEventRsvpRequest
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? GuestCount { get; set; }
    public string? Notes { get; set; }
}
