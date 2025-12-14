using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class ContributionRepository : IContributionRepository
{
    private readonly RushtonRootsDbContext _context;

    public ContributionRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Contribution?> GetByIdAsync(int id)
    {
        return await _context.Contributions
            .Include(c => c.Contributor)
            .Include(c => c.Reviewer)
            .Include(c => c.Citation)
            .Include(c => c.Approvals)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contribution>> GetByStatusAsync(string status)
    {
        return await _context.Contributions
            .Include(c => c.Contributor)
            .Include(c => c.Reviewer)
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contribution>> GetByContributorAsync(string userId)
    {
        return await _context.Contributions
            .Include(c => c.Reviewer)
            .Where(c => c.ContributorUserId == userId)
            .OrderByDescending(c => c.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contribution>> GetByEntityAsync(string entityType, int entityId)
    {
        return await _context.Contributions
            .Include(c => c.Contributor)
            .Include(c => c.Reviewer)
            .Where(c => c.EntityType == entityType && c.EntityId == entityId)
            .OrderByDescending(c => c.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contribution>> GetPendingContributionsAsync()
    {
        return await GetByStatusAsync("Pending");
    }

    public async Task<Contribution> CreateAsync(Contribution contribution)
    {
        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync();
        return contribution;
    }

    public async Task<Contribution> UpdateAsync(Contribution contribution)
    {
        _context.Contributions.Update(contribution);
        await _context.SaveChangesAsync();
        return contribution;
    }

    public async Task DeleteAsync(int id)
    {
        var contribution = await _context.Contributions.FindAsync(id);
        if (contribution != null)
        {
            _context.Contributions.Remove(contribution);
            await _context.SaveChangesAsync();
        }
    }
}
