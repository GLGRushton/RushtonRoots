namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a version of a document for version control.
/// Each time a document is updated, a new version is created.
/// </summary>
public class DocumentVersion : BaseEntity
{
    public int DocumentId { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string? BlobName { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public int VersionNumber { get; set; }
    public string? ChangeNotes { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    
    // Navigation properties
    public Document? Document { get; set; }
    public ApplicationUser? UploadedBy { get; set; }
}
