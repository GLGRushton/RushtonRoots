using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class ChatRoomService : IChatRoomService
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IChatRoomMapper _mapper;

    public ChatRoomService(
        IChatRoomRepository chatRoomRepository,
        IChatRoomMapper mapper)
    {
        _chatRoomRepository = chatRoomRepository;
        _mapper = mapper;
    }

    public async Task<ChatRoomViewModel?> GetByIdAsync(int id)
    {
        var chatRoom = await _chatRoomRepository.GetByIdAsync(id);
        return chatRoom == null ? null : _mapper.MapToViewModel(chatRoom);
    }

    public async Task<List<ChatRoomViewModel>> GetUserChatRoomsAsync(string userId)
    {
        var chatRooms = await _chatRoomRepository.GetByUserIdAsync(userId);
        return chatRooms.Select(cr => _mapper.MapToViewModel(cr, userId)).ToList();
    }

    public async Task<List<ChatRoomViewModel>> GetHouseholdChatRoomsAsync(int householdId)
    {
        var chatRooms = await _chatRoomRepository.GetByHouseholdIdAsync(householdId);
        return chatRooms.Select(cr => _mapper.MapToViewModel(cr)).ToList();
    }

    public async Task<ChatRoomViewModel> CreateChatRoomAsync(CreateChatRoomRequest request, string creatorUserId)
    {
        var chatRoom = _mapper.MapToEntity(request, creatorUserId);
        var savedChatRoom = await _chatRoomRepository.AddAsync(chatRoom);
        
        return _mapper.MapToViewModel(savedChatRoom);
    }

    public async Task<ChatRoomViewModel> UpdateChatRoomAsync(int id, UpdateChatRoomRequest request, string userId)
    {
        var chatRoom = await _chatRoomRepository.GetByIdAsync(id);
        if (chatRoom == null)
            throw new InvalidOperationException("Chat room not found");
        
        // Check if user is admin of the chat room
        var member = await _chatRoomRepository.GetMemberAsync(id, userId);
        if (member == null || member.Role != "Admin")
            throw new UnauthorizedAccessException("Only admins can update chat room settings");
        
        _mapper.UpdateEntity(chatRoom, request);
        var updatedChatRoom = await _chatRoomRepository.UpdateAsync(chatRoom);
        
        return _mapper.MapToViewModel(updatedChatRoom);
    }

    public async Task DeleteChatRoomAsync(int id, string userId)
    {
        var chatRoom = await _chatRoomRepository.GetByIdAsync(id);
        if (chatRoom == null)
            throw new InvalidOperationException("Chat room not found");
        
        if (chatRoom.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can delete the chat room");
        
        await _chatRoomRepository.DeleteAsync(id);
    }

    public async Task<ChatRoomMemberViewModel> AddMemberAsync(int chatRoomId, AddChatRoomMemberRequest request, string adminUserId)
    {
        var chatRoom = await _chatRoomRepository.GetByIdAsync(chatRoomId);
        if (chatRoom == null)
            throw new InvalidOperationException("Chat room not found");
        
        // Check if user is admin
        var adminMember = await _chatRoomRepository.GetMemberAsync(chatRoomId, adminUserId);
        if (adminMember == null || adminMember.Role != "Admin")
            throw new UnauthorizedAccessException("Only admins can add members");
        
        // Check if user is already a member
        var existingMember = await _chatRoomRepository.GetMemberAsync(chatRoomId, request.UserId);
        if (existingMember != null)
            throw new InvalidOperationException("User is already a member of this chat room");
        
        var member = _mapper.MapToEntity(request, chatRoomId);
        var savedMember = await _chatRoomRepository.AddMemberAsync(member);
        
        return _mapper.MapToViewModel(savedMember);
    }

    public async Task RemoveMemberAsync(int chatRoomId, string userId, string adminUserId)
    {
        var chatRoom = await _chatRoomRepository.GetByIdAsync(chatRoomId);
        if (chatRoom == null)
            throw new InvalidOperationException("Chat room not found");
        
        // Check if admin user is actually an admin (or if user is removing themselves)
        if (userId != adminUserId)
        {
            var adminMember = await _chatRoomRepository.GetMemberAsync(chatRoomId, adminUserId);
            if (adminMember == null || adminMember.Role != "Admin")
                throw new UnauthorizedAccessException("Only admins can remove members");
        }
        
        await _chatRoomRepository.RemoveMemberAsync(chatRoomId, userId);
    }

    public async Task UpdateLastReadAsync(int chatRoomId, string userId)
    {
        var member = await _chatRoomRepository.GetMemberAsync(chatRoomId, userId);
        if (member == null)
            throw new InvalidOperationException("User is not a member of this chat room");
        
        member.LastReadAt = DateTime.UtcNow;
        await _chatRoomRepository.UpdateMemberAsync(member);
    }
}
