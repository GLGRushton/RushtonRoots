namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request to create a new story
/// </summary>
public class CreateStoryRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime? StoryDate { get; set; }
    public string? Location { get; set; }
    public bool IsPublished { get; set; }
    public bool AllowCollaboration { get; set; }
    public int? CollectionId { get; set; }
    public List<int> PersonIds { get; set; } = new List<int>();
}

/// <summary>
/// Request to update an existing story
/// </summary>
public class UpdateStoryRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime? StoryDate { get; set; }
    public string? Location { get; set; }
    public bool IsPublished { get; set; }
    public bool AllowCollaboration { get; set; }
    public int? CollectionId { get; set; }
    public List<int> PersonIds { get; set; } = new List<int>();
}

/// <summary>
/// Request to search stories
/// </summary>
public class SearchStoryRequest
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public int? PersonId { get; set; }
    public int? CollectionId { get; set; }
    public bool? IsPublished { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string SortBy { get; set; } = "UpdatedDateTime"; // Title, CreatedDateTime, UpdatedDateTime, ViewCount, StoryDate
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Request to create a new story collection
/// </summary>
public class CreateStoryCollectionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Request to update a story collection
/// </summary>
public class UpdateStoryCollectionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
}
