namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for displaying wiki pages
/// </summary>
public class WikiPageViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? CreatedByUserName { get; set; }
    public string? LastUpdatedByUserId { get; set; }
    public string? LastUpdatedByUserName { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<WikiTagViewModel> Tags { get; set; } = new();
    public int VersionCount { get; set; }
}

/// <summary>
/// View model for wiki page version history
/// </summary>
public class WikiPageVersionViewModel
{
    public int Id { get; set; }
    public int WikiPageId { get; set; }
    public int VersionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string UpdatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserName { get; set; }
    public string? ChangeDescription { get; set; }
    public DateTime CreatedDateTime { get; set; }
}

/// <summary>
/// View model for wiki categories
/// </summary>
public class WikiCategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public int PageCount { get; set; }
    public List<WikiCategoryViewModel> ChildCategories { get; set; } = new();
}

/// <summary>
/// View model for wiki tags
/// </summary>
public class WikiTagViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UsageCount { get; set; }
}

/// <summary>
/// View model for wiki templates
/// </summary>
public class WikiTemplateViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
