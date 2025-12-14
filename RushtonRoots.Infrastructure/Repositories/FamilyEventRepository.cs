using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class FamilyEventRepository : IFamilyEventRepository
{
    private readonly RushtonRootsDbContext _context;

    public FamilyEventRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<FamilyEvent?> GetByIdAsync(int id)
    {
        return await _context.FamilyEvents
            .Include(e => e.CreatedByUser)
            .Include(e => e.Household)
            .Include(e => e.Rsvps)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<FamilyEvent>> GetAllAsync()
    {
        return await _context.FamilyEvents
            .Include(e => e.CreatedByUser)
            .Include(e => e.Household)
            .Include(e => e.Rsvps)
            .OrderBy(e => e.StartDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FamilyEvent>> GetByHouseholdIdAsync(int householdId)
    {
        return await _context.FamilyEvents
            .Include(e => e.CreatedByUser)
            .Include(e => e.Household)
            .Include(e => e.Rsvps)
            .Where(e => e.HouseholdId == householdId)
            .OrderBy(e => e.StartDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FamilyEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.FamilyEvents
            .Include(e => e.CreatedByUser)
            .Include(e => e.Household)
            .Include(e => e.Rsvps)
            .Where(e => e.StartDateTime >= startDate && e.StartDateTime <= endDate)
            .OrderBy(e => e.StartDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FamilyEvent>> GetUpcomingEventsAsync(int count = 10)
    {
        var now = DateTime.UtcNow;
        return await _context.FamilyEvents
            .Include(e => e.CreatedByUser)
            .Include(e => e.Household)
            .Include(e => e.Rsvps)
            .Where(e => e.StartDateTime >= now && !e.IsCancelled)
            .OrderBy(e => e.StartDateTime)
            .Take(count)
            .ToListAsync();
    }

    public async Task<FamilyEvent> AddAsync(FamilyEvent familyEvent)
    {
        _context.FamilyEvents.Add(familyEvent);
        await _context.SaveChangesAsync();
        return familyEvent;
    }

    public async Task<FamilyEvent> UpdateAsync(FamilyEvent familyEvent)
    {
        _context.FamilyEvents.Update(familyEvent);
        await _context.SaveChangesAsync();
        return familyEvent;
    }

    public async Task DeleteAsync(int id)
    {
        var familyEvent = await _context.FamilyEvents.FindAsync(id);
        if (familyEvent != null)
        {
            _context.FamilyEvents.Remove(familyEvent);
            await _context.SaveChangesAsync();
        }
    }
}
