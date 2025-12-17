using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for adding a member to a household.
/// </summary>
public class AddHouseholdMemberRequest
{
    [Required]
    public int HouseholdId { get; set; }
    
    [Required]
    public int PersonId { get; set; }
}
