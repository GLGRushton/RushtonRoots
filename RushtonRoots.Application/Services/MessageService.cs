using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageMapper _mapper;
    private readonly INotificationService _notificationService;

    public MessageService(
        IMessageRepository messageRepository,
        IMessageMapper mapper,
        INotificationService notificationService)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<MessageViewModel?> GetByIdAsync(int id)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        return message == null ? null : _mapper.MapToViewModel(message);
    }

    public async Task<List<MessageViewModel>> GetDirectMessagesAsync(string userId1, string userId2, int pageSize = 50, int pageNumber = 1)
    {
        var skip = (pageNumber - 1) * pageSize;
        var messages = await _messageRepository.GetDirectMessagesAsync(userId1, userId2, pageSize, skip);
        return messages.Select(m => _mapper.MapToViewModel(m)).ToList();
    }

    public async Task<List<MessageViewModel>> GetChatRoomMessagesAsync(int chatRoomId, int pageSize = 50, int pageNumber = 1)
    {
        var skip = (pageNumber - 1) * pageSize;
        var messages = await _messageRepository.GetChatRoomMessagesAsync(chatRoomId, pageSize, skip);
        return messages.Select(m => _mapper.MapToViewModel(m)).ToList();
    }

    public async Task<MessageViewModel> SendMessageAsync(CreateMessageRequest request, string senderUserId)
    {
        var message = _mapper.MapToEntity(request, senderUserId);
        var savedMessage = await _messageRepository.AddAsync(message);
        
        // Create notification for recipient(s)
        if (savedMessage.RecipientUserId != null)
        {
            // Direct message notification
            await _notificationService.CreateNotificationAsync(
                savedMessage.RecipientUserId,
                "Message",
                "New Message",
                $"You have a new message from {savedMessage.Sender?.UserName ?? "a user"}",
                $"/messages/{savedMessage.Id}",
                savedMessage.Id,
                "Message");
        }
        
        // Handle @mentions
        if (request.MentionedUserIds.Any())
        {
            foreach (var mentionedUserId in request.MentionedUserIds)
            {
                await _notificationService.CreateNotificationAsync(
                    mentionedUserId,
                    "Mention",
                    "You were mentioned",
                    $"{savedMessage.Sender?.UserName ?? "Someone"} mentioned you in a message",
                    $"/messages/{savedMessage.Id}",
                    savedMessage.Id,
                    "Message");
            }
        }
        
        return _mapper.MapToViewModel(savedMessage);
    }

    public async Task<MessageViewModel> UpdateMessageAsync(int id, UpdateMessageRequest request, string userId)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        if (message == null)
            throw new InvalidOperationException("Message not found");
        
        if (message.SenderUserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own messages");
        
        _mapper.UpdateEntity(message, request);
        var updatedMessage = await _messageRepository.UpdateAsync(message);
        
        return _mapper.MapToViewModel(updatedMessage);
    }

    public async Task DeleteMessageAsync(int id, string userId)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        if (message == null)
            throw new InvalidOperationException("Message not found");
        
        if (message.SenderUserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own messages");
        
        await _messageRepository.DeleteAsync(id);
    }

    public async Task MarkAsReadAsync(int id, string userId)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        if (message == null)
            throw new InvalidOperationException("Message not found");
        
        if (message.RecipientUserId != userId)
            throw new UnauthorizedAccessException("You can only mark your own messages as read");
        
        if (message.ReadAt == null)
        {
            message.ReadAt = DateTime.UtcNow;
            await _messageRepository.UpdateAsync(message);
        }
    }

    public async Task<int> GetUnreadDirectMessageCountAsync(string userId)
    {
        return await _messageRepository.GetUnreadDirectMessageCountAsync(userId);
    }
}
