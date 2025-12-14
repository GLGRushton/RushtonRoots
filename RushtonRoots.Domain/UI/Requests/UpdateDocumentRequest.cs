namespace RushtonRoots.Domain.UI.Requests;

public class UpdateDocumentRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime? DocumentDate { get; set; }
    public bool IsPublic { get; set; }
    public List<int> AssociatedPeople { get; set; } = new List<int>();
}
