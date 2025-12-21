namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model representing the impact of deleting a household.
/// Shows counts of related data that will be affected.
/// </summary>
public class HouseholdDeleteImpact
{
    /// <summary>
    /// Number of members (people) in the household.
    /// </summary>
    public int MemberCount { get; set; }
    
    /// <summary>
    /// Number of photos associated with household members.
    /// </summary>
    public int PhotoCount { get; set; }
    
    /// <summary>
    /// Number of documents associated with household members.
    /// </summary>
    public int DocumentCount { get; set; }
    
    /// <summary>
    /// Number of relationships (partnerships and parent-child) involving household members.
    /// </summary>
    public int RelationshipCount { get; set; }
    
    /// <summary>
    /// Number of family events associated with the household.
    /// </summary>
    public int EventCount { get; set; }
}
