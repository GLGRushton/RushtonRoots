namespace RushtonRoots.Domain.UI.Requests;

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int? ParentCommentId { get; set; }
}
