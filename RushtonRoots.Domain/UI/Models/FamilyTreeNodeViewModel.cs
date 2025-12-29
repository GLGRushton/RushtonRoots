namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for family tree node in mini tree visualization.
/// Represents a person with their immediate family relationships.
/// </summary>
public class FamilyTreeNodeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public int Generation { get; set; } // 0 = focus person, -1 = parents, -2 = grandparents, 1 = children
    public List<FamilyTreeNodeViewModel>? Parents { get; set; }
    public List<FamilyTreeNodeViewModel>? Children { get; set; }
    public List<FamilyTreeNodeViewModel>? Spouses { get; set; }
}
