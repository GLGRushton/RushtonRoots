using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IChatRoomMapper
{
    ChatRoomViewModel MapToViewModel(ChatRoom chatRoom, string? currentUserId = null);
    ChatRoomMemberViewModel MapToViewModel(ChatRoomMember member);
    ChatRoom MapToEntity(CreateChatRoomRequest request, string creatorUserId);
    void UpdateEntity(ChatRoom chatRoom, UpdateChatRoomRequest request);
    ChatRoomMember MapToEntity(AddChatRoomMemberRequest request, int chatRoomId);
}
