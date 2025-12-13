namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for displaying partnership information.
/// </summary>
public class PartnershipViewModel
{
    public int Id { get; set; }
    public int PersonAId { get; set; }
    public int PersonBId { get; set; }
    public string PersonAName { get; set; } = string.Empty;
    public string PersonBName { get; set; } = string.Empty;
    public string PartnershipType { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
