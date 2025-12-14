namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for a tradition
/// </summary>
public class TraditionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime? StartedDate { get; set; }
    public int? StartedByPersonId { get; set; }
    public string? StartedByPersonName { get; set; }
    public string Status { get; set; } = "Active";
    public string? PhotoUrl { get; set; }
    public string? HowToCelebrate { get; set; }
    public string? AssociatedItems { get; set; }
    public string SubmittedByUserId { get; set; } = string.Empty;
    public string? SubmittedByUserName { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<TraditionTimelineViewModel> Timeline { get; set; } = new List<TraditionTimelineViewModel>();
}

/// <summary>
/// View model for a tradition timeline entry
/// </summary>
public class TraditionTimelineViewModel
{
    public int Id { get; set; }
    public int TraditionId { get; set; }
    public DateTime EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string RecordedByUserId { get; set; } = string.Empty;
    public string? RecordedByUserName { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}

/// <summary>
/// Search result for traditions
/// </summary>
public class TraditionSearchResult
{
    public List<TraditionViewModel> Traditions { get; set; } = new List<TraditionViewModel>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
