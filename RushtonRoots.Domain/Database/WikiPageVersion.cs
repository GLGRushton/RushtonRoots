namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a historical version of a wiki page
/// </summary>
public class WikiPageVersion : BaseEntity
{
    /// <summary>
    /// ID of the wiki page this version belongs to
    /// </summary>
    public int WikiPageId { get; set; }

    /// <summary>
    /// Navigation property to wiki page
    /// </summary>
    public WikiPage WikiPage { get; set; } = null!;

    /// <summary>
    /// Version number (sequential)
    /// </summary>
    public int VersionNumber { get; set; }

    /// <summary>
    /// Title at this version
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content at this version
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Summary at this version
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// User who created this version
    /// </summary>
    public string UpdatedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to user who updated
    /// </summary>
    public ApplicationUser UpdatedByUser { get; set; } = null!;

    /// <summary>
    /// Description of changes made in this version
    /// </summary>
    public string? ChangeDescription { get; set; }
}
