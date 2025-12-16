namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for displaying parent-child relationship information.
/// </summary>
public class ParentChildViewModel
{
    public int Id { get; set; }
    public int ParentPersonId { get; set; }
    public int ChildPersonId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public string ChildName { get; set; } = string.Empty;
    public string? ParentPhotoUrl { get; set; }
    public string? ChildPhotoUrl { get; set; }
    public DateTime? ChildBirthDate { get; set; }
    public int? ChildAge { get; set; }
    public string RelationshipType { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
