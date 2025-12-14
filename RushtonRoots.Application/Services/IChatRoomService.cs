using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IChatRoomService
{
    Task<ChatRoomViewModel?> GetByIdAsync(int id);
    Task<List<ChatRoomViewModel>> GetUserChatRoomsAsync(string userId);
    Task<List<ChatRoomViewModel>> GetHouseholdChatRoomsAsync(int householdId);
    Task<ChatRoomViewModel> CreateChatRoomAsync(CreateChatRoomRequest request, string creatorUserId);
    Task<ChatRoomViewModel> UpdateChatRoomAsync(int id, UpdateChatRoomRequest request, string userId);
    Task DeleteChatRoomAsync(int id, string userId);
    Task<ChatRoomMemberViewModel> AddMemberAsync(int chatRoomId, AddChatRoomMemberRequest request, string adminUserId);
    Task RemoveMemberAsync(int chatRoomId, string userId, string adminUserId);
    Task UpdateLastReadAsync(int chatRoomId, string userId);
}
