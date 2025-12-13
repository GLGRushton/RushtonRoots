using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for updating an existing household.
/// </summary>
public class UpdateHouseholdRequest
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string HouseholdName { get; set; } = string.Empty;
    
    [Required]
    public int AnchorPersonId { get; set; }
}
