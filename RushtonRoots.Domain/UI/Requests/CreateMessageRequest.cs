namespace RushtonRoots.Domain.UI.Requests;

public class CreateMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public string? RecipientUserId { get; set; }
    public int? ChatRoomId { get; set; }
    public int? ParentMessageId { get; set; }
    public List<string> MentionedUserIds { get; set; } = new();
}
