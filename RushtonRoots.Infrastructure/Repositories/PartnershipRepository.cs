using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository for Partnership operations.
/// </summary>
public class PartnershipRepository : IPartnershipRepository
{
    private readonly RushtonRootsDbContext _context;

    public PartnershipRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Partnership?> GetByIdAsync(int id)
    {
        return await _context.Partnerships
            .Include(p => p.PersonA)
            .Include(p => p.PersonB)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Partnership>> GetAllAsync()
    {
        return await _context.Partnerships
            .Include(p => p.PersonA)
            .Include(p => p.PersonB)
            .ToListAsync();
    }

    public async Task<IEnumerable<Partnership>> GetByPersonIdAsync(int personId)
    {
        return await _context.Partnerships
            .Include(p => p.PersonA)
            .Include(p => p.PersonB)
            .Where(p => p.PersonAId == personId || p.PersonBId == personId)
            .ToListAsync();
    }

    public async Task<Partnership> AddAsync(Partnership partnership)
    {
        _context.Partnerships.Add(partnership);
        await _context.SaveChangesAsync();
        return partnership;
    }

    public async Task<Partnership> UpdateAsync(Partnership partnership)
    {
        _context.Entry(partnership).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return partnership;
    }

    public async Task DeleteAsync(int id)
    {
        var partnership = await _context.Partnerships.FindAsync(id);
        if (partnership != null)
        {
            _context.Partnerships.Remove(partnership);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasCircularRelationshipAsync(int personAId, int personBId)
    {
        // For partnerships, we just need to check if a partnership already exists
        // between these two people (in either direction)
        return await PartnershipExistsAsync(personAId, personBId);
    }

    public async Task<bool> PartnershipExistsAsync(int personAId, int personBId)
    {
        return await _context.Partnerships
            .AnyAsync(p => 
                (p.PersonAId == personAId && p.PersonBId == personBId) ||
                (p.PersonAId == personBId && p.PersonBId == personAId));
    }
}
