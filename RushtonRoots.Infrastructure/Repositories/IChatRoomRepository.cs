using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IChatRoomRepository
{
    Task<ChatRoom?> GetByIdAsync(int id);
    Task<List<ChatRoom>> GetByUserIdAsync(string userId);
    Task<List<ChatRoom>> GetByHouseholdIdAsync(int householdId);
    Task<ChatRoom> AddAsync(ChatRoom chatRoom);
    Task<ChatRoom> UpdateAsync(ChatRoom chatRoom);
    Task DeleteAsync(int id);
    Task<ChatRoomMember?> GetMemberAsync(int chatRoomId, string userId);
    Task<ChatRoomMember> AddMemberAsync(ChatRoomMember member);
    Task<ChatRoomMember> UpdateMemberAsync(ChatRoomMember member);
    Task RemoveMemberAsync(int chatRoomId, string userId);
}
