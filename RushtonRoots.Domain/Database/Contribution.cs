namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a suggested edit or contribution to family data
/// </summary>
public class Contribution : BaseEntity
{
    public string EntityType { get; set; } = string.Empty; // Person, Partnership, LifeEvent, etc.
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty; // Which field is being changed
    public string? OldValue { get; set; } // Previous value (null for new entities)
    public string NewValue { get; set; } = string.Empty; // Proposed new value
    public string Reason { get; set; } = string.Empty; // Why this change is being suggested
    public string ContributorUserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Conflicted
    public string? ReviewerUserId { get; set; } // Who reviewed the contribution
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public int? CitationId { get; set; } // Optional citation supporting this change
    public bool RequiresCitation { get; set; } // Whether this contribution requires a citation

    // Navigation properties
    public ApplicationUser? Contributor { get; set; }
    public ApplicationUser? Reviewer { get; set; }
    public Citation? Citation { get; set; }
    public ICollection<ContributionApproval> Approvals { get; set; } = new List<ContributionApproval>();
    public ICollection<ConflictResolution> Conflicts { get; set; } = new List<ConflictResolution>();
}
