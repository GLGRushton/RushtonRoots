namespace RushtonRoots.Domain.UI.Models;

public class BiographicalNoteViewModel
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public int? SourceId { get; set; }
    public string? SourceTitle { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
