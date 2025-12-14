namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a document (PDF, Word, etc.) stored in the system.
/// Documents can be categorized and associated with people.
/// </summary>
public class Document : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty; // Birth Certificate, Will, Deed, etc.
    public string? BlobName { get; set; } // Blob storage filename
    public long FileSize { get; set; } // File size in bytes
    public string? ContentType { get; set; } // MIME type
    public DateTime? DocumentDate { get; set; } // Date relevant to the document
    public string UploadedByUserId { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation properties
    public ApplicationUser? UploadedBy { get; set; }
    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    public ICollection<DocumentPerson> DocumentPeople { get; set; } = new List<DocumentPerson>();
    public ICollection<DocumentPermission> DocumentPermissions { get; set; } = new List<DocumentPermission>();
}
