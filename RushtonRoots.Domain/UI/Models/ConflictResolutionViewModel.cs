namespace RushtonRoots.Domain.UI.Models;

public class ConflictResolutionViewModel
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public int? ContributionId { get; set; }
    public string ConflictType { get; set; } = string.Empty;
    public string? CurrentValue { get; set; }
    public string? ConflictingValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Resolution { get; set; }
    public string? ResolutionNotes { get; set; }
    public string? ResolvedByUserId { get; set; }
    public string? ResolvedByName { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int? AcceptedCitationId { get; set; }
    public DateTime CreatedDateTime { get; set; }
}
