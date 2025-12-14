namespace RushtonRoots.Domain.UI.Requests;

public class CreateBiographicalNoteRequest
{
    public int PersonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public int? SourceId { get; set; }
}
