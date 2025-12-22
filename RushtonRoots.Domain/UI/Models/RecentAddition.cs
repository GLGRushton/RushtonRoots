namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// Represents a recently added person to the family tree
/// </summary>
public class RecentAddition
{
    public int PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime AddedDate { get; set; }
    public string? RelationshipDescription { get; set; }
}
