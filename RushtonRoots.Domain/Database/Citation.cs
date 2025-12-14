namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a specific citation of a source for a particular fact or claim.
/// Links sources to specific data points with page numbers and quotes.
/// </summary>
public class Citation : BaseEntity
{
    public int SourceId { get; set; }
    public string? PageNumber { get; set; }
    public string? Quote { get; set; }
    public string? TranscriptionOrSummary { get; set; }
    public string? AccessedDate { get; set; } // For online sources
    public string? Url { get; set; } // For online sources
    
    // Navigation properties
    public Source? Source { get; set; }
}
