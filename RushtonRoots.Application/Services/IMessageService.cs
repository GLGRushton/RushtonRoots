using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IMessageService
{
    Task<MessageViewModel?> GetByIdAsync(int id);
    Task<List<MessageViewModel>> GetDirectMessagesAsync(string userId1, string userId2, int pageSize = 50, int pageNumber = 1);
    Task<List<MessageViewModel>> GetChatRoomMessagesAsync(int chatRoomId, int pageSize = 50, int pageNumber = 1);
    Task<MessageViewModel> SendMessageAsync(CreateMessageRequest request, string senderUserId);
    Task<MessageViewModel> UpdateMessageAsync(int id, UpdateMessageRequest request, string userId);
    Task DeleteMessageAsync(int id, string userId);
    Task MarkAsReadAsync(int id, string userId);
    Task<int> GetUnreadDirectMessageCountAsync(string userId);
}
