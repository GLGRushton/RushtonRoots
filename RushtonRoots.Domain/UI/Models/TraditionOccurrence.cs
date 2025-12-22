namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model representing an occurrence of a tradition
/// Can be either a past occurrence or a future/next occurrence
/// </summary>
public class TraditionOccurrence
{
    public int Id { get; set; }
    public int TraditionId { get; set; }
    public DateTime EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string RecordedByUserId { get; set; } = string.Empty;
    public string? RecordedByUserName { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
