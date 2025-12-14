namespace RushtonRoots.Domain.UI.Models;

public class DocumentViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime? DocumentDate { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    public string? UploadedByUserName { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public int VersionCount { get; set; }
    public int CurrentVersion { get; set; }
    public List<int> AssociatedPeople { get; set; } = new List<int>();
}
