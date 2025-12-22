namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for a story comment
/// </summary>
public class StoryComment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public int? ParentCommentId { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<StoryComment> Replies { get; set; } = new List<StoryComment>();
}
