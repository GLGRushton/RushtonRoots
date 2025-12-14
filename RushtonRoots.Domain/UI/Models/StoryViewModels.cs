namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for a story
/// </summary>
public class StoryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime? StoryDate { get; set; }
    public string? Location { get; set; }
    public string SubmittedByUserId { get; set; } = string.Empty;
    public string? SubmittedByUserName { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public bool AllowCollaboration { get; set; }
    public int? CollectionId { get; set; }
    public string? CollectionName { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<PersonBasicViewModel> AssociatedPeople { get; set; } = new List<PersonBasicViewModel>();
}

/// <summary>
/// Basic person information for story associations
/// </summary>
public class PersonBasicViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RoleInStory { get; set; }
}

/// <summary>
/// View model for a story collection
/// </summary>
public class StoryCollectionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? CreatedByUserName { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public int StoryCount { get; set; }
}

/// <summary>
/// Search result for stories
/// </summary>
public class StorySearchResult
{
    public List<StoryViewModel> Stories { get; set; } = new List<StoryViewModel>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
