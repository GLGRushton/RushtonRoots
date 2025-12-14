namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a video or audio file stored in the system.
/// Extends the document concept with media-specific features like timeline markers and transcription.
/// </summary>
public class Media : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaType MediaType { get; set; } // Video or Audio
    public string? BlobName { get; set; } // Blob storage filename
    public long FileSize { get; set; } // File size in bytes
    public string? ContentType { get; set; } // MIME type (video/mp4, audio/mp3, etc.)
    public int? DurationSeconds { get; set; } // Duration in seconds
    public DateTime? MediaDate { get; set; } // Date when the media was recorded
    public string? Transcription { get; set; } // Full transcription of audio/video
    public string UploadedByUserId { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation properties
    public ApplicationUser? UploadedBy { get; set; }
    public ICollection<MediaTimelineMarker> TimelineMarkers { get; set; } = new List<MediaTimelineMarker>();
    public ICollection<MediaPerson> MediaPeople { get; set; } = new List<MediaPerson>();
    public ICollection<MediaPermission> MediaPermissions { get; set; } = new List<MediaPermission>();
}

/// <summary>
/// Enum to distinguish between video and audio media types.
/// </summary>
public enum MediaType
{
    Video = 0,
    Audio = 1
}
