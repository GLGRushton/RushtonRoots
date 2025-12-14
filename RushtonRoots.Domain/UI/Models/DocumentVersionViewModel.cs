namespace RushtonRoots.Domain.UI.Models;

public class DocumentVersionViewModel
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public int VersionNumber { get; set; }
    public string? ChangeNotes { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    public string? UploadedByUserName { get; set; }
    public DateTime CreatedDateTime { get; set; }
}
