namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// Represents the relationship path between two people.
/// </summary>
public class RelationshipPathViewModel
{
    public int PersonAId { get; set; }
    public string PersonAName { get; set; } = string.Empty;
    public int PersonBId { get; set; }
    public string PersonBName { get; set; } = string.Empty;
    public string RelationshipDescription { get; set; } = string.Empty;
    public int Degree { get; set; } // Number of steps between people
    public List<RelationshipStepViewModel> Steps { get; set; } = new();
}

/// <summary>
/// Represents a single step in a relationship path.
/// </summary>
public class RelationshipStepViewModel
{
    public int FromPersonId { get; set; }
    public string FromPersonName { get; set; } = string.Empty;
    public int ToPersonId { get; set; }
    public string ToPersonName { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty; // "parent", "child", "spouse", etc.
}
