namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a family tradition, custom, or regularly observed practice.
/// Traditions preserve family culture and create continuity across generations.
/// </summary>
public class Tradition : BaseEntity
{
    /// <summary>
    /// Name of the tradition
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the tradition
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the tradition
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Category (e.g., Holiday, Birthday, Anniversary, Seasonal, Religious, Cultural)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// How often the tradition is observed (e.g., Yearly, Monthly, Weekly, Special Occasions)
    /// </summary>
    public string Frequency { get; set; } = string.Empty;

    /// <summary>
    /// Date when the tradition started (if known)
    /// </summary>
    public DateTime? StartedDate { get; set; }

    /// <summary>
    /// Person who started the tradition (if known)
    /// </summary>
    public int? StartedByPersonId { get; set; }

    /// <summary>
    /// Navigation property to person who started the tradition
    /// </summary>
    public Person? StartedByPerson { get; set; }

    /// <summary>
    /// Current status of the tradition (Active, Discontinued, Evolving)
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// URL to photo representing the tradition
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Additional notes about how the tradition is practiced
    /// </summary>
    public string? HowToCelebrate { get; set; }

    /// <summary>
    /// Special items, foods, or materials associated with the tradition
    /// </summary>
    public string? AssociatedItems { get; set; }

    /// <summary>
    /// User who submitted the tradition to the system
    /// </summary>
    public string SubmittedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to submitter
    /// </summary>
    public ApplicationUser? SubmittedByUser { get; set; }

    /// <summary>
    /// Whether the tradition is published (visible to family members)
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Number of times this tradition has been viewed
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Timeline entries showing the evolution of this tradition
    /// </summary>
    public ICollection<TraditionTimeline> Timeline { get; set; } = new List<TraditionTimeline>();
}
