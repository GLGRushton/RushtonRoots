namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a comment on various entities (profiles, photos, stories, etc.)
/// </summary>
public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // Person, Media, BiographicalNote, etc.
    public int EntityId { get; set; }
    public int? ParentCommentId { get; set; } // For comment replies
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }

    // Navigation properties
    public ApplicationUser? User { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
