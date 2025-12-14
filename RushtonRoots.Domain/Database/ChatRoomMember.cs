namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a member of a chat room
/// </summary>
public class ChatRoomMember : BaseEntity
{
    /// <summary>
    /// The ID of the chat room
    /// </summary>
    public int ChatRoomId { get; set; }

    /// <summary>
    /// The chat room
    /// </summary>
    public ChatRoom ChatRoom { get; set; } = null!;

    /// <summary>
    /// The ID of the user who is a member
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The user who is a member
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// The role of the member in the chat room (e.g., Admin, Member)
    /// </summary>
    public string Role { get; set; } = "Member";

    /// <summary>
    /// When the member joined the chat room
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the member last read messages in the chat room
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    /// <summary>
    /// Whether the member is still active in the chat room
    /// </summary>
    public bool IsActive { get; set; } = true;
}
