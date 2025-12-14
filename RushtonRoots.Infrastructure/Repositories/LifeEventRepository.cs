using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class LifeEventRepository : ILifeEventRepository
{
    private readonly RushtonRootsDbContext _context;

    public LifeEventRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<LifeEvent?> GetByIdAsync(int id)
    {
        return await _context.LifeEvents
            .Include(e => e.Location)
            .Include(e => e.Source)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<LifeEvent>> GetByPersonIdAsync(int personId)
    {
        return await _context.LifeEvents
            .Include(e => e.Location)
            .Include(e => e.Source)
            .Where(e => e.PersonId == personId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<LifeEvent> AddAsync(LifeEvent lifeEvent)
    {
        _context.LifeEvents.Add(lifeEvent);
        await _context.SaveChangesAsync();
        return lifeEvent;
    }

    public async Task<LifeEvent> UpdateAsync(LifeEvent lifeEvent)
    {
        _context.LifeEvents.Update(lifeEvent);
        await _context.SaveChangesAsync();
        return lifeEvent;
    }

    public async Task DeleteAsync(int id)
    {
        var lifeEvent = await _context.LifeEvents.FindAsync(id);
        if (lifeEvent != null)
        {
            _context.LifeEvents.Remove(lifeEvent);
            await _context.SaveChangesAsync();
        }
    }
}
