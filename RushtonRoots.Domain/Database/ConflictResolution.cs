namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a conflict between different data sources or contributions
/// </summary>
public class ConflictResolution : BaseEntity
{
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public int? ContributionId { get; set; } // The contribution that caused the conflict
    public string ConflictType { get; set; } = string.Empty; // DataMismatch, DuplicateEntry, SourceConflict
    public string? CurrentValue { get; set; }
    public string? ConflictingValue { get; set; }
    public string Status { get; set; } = "Open"; // Open, UnderReview, Resolved, Dismissed
    public string? Resolution { get; set; } // How the conflict was resolved
    public string? ResolutionNotes { get; set; }
    public string? ResolvedByUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int? AcceptedCitationId { get; set; } // Which citation was chosen as more authoritative

    // Navigation properties
    public Contribution? Contribution { get; set; }
    public ApplicationUser? ResolvedBy { get; set; }
    public Citation? AcceptedCitation { get; set; }
}
