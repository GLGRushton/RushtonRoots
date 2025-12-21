using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for updating notes on a parent-child relationship.
/// </summary>
public class UpdateParentChildNotesRequest
{
    [Required(ErrorMessage = "Notes are required")]
    [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
    public string Notes { get; set; } = string.Empty;
}
