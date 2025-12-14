namespace RushtonRoots.Domain.UI.Models;

public class ChatRoomMemberViewModel
{
    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string Role { get; set; } = "Member";
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }
    public bool IsActive { get; set; }
}
