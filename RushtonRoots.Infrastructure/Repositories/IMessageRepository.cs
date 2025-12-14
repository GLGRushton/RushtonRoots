using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(int id);
    Task<List<Message>> GetDirectMessagesAsync(string userId1, string userId2, int pageSize, int skip);
    Task<List<Message>> GetChatRoomMessagesAsync(int chatRoomId, int pageSize, int skip);
    Task<Message> AddAsync(Message message);
    Task<Message> UpdateAsync(Message message);
    Task DeleteAsync(int id);
    Task<int> GetUnreadDirectMessageCountAsync(string userId);
}
