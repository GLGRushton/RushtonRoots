namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a group chat room for family members
/// </summary>
public class ChatRoom : BaseEntity
{
    /// <summary>
    /// The name of the chat room
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the chat room
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The ID of the user who created the chat room
    /// </summary>
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// The user who created the chat room
    /// </summary>
    public ApplicationUser CreatedBy { get; set; } = null!;

    /// <summary>
    /// The ID of the household this chat room belongs to (optional)
    /// </summary>
    public int? HouseholdId { get; set; }

    /// <summary>
    /// The household this chat room belongs to (optional)
    /// </summary>
    public Household? Household { get; set; }

    /// <summary>
    /// Whether the chat room is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Members of the chat room
    /// </summary>
    public ICollection<ChatRoomMember> Members { get; set; } = new List<ChatRoomMember>();

    /// <summary>
    /// Messages in the chat room
    /// </summary>
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
