namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents an activity item in the family contribution feed
/// </summary>
public class ActivityFeedItem : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty; // ContributionSubmitted, ContributionApproved, PersonAdded, PhotoUploaded, etc.
    public string? EntityType { get; set; } // Type of entity affected
    public int? EntityId { get; set; } // ID of entity affected
    public string Description { get; set; } = string.Empty; // Human-readable description
    public string? ActionUrl { get; set; } // URL to view the activity
    public int Points { get; set; } // Points earned for this activity (for gamification)
    public bool IsPublic { get; set; } = true; // Whether this activity is visible to all family members

    // Navigation properties
    public ApplicationUser? User { get; set; }
}
