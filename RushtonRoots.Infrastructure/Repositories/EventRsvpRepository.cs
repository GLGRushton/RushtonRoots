using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class EventRsvpRepository : IEventRsvpRepository
{
    private readonly RushtonRootsDbContext _context;

    public EventRsvpRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<EventRsvp?> GetByIdAsync(int id)
    {
        return await _context.EventRsvps
            .Include(r => r.User)
            .Include(r => r.FamilyEvent)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<EventRsvp?> GetByEventAndUserAsync(int eventId, string userId)
    {
        return await _context.EventRsvps
            .Include(r => r.User)
            .Include(r => r.FamilyEvent)
            .FirstOrDefaultAsync(r => r.FamilyEventId == eventId && r.UserId == userId);
    }

    public async Task<IEnumerable<EventRsvp>> GetByEventIdAsync(int eventId)
    {
        return await _context.EventRsvps
            .Include(r => r.User)
            .Where(r => r.FamilyEventId == eventId)
            .OrderBy(r => r.ResponseDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventRsvp>> GetByUserIdAsync(string userId)
    {
        return await _context.EventRsvps
            .Include(r => r.FamilyEvent)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.FamilyEvent!.StartDateTime)
            .ToListAsync();
    }

    public async Task<EventRsvp> AddAsync(EventRsvp eventRsvp)
    {
        _context.EventRsvps.Add(eventRsvp);
        await _context.SaveChangesAsync();
        return eventRsvp;
    }

    public async Task<EventRsvp> UpdateAsync(EventRsvp eventRsvp)
    {
        _context.EventRsvps.Update(eventRsvp);
        await _context.SaveChangesAsync();
        return eventRsvp;
    }

    public async Task DeleteAsync(int id)
    {
        var eventRsvp = await _context.EventRsvps.FindAsync(id);
        if (eventRsvp != null)
        {
            _context.EventRsvps.Remove(eventRsvp);
            await _context.SaveChangesAsync();
        }
    }
}
