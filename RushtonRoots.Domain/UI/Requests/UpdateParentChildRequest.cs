using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for updating an existing parent-child relationship.
/// </summary>
public class UpdateParentChildRequest
{
    [Required]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Parent is required")]
    public int ParentPersonId { get; set; }
    
    [Required(ErrorMessage = "Child is required")]
    public int ChildPersonId { get; set; }
    
    [Required(ErrorMessage = "Relationship type is required")]
    [MaxLength(50)]
    public string RelationshipType { get; set; } = string.Empty;
}
