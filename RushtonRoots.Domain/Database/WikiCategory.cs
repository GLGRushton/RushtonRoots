namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a category for organizing wiki pages
/// </summary>
public class WikiCategory : BaseEntity
{
    /// <summary>
    /// Name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Description of the category
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Parent category ID for hierarchical organization
    /// </summary>
    public int? ParentCategoryId { get; set; }

    /// <summary>
    /// Navigation property to parent category
    /// </summary>
    public WikiCategory? ParentCategory { get; set; }

    /// <summary>
    /// Icon or emoji for the category
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property to child categories
    /// </summary>
    public ICollection<WikiCategory> ChildCategories { get; set; } = new List<WikiCategory>();

    /// <summary>
    /// Navigation property to wiki pages in this category
    /// </summary>
    public ICollection<WikiPage> WikiPages { get; set; } = new List<WikiPage>();
}
