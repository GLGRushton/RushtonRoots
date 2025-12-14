namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a tag for wiki pages (many-to-many relationship)
/// </summary>
public class WikiTag : BaseEntity
{
    /// <summary>
    /// Name of the tag
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the tag
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Number of pages using this tag
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Navigation property to wiki pages with this tag (many-to-many)
    /// </summary>
    public ICollection<WikiPage> WikiPages { get; set; } = new List<WikiPage>();
}
