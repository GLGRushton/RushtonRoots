namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// Statistics displayed on the home page dashboard
/// </summary>
public class HomePageStatistics
{
    public int TotalMembers { get; set; }
    public string? OldestAncestor { get; set; }
    public string? NewestMember { get; set; }
    public int TotalPhotos { get; set; }
    public int TotalStories { get; set; }
    public int ActiveHouseholds { get; set; }
}
