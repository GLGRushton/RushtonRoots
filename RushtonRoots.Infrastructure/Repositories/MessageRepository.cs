using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly RushtonRootsDbContext _context;

    public MessageRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(int id)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.ChatRoom)
            .Include(m => m.Replies)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Message>> GetDirectMessagesAsync(string userId1, string userId2, int pageSize, int skip)
    {
        return await _context.Messages
            .Where(m => m.ChatRoomId == null && 
                       ((m.SenderUserId == userId1 && m.RecipientUserId == userId2) ||
                        (m.SenderUserId == userId2 && m.RecipientUserId == userId1)))
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .OrderByDescending(m => m.CreatedDateTime)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Message>> GetChatRoomMessagesAsync(int chatRoomId, int pageSize, int skip)
    {
        return await _context.Messages
            .Where(m => m.ChatRoomId == chatRoomId)
            .Include(m => m.Sender)
            .Include(m => m.Replies)
            .OrderByDescending(m => m.CreatedDateTime)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Message> AddAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        
        // Reload with includes
        return (await GetByIdAsync(message.Id))!;
    }

    public async Task<Message> UpdateAsync(Message message)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
        
        // Reload with includes
        return (await GetByIdAsync(message.Id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetUnreadDirectMessageCountAsync(string userId)
    {
        return await _context.Messages
            .Where(m => m.RecipientUserId == userId && m.ReadAt == null && m.ChatRoomId == null)
            .CountAsync();
    }
}
