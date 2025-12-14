namespace RushtonRoots.Domain.UI.Requests;

public class SearchDocumentRequest
{
    public string? Title { get; set; }
    public string? Category { get; set; }
    public int? PersonId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? UploadedByUserId { get; set; }
}
