namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for creating a new wiki page
/// </summary>
public class CreateWikiPageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int? CategoryId { get; set; }
    public int? TemplateId { get; set; }
    public bool IsPublished { get; set; }
    public List<int> TagIds { get; set; } = new();
    public string? ChangeDescription { get; set; }
}

/// <summary>
/// Request model for updating a wiki page
/// </summary>
public class UpdateWikiPageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int? CategoryId { get; set; }
    public bool IsPublished { get; set; }
    public List<int> TagIds { get; set; } = new();
    public string? ChangeDescription { get; set; }
}

/// <summary>
/// Request model for searching wiki pages
/// </summary>
public class WikiSearchRequest
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public List<int>? TagIds { get; set; }
    public bool? IsPublished { get; set; }
    public string? SortBy { get; set; } // Title, CreatedDate, UpdatedDate, ViewCount
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Request model for creating a wiki category
/// </summary>
public class CreateWikiCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Request model for updating a wiki category
/// </summary>
public class UpdateWikiCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Request model for creating a wiki tag
/// </summary>
public class CreateWikiTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// Request model for creating a wiki template
/// </summary>
public class CreateWikiTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Request model for updating a wiki template
/// </summary>
public class UpdateWikiTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
