namespace RushtonRoots.Domain.UI.Models;

public class ChatRoomViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? CreatedByName { get; set; }
    public int? HouseholdId { get; set; }
    public string? HouseholdName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<ChatRoomMemberViewModel> Members { get; set; } = new();
    public int UnreadMessageCount { get; set; }
    public MessageViewModel? LastMessage { get; set; }
}
