namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a specific point in time or event in the history of a family tradition.
/// Used to track how traditions have evolved or significant moments in their observance.
/// </summary>
public class TraditionTimeline : BaseEntity
{
    /// <summary>
    /// ID of the tradition this timeline entry belongs to
    /// </summary>
    public int TraditionId { get; set; }

    /// <summary>
    /// Navigation property to the tradition
    /// </summary>
    public Tradition? Tradition { get; set; }

    /// <summary>
    /// Date of this timeline event
    /// </summary>
    public DateTime EventDate { get; set; }

    /// <summary>
    /// Title of the timeline event
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of what happened at this point in the tradition's history
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of event (Started, Modified, Paused, Resumed, Special Observance)
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// User who recorded this timeline entry
    /// </summary>
    public string RecordedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to user who recorded the entry
    /// </summary>
    public ApplicationUser? RecordedByUser { get; set; }

    /// <summary>
    /// Optional photo URL for this timeline event
    /// </summary>
    public string? PhotoUrl { get; set; }
}
