using RushtonRoots.Domain.Database;

namespace RushtonRoots.Domain.UI.Requests;

public class CreateMediaRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MediaType MediaType { get; set; }
    public DateTime? MediaDate { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public List<int> AssociatedPeople { get; set; } = new();
}
