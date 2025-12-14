namespace RushtonRoots.Domain.UI.Requests;

public class UpdateCommentRequest
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
}
