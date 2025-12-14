namespace RushtonRoots.Domain.UI.Requests;

public class UpdateMediaRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? MediaDate { get; set; }
    public string? Transcription { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public List<int> AssociatedPeople { get; set; } = new();
}
