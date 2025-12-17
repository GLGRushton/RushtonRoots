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
    public string? PersonAPhotoUrl { get; set; }
    public string? PersonBPhotoUrl { get; set; }
    public DateTime? PersonABirthDate { get; set; }
    public DateTime? PersonADeathDate { get; set; }
    public bool PersonAIsDeceased { get; set; }
    public DateTime? PersonBBirthDate { get; set; }
    public DateTime? PersonBDeathDate { get; set; }
    public bool PersonBIsDeceased { get; set; }
    public string PartnershipType { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public PartnershipRelatedDataViewModel? RelatedData { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}

/// <summary>
/// View model for related data counts in a partnership.
/// </summary>
public class PartnershipRelatedDataViewModel
{
    public int Children { get; set; }
    public int SharedEvents { get; set; }
    public int Photos { get; set; }
    public int Stories { get; set; }
    public int Documents { get; set; }
}
