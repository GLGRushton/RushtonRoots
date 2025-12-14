namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a collection or "book" of related stories.
/// Collections can group stories by theme, time period, or any other criteria.
/// </summary>
public class StoryCollection : BaseEntity
{
    /// <summary>
    /// Name of the collection
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the collection
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Description of the collection
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional cover image URL
    /// </summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// User who created the collection
    /// </summary>
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to creator
    /// </summary>
    public ApplicationUser? CreatedByUser { get; set; }

    /// <summary>
    /// Whether the collection is published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Display order for stories in this collection
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property to stories in this collection
    /// </summary>
    public ICollection<Story> Stories { get; set; } = new List<Story>();
}
