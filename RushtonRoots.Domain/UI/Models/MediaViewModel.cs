using RushtonRoots.Domain.Database;

namespace RushtonRoots.Domain.UI.Models;

public class MediaViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaType MediaType { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTime? MediaDate { get; set; }
    public string? Transcription { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    
    public List<MediaTimelineMarkerViewModel> TimelineMarkers { get; set; } = new();
    public List<int> AssociatedPeopleIds { get; set; } = new();
}
