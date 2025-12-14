using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class ConflictResolutionRepository : IConflictResolutionRepository
{
    private readonly RushtonRootsDbContext _context;

    public ConflictResolutionRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<ConflictResolution?> GetByIdAsync(int id)
    {
        return await _context.ConflictResolutions
            .Include(cr => cr.Contribution)
            .Include(cr => cr.ResolvedBy)
            .Include(cr => cr.AcceptedCitation)
            .FirstOrDefaultAsync(cr => cr.Id == id);
    }

    public async Task<IEnumerable<ConflictResolution>> GetByStatusAsync(string status)
    {
        return await _context.ConflictResolutions
            .Include(cr => cr.ResolvedBy)
            .Where(cr => cr.Status == status)
            .OrderByDescending(cr => cr.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<ConflictResolution>> GetByEntityAsync(string entityType, int entityId)
    {
        return await _context.ConflictResolutions
            .Include(cr => cr.ResolvedBy)
            .Where(cr => cr.EntityType == entityType && cr.EntityId == entityId)
            .OrderByDescending(cr => cr.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<ConflictResolution>> GetOpenConflictsAsync()
    {
        return await GetByStatusAsync("Open");
    }

    public async Task<ConflictResolution> CreateAsync(ConflictResolution conflict)
    {
        _context.ConflictResolutions.Add(conflict);
        await _context.SaveChangesAsync();
        return conflict;
    }

    public async Task<ConflictResolution> UpdateAsync(ConflictResolution conflict)
    {
        _context.ConflictResolutions.Update(conflict);
        await _context.SaveChangesAsync();
        return conflict;
    }

    public async Task DeleteAsync(int id)
    {
        var conflict = await _context.ConflictResolutions.FindAsync(id);
        if (conflict != null)
        {
            _context.ConflictResolutions.Remove(conflict);
            await _context.SaveChangesAsync();
        }
    }
}
