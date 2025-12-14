using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class ChatRoomMapper : IChatRoomMapper
{
    private readonly IMessageMapper _messageMapper;

    public ChatRoomMapper(IMessageMapper messageMapper)
    {
        _messageMapper = messageMapper;
    }

    public ChatRoomViewModel MapToViewModel(ChatRoom chatRoom, string? currentUserId = null)
    {
        var viewModel = new ChatRoomViewModel
        {
            Id = chatRoom.Id,
            Name = chatRoom.Name,
            Description = chatRoom.Description,
            CreatedByUserId = chatRoom.CreatedByUserId,
            CreatedByName = chatRoom.CreatedBy?.UserName,
            HouseholdId = chatRoom.HouseholdId,
            HouseholdName = chatRoom.Household?.HouseholdName,
            IsActive = chatRoom.IsActive,
            CreatedDateTime = chatRoom.CreatedDateTime,
            UpdatedDateTime = chatRoom.UpdatedDateTime,
            Members = chatRoom.Members?.Select(MapToViewModel).ToList() ?? new List<ChatRoomMemberViewModel>(),
            LastMessage = chatRoom.Messages?.OrderByDescending(m => m.CreatedDateTime).FirstOrDefault() != null
                ? _messageMapper.MapToViewModel(chatRoom.Messages.OrderByDescending(m => m.CreatedDateTime).First())
                : null
        };

        // Calculate unread message count if currentUserId is provided
        if (!string.IsNullOrEmpty(currentUserId) && chatRoom.Members != null)
        {
            var currentMember = chatRoom.Members.FirstOrDefault(m => m.UserId == currentUserId);
            if (currentMember != null && chatRoom.Messages != null)
            {
                viewModel.UnreadMessageCount = chatRoom.Messages
                    .Count(m => currentMember.LastReadAt == null || m.CreatedDateTime > currentMember.LastReadAt);
            }
        }

        return viewModel;
    }

    public ChatRoomMemberViewModel MapToViewModel(ChatRoomMember member)
    {
        return new ChatRoomMemberViewModel
        {
            Id = member.Id,
            ChatRoomId = member.ChatRoomId,
            UserId = member.UserId,
            UserName = member.User?.UserName,
            Role = member.Role,
            JoinedAt = member.JoinedAt,
            LastReadAt = member.LastReadAt,
            IsActive = member.IsActive
        };
    }

    public ChatRoom MapToEntity(CreateChatRoomRequest request, string creatorUserId)
    {
        var chatRoom = new ChatRoom
        {
            Name = request.Name,
            Description = request.Description,
            CreatedByUserId = creatorUserId,
            HouseholdId = request.HouseholdId,
            IsActive = true
        };

        // Add creator as admin
        chatRoom.Members.Add(new ChatRoomMember
        {
            UserId = creatorUserId,
            Role = "Admin",
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        });

        // Add other members
        foreach (var userId in request.MemberUserIds.Where(id => id != creatorUserId))
        {
            chatRoom.Members.Add(new ChatRoomMember
            {
                UserId = userId,
                Role = "Member",
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            });
        }

        return chatRoom;
    }

    public void UpdateEntity(ChatRoom chatRoom, UpdateChatRoomRequest request)
    {
        chatRoom.Name = request.Name;
        chatRoom.Description = request.Description;
        chatRoom.IsActive = request.IsActive;
    }

    public ChatRoomMember MapToEntity(AddChatRoomMemberRequest request, int chatRoomId)
    {
        return new ChatRoomMember
        {
            ChatRoomId = chatRoomId,
            UserId = request.UserId,
            Role = request.Role,
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        };
    }
}
