namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for recent activity items on admin dashboard
/// </summary>
public class RecentActivity
{
    /// <summary>
    /// Activity item ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID who performed the activity
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Username of the user who performed the activity
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Type of activity (e.g., "PersonAdded", "StoryPublished")
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Entity type the activity relates to (e.g., "Person", "Story")
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the activity
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// URL to the activity action (if applicable)
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// When the activity occurred
    /// </summary>
    public DateTime CreatedDateTime { get; set; }
}
