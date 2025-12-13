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
    public string RelationshipType { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
