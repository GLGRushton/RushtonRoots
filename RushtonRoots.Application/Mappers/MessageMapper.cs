using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class MessageMapper : IMessageMapper
{
    public MessageViewModel MapToViewModel(Message message)
    {
        return new MessageViewModel
        {
            Id = message.Id,
            Content = message.Content,
            SenderUserId = message.SenderUserId,
            SenderName = message.Sender?.UserName,
            RecipientUserId = message.RecipientUserId,
            RecipientName = message.Recipient?.UserName,
            ChatRoomId = message.ChatRoomId,
            ChatRoomName = message.ChatRoom?.Name,
            ParentMessageId = message.ParentMessageId,
            ReadAt = message.ReadAt,
            IsEdited = message.IsEdited,
            EditedAt = message.EditedAt,
            MentionedUserIds = string.IsNullOrEmpty(message.MentionedUserIds) 
                ? new List<string>() 
                : message.MentionedUserIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            CreatedDateTime = message.CreatedDateTime,
            UpdatedDateTime = message.UpdatedDateTime,
            Replies = message.Replies?.Select(MapToViewModel).ToList() ?? new List<MessageViewModel>()
        };
    }

    public Message MapToEntity(CreateMessageRequest request, string senderUserId)
    {
        return new Message
        {
            Content = request.Content,
            SenderUserId = senderUserId,
            RecipientUserId = request.RecipientUserId,
            ChatRoomId = request.ChatRoomId,
            ParentMessageId = request.ParentMessageId,
            MentionedUserIds = request.MentionedUserIds.Any() 
                ? string.Join(",", request.MentionedUserIds) 
                : null
        };
    }

    public void UpdateEntity(Message message, UpdateMessageRequest request)
    {
        message.Content = request.Content;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;
    }
}
