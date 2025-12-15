using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for creating a new household.
/// </summary>
public class CreateHouseholdRequest
{
    [Required]
    [StringLength(200)]
    public string HouseholdName { get; set; } = string.Empty;
    
    public int? AnchorPersonId { get; set; }
}
