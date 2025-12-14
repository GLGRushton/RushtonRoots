namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a user's notification preferences
/// </summary>
public class NotificationPreference : BaseEntity
{
    /// <summary>
    /// The ID of the user
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The user
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// The type of notification (e.g., Message, Mention, Event, etc.)
    /// </summary>
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Whether in-app notifications are enabled for this type
    /// </summary>
    public bool InAppEnabled { get; set; } = true;

    /// <summary>
    /// Whether email notifications are enabled for this type
    /// </summary>
    public bool EmailEnabled { get; set; } = true;

    /// <summary>
    /// The frequency of email notifications (Immediate, Daily, Weekly, Never)
    /// </summary>
    public string EmailFrequency { get; set; } = "Immediate";
}
