using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class FamilyTaskRepository : IFamilyTaskRepository
{
    private readonly RushtonRootsDbContext _context;

    public FamilyTaskRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<FamilyTask?> GetByIdAsync(int id)
    {
        return await _context.FamilyTasks
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Household)
            .Include(t => t.RelatedEvent)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<FamilyTask>> GetAllAsync()
    {
        return await _context.FamilyTasks
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Household)
            .Include(t => t.RelatedEvent)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FamilyTask>> GetByHouseholdIdAsync(int householdId)
    {
        return await _context.FamilyTasks
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Household)
            .Include(t => t.RelatedEvent)
            .Where(t => t.HouseholdId == householdId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FamilyTask>> GetByAssignedUserIdAsync(string userId)
    {
        return await _context.FamilyTasks
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Household)
            .Include(t => t.RelatedEvent)
            .Where(t => t.AssignedToUserId == userId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FamilyTask>> GetByStatusAsync(string status)
    {
        return await _context.FamilyTasks
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Household)
            .Include(t => t.RelatedEvent)
            .Where(t => t.Status == status)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FamilyTask>> GetByEventIdAsync(int eventId)
    {
        return await _context.FamilyTasks
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Household)
            .Include(t => t.RelatedEvent)
            .Where(t => t.RelatedEventId == eventId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<FamilyTask> AddAsync(FamilyTask familyTask)
    {
        _context.FamilyTasks.Add(familyTask);
        await _context.SaveChangesAsync();
        return familyTask;
    }

    public async Task<FamilyTask> UpdateAsync(FamilyTask familyTask)
    {
        _context.FamilyTasks.Update(familyTask);
        await _context.SaveChangesAsync();
        return familyTask;
    }

    public async Task DeleteAsync(int id)
    {
        var familyTask = await _context.FamilyTasks.FindAsync(id);
        if (familyTask != null)
        {
            _context.FamilyTasks.Remove(familyTask);
            await _context.SaveChangesAsync();
        }
    }
}
