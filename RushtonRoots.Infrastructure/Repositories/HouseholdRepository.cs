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

    public async Task<IEnumerable<Person>> GetMembersAsync(int householdId)
    {
        return await _context.People
            .Where(p => p.HouseholdId == householdId)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task AddMemberAsync(int householdId, int personId)
    {
        var person = await _context.People.FindAsync(personId);
        if (person == null)
        {
            throw new KeyNotFoundException($"Person with ID {personId} not found.");
        }

        var householdExists = await ExistsAsync(householdId);
        if (!householdExists)
        {
            throw new KeyNotFoundException($"Household with ID {householdId} not found.");
        }

        person.HouseholdId = householdId;
        _context.People.Update(person);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int householdId, int personId)
    {
        var person = await _context.People.FindAsync(personId);
        if (person == null)
        {
            throw new KeyNotFoundException($"Person with ID {personId} not found.");
        }

        if (person.HouseholdId != householdId)
        {
            throw new InvalidOperationException($"Person with ID {personId} is not a member of household {householdId}.");
        }

        // Since HouseholdId is required in the domain model, we cannot simply remove a person from a household.
        // To remove a person from a household, they must be moved to another household first.
        // Use the AddMemberAsync method to move them to a different household, or delete the person entirely.
        throw new InvalidOperationException("Cannot remove a person from a household without assigning them to another household. Use the Person API to update their household assignment or delete the person.");
    }

    public async Task<int?> GetPersonIdFromUserIdAsync(string userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        return user?.PersonId;
    }

    public async Task<HouseholdPermission?> GetMemberRoleAsync(int householdId, int personId)
    {
        return await _context.HouseholdPermissions
            .FirstOrDefaultAsync(hp => hp.HouseholdId == householdId && hp.PersonId == personId);
    }

    public async Task UpdateMemberRoleAsync(int householdId, int personId, string role)
    {
        var permission = await GetMemberRoleAsync(householdId, personId);
        
        if (permission == null)
        {
            // Create new permission if it doesn't exist
            permission = new HouseholdPermission
            {
                HouseholdId = householdId,
                PersonId = personId,
                Role = role
            };
            _context.HouseholdPermissions.Add(permission);
        }
        else
        {
            permission.Role = role;
            _context.HouseholdPermissions.Update(permission);
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsHouseholdAdminAsync(int householdId, int personId)
    {
        var permission = await GetMemberRoleAsync(householdId, personId);
        return permission?.Role == "ADMIN";
    }

    public async Task<int> GetPhotoCountAsync(int householdId)
    {
        // Get all person IDs in the household
        var memberIds = await _context.People
            .Where(p => p.HouseholdId == householdId)
            .Select(p => p.Id)
            .ToListAsync();

        // Count photos for all members
        return await _context.PersonPhotos
            .Where(pp => memberIds.Contains(pp.PersonId))
            .CountAsync();
    }

    public async Task<int> GetDocumentCountAsync(int householdId)
    {
        // Get all person IDs in the household
        var memberIds = await _context.People
            .Where(p => p.HouseholdId == householdId)
            .Select(p => p.Id)
            .ToListAsync();

        // Count documents associated with household members
        return await _context.DocumentPeople
            .Where(dp => memberIds.Contains(dp.PersonId))
            .Select(dp => dp.DocumentId)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> GetRelationshipCountAsync(int householdId)
    {
        // Get all person IDs in the household
        var memberIds = await _context.People
            .Where(p => p.HouseholdId == householdId)
            .Select(p => p.Id)
            .ToListAsync();

        // Count partnerships involving household members
        var partnershipCount = await _context.Partnerships
            .Where(p => memberIds.Contains(p.PersonAId) || memberIds.Contains(p.PersonBId))
            .CountAsync();

        // Count parent-child relationships involving household members
        var parentChildCount = await _context.ParentChildren
            .Where(pc => memberIds.Contains(pc.ParentPersonId) || memberIds.Contains(pc.ChildPersonId))
            .CountAsync();

        return partnershipCount + parentChildCount;
    }

    public async Task<int> GetEventCountAsync(int householdId)
    {
        // Count family events directly associated with the household
        return await _context.FamilyEvents
            .Where(e => e.HouseholdId == householdId)
            .CountAsync();
    }
}
