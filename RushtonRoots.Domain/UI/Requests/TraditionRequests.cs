namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request to create a new tradition
/// </summary>
public class CreateTraditionRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime? StartedDate { get; set; }
    public int? StartedByPersonId { get; set; }
    public string Status { get; set; } = "Active";
    public string? PhotoUrl { get; set; }
    public string? HowToCelebrate { get; set; }
    public string? AssociatedItems { get; set; }
    public bool IsPublished { get; set; }
}

/// <summary>
/// Request to update an existing tradition
/// </summary>
public class UpdateTraditionRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime? StartedDate { get; set; }
    public int? StartedByPersonId { get; set; }
    public string Status { get; set; } = "Active";
    public string? PhotoUrl { get; set; }
    public string? HowToCelebrate { get; set; }
    public string? AssociatedItems { get; set; }
    public bool IsPublished { get; set; }
}

/// <summary>
/// Request to search traditions
/// </summary>
public class SearchTraditionRequest
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public int? StartedByPersonId { get; set; }
    public bool? IsPublished { get; set; }
    public string SortBy { get; set; } = "Name"; // Name, StartedDate, CreatedDateTime, UpdatedDateTime
    public bool SortDescending { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Request to create a tradition timeline entry
/// </summary>
public class CreateTraditionTimelineRequest
{
    public int TraditionId { get; set; }
    public DateTime EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}

/// <summary>
/// Request to update a tradition timeline entry
/// </summary>
public class UpdateTraditionTimelineRequest
{
    public DateTime EventDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}
