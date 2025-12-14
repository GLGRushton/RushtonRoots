namespace RushtonRoots.Domain.UI.Requests;

public class UpdateChatRoomRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
