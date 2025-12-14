namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents an in-app notification for a user
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// The ID of the user who will receive the notification
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The user who will receive the notification
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// The type of notification (e.g., Message, Mention, Event, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The title of the notification
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The message/content of the notification
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The URL to navigate to when the notification is clicked
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// The ID of the related entity (e.g., message ID, person ID, etc.)
    /// </summary>
    public int? RelatedEntityId { get; set; }

    /// <summary>
    /// The type of the related entity (e.g., Message, Person, Event, etc.)
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Whether the notification has been read
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// When the notification was read
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Whether an email notification was sent
    /// </summary>
    public bool EmailSent { get; set; } = false;

    /// <summary>
    /// When the email notification was sent
    /// </summary>
    public DateTime? EmailSentAt { get; set; }
}
