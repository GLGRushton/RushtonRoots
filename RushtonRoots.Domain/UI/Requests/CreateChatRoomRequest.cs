namespace RushtonRoots.Domain.UI.Requests;

public class CreateChatRoomRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? HouseholdId { get; set; }
    public List<string> MemberUserIds { get; set; } = new();
}
