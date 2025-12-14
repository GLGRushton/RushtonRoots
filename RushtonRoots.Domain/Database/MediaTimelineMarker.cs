namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a timeline marker in a video or audio file.
/// Markers can be used to highlight important moments, chapters, or specific events.
/// </summary>
public class MediaTimelineMarker : BaseEntity
{
    public int MediaId { get; set; }
    public int TimeSeconds { get; set; } // Position in the media file (in seconds)
    public string Label { get; set; } = string.Empty; // Short label for the marker
    public string? Description { get; set; } // Detailed description
    public string? ThumbnailUrl { get; set; } // Optional thumbnail for video markers
    
    // Navigation properties
    public Media? Media { get; set; }
}
