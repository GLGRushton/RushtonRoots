namespace RushtonRoots.Domain.UI.Requests;

public class ReviewContributionRequest
{
    public int ContributionId { get; set; }
    public string Decision { get; set; } = string.Empty; // Approved, Rejected, RequestMoreInfo
    public string? Notes { get; set; }
}
