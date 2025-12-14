namespace RushtonRoots.Domain.UI.Models;

public class MessageViewModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string SenderUserId { get; set; } = string.Empty;
    public string? SenderName { get; set; }
    public string? RecipientUserId { get; set; }
    public string? RecipientName { get; set; }
    public int? ChatRoomId { get; set; }
    public string? ChatRoomName { get; set; }
    public int? ParentMessageId { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public List<string> MentionedUserIds { get; set; } = new();
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<MessageViewModel> Replies { get; set; } = new();
}
