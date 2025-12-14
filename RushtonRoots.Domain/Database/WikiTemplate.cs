namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a reusable template for wiki pages
/// </summary>
public class WikiTemplate : BaseEntity
{
    /// <summary>
    /// Name of the template
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this template is for
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Template content with placeholders
    /// </summary>
    public string TemplateContent { get; set; } = string.Empty;

    /// <summary>
    /// Type of template (Person, Place, Event, General, etc.)
    /// </summary>
    public string TemplateType { get; set; } = string.Empty;

    /// <summary>
    /// Whether this template is available for use
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property to wiki pages using this template
    /// </summary>
    public ICollection<WikiPage> WikiPages { get; set; } = new List<WikiPage>();
}
