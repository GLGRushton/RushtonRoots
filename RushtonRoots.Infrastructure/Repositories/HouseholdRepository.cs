using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Household entity operations.
/// </summary>
public class HouseholdRepository : IHouseholdRepository
{
    private readonly RushtonRootsDbContext _context;

    public HouseholdRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Household?> GetByIdAsync(int id)
    {
        return await _context.Households
            .Include(h => h.AnchorPerson)
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<Household>> GetAllAsync()
    {
        return await _context.Households
            .Include(h => h.AnchorPerson)
            .Include(h => h.Members)
            .OrderBy(h => h.HouseholdName)
            .ToListAsync();
    }

    public async Task<Household> AddAsync(Household household)
    {
        _context.Households.Add(household);
        await _context.SaveChangesAsync();
        return household;
    }

    public async Task<Household> UpdateAsync(Household household)
    {
        _context.Households.Update(household);
        await _context.SaveChangesAsync();
        return household;
    }

    public async Task DeleteAsync(int id)
    {
        var household = await _context.Households.FindAsync(id);
        if (household != null)
        {
            _context.Households.Remove(household);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Households.AnyAsync(h => h.Id == id);
    }

    public async Task<int> GetMemberCountAsync(int householdId)
    {
        return await _context.People.CountAsync(p => p.HouseholdId == householdId);
    }
}
