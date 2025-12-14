namespace RushtonRoots.Domain.UI.Requests;

public class AddChatRoomMemberRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
}
