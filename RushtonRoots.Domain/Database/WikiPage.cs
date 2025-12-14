namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a wiki page in the family knowledge base
/// </summary>
public class WikiPage : BaseEntity
{
    /// <summary>
    /// Title of the wiki page
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the page
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Markdown content of the page
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional summary/excerpt
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// ID of the category this page belongs to
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Navigation property to category
    /// </summary>
    public WikiCategory? Category { get; set; }

    /// <summary>
    /// ID of the template used (if any)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Navigation property to template
    /// </summary>
    public WikiTemplate? Template { get; set; }

    /// <summary>
    /// User who created the page
    /// </summary>
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to creator
    /// </summary>
    public ApplicationUser? CreatedByUser { get; set; }

    /// <summary>
    /// User who last updated the page
    /// </summary>
    public string? LastUpdatedByUserId { get; set; }

    /// <summary>
    /// Navigation property to last updater
    /// </summary>
    public ApplicationUser? LastUpdatedByUser { get; set; }

    /// <summary>
    /// Whether the page is published (visible to family members)
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Number of times this page has been viewed
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Navigation property to version history
    /// </summary>
    public ICollection<WikiPageVersion> Versions { get; set; } = new List<WikiPageVersion>();

    /// <summary>
    /// Navigation property to tags (many-to-many)
    /// </summary>
    public ICollection<WikiTag> Tags { get; set; } = new List<WikiTag>();
}
