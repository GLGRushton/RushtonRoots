namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents an approval or rejection action on a contribution
/// </summary>
public class ContributionApproval : BaseEntity
{
    public int ContributionId { get; set; }
    public string ApproverUserId { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty; // Approved, Rejected, RequestMoreInfo
    public string? Notes { get; set; }
    public DateTime DecisionDate { get; set; }
    public bool IsFinalDecision { get; set; } // Whether this approval finalizes the contribution

    // Navigation properties
    public Contribution? Contribution { get; set; }
    public ApplicationUser? Approver { get; set; }
}
