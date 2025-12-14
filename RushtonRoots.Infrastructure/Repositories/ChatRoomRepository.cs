using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class ChatRoomRepository : IChatRoomRepository
{
    private readonly RushtonRootsDbContext _context;

    public ChatRoomRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<ChatRoom?> GetByIdAsync(int id)
    {
        return await _context.ChatRooms
            .Include(cr => cr.CreatedBy)
            .Include(cr => cr.Household)
            .Include(cr => cr.Members)
                .ThenInclude(m => m.User)
            .Include(cr => cr.Messages.OrderByDescending(m => m.CreatedDateTime).Take(1))
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(cr => cr.Id == id);
    }

    public async Task<List<ChatRoom>> GetByUserIdAsync(string userId)
    {
        return await _context.ChatRooms
            .Where(cr => cr.Members.Any(m => m.UserId == userId && m.IsActive) && cr.IsActive)
            .Include(cr => cr.CreatedBy)
            .Include(cr => cr.Household)
            .Include(cr => cr.Members)
                .ThenInclude(m => m.User)
            .Include(cr => cr.Messages.OrderByDescending(m => m.CreatedDateTime).Take(1))
                .ThenInclude(m => m.Sender)
            .OrderByDescending(cr => cr.UpdatedDateTime)
            .ToListAsync();
    }

    public async Task<List<ChatRoom>> GetByHouseholdIdAsync(int householdId)
    {
        return await _context.ChatRooms
            .Where(cr => cr.HouseholdId == householdId && cr.IsActive)
            .Include(cr => cr.CreatedBy)
            .Include(cr => cr.Members)
                .ThenInclude(m => m.User)
            .OrderByDescending(cr => cr.UpdatedDateTime)
            .ToListAsync();
    }

    public async Task<ChatRoom> AddAsync(ChatRoom chatRoom)
    {
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();
        
        // Reload with includes
        return (await GetByIdAsync(chatRoom.Id))!;
    }

    public async Task<ChatRoom> UpdateAsync(ChatRoom chatRoom)
    {
        _context.ChatRooms.Update(chatRoom);
        await _context.SaveChangesAsync();
        
        // Reload with includes
        return (await GetByIdAsync(chatRoom.Id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var chatRoom = await _context.ChatRooms.FindAsync(id);
        if (chatRoom != null)
        {
            _context.ChatRooms.Remove(chatRoom);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ChatRoomMember?> GetMemberAsync(int chatRoomId, string userId)
    {
        return await _context.ChatRoomMembers
            .Include(m => m.User)
            .Include(m => m.ChatRoom)
            .FirstOrDefaultAsync(m => m.ChatRoomId == chatRoomId && m.UserId == userId);
    }

    public async Task<ChatRoomMember> AddMemberAsync(ChatRoomMember member)
    {
        _context.ChatRoomMembers.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<ChatRoomMember> UpdateMemberAsync(ChatRoomMember member)
    {
        _context.ChatRoomMembers.Update(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task RemoveMemberAsync(int chatRoomId, string userId)
    {
        var member = await GetMemberAsync(chatRoomId, userId);
        if (member != null)
        {
            _context.ChatRoomMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}
