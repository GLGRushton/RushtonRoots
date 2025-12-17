using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for updating household settings.
/// </summary>
public class UpdateHouseholdSettingsRequest
{
    [Required]
    public int Id { get; set; }
    
    public bool IsArchived { get; set; }
}
