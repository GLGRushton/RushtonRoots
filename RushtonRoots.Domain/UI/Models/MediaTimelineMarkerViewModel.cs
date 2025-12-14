namespace RushtonRoots.Domain.UI.Models;

public class MediaTimelineMarkerViewModel
{
    public int Id { get; set; }
    public int MediaId { get; set; }
    public int TimeSeconds { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
