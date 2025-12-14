namespace RushtonRoots.Domain.UI.Models;

public class ActivityFeedItemViewModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public int Points { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedDateTime { get; set; }
}
