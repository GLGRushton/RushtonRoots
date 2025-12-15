namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for displaying household information.
/// </summary>
public class HouseholdViewModel
{
    public int Id { get; set; }
    public string HouseholdName { get; set; } = string.Empty;
    public int? AnchorPersonId { get; set; }
    public string AnchorPersonName { get; set; } = string.Empty;
    public int MemberCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
