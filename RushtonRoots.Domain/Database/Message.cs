namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a message sent between family members
/// </summary>
public class Message : BaseEntity
{
    /// <summary>
    /// The content of the message
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the user who sent the message
    /// </summary>
    public string SenderUserId { get; set; } = string.Empty;

    /// <summary>
    /// The user who sent the message
    /// </summary>
    public ApplicationUser Sender { get; set; } = null!;

    /// <summary>
    /// The ID of the user who receives the message (for direct messages)
    /// Null if this is a group message
    /// </summary>
    public string? RecipientUserId { get; set; }

    /// <summary>
    /// The user who receives the message (for direct messages)
    /// </summary>
    public ApplicationUser? Recipient { get; set; }

    /// <summary>
    /// The ID of the chat room (for group messages)
    /// Null if this is a direct message
    /// </summary>
    public int? ChatRoomId { get; set; }

    /// <summary>
    /// The chat room this message belongs to (for group messages)
    /// </summary>
    public ChatRoom? ChatRoom { get; set; }

    /// <summary>
    /// The ID of the parent message (for replies/threads)
    /// </summary>
    public int? ParentMessageId { get; set; }

    /// <summary>
    /// The parent message (for replies/threads)
    /// </summary>
    public Message? ParentMessage { get; set; }

    /// <summary>
    /// Child messages (replies to this message)
    /// </summary>
    public ICollection<Message> Replies { get; set; } = new List<Message>();

    /// <summary>
    /// When the message was read by the recipient
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Whether the message has been edited
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    /// When the message was last edited
    /// </summary>
    public DateTime? EditedAt { get; set; }

    /// <summary>
    /// User mentions in the message (e.g., @username)
    /// Stored as comma-separated user IDs
    /// </summary>
    public string? MentionedUserIds { get; set; }
}
