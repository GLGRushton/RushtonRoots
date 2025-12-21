using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request to update a household member's role.
/// </summary>
public class UpdateMemberRoleRequest
{
    [Required(ErrorMessage = "Role is required")]
    [RegularExpression("^(ADMIN|EDITOR)$", ErrorMessage = "Role must be either 'ADMIN' or 'EDITOR'")]
    public string Role { get; set; } = string.Empty;
}
