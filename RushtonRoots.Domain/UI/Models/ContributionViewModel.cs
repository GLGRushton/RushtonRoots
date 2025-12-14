namespace RushtonRoots.Domain.UI.Models;

public class ContributionViewModel
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string NewValue { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ContributorUserId { get; set; } = string.Empty;
    public string? ContributorName { get; set; }
    public string? ReviewerUserId { get; set; }
    public string? ReviewerName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public int? CitationId { get; set; }
    public bool RequiresCitation { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
