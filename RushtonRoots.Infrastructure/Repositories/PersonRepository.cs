using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Person entity operations.
/// </summary>
public class PersonRepository : IPersonRepository
{
    private readonly RushtonRootsDbContext _context;

    public PersonRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Person?> GetByIdAsync(int id)
    {
        return await _context.People
            .Include(p => p.Household)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        return await _context.People
            .Include(p => p.Household)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> SearchAsync(SearchPersonRequest request)
    {
        var query = _context.People.Include(p => p.Household).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm));
        }

        if (request.HouseholdId.HasValue)
        {
            query = query.Where(p => p.HouseholdId == request.HouseholdId.Value);
        }

        if (request.IsDeceased.HasValue)
        {
            query = query.Where(p => p.IsDeceased == request.IsDeceased.Value);
        }

        return await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetByHouseholdIdAsync(int householdId)
    {
        return await _context.People
            .Include(p => p.Household)
            .Where(p => p.HouseholdId == householdId)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<Person> AddAsync(Person person)
    {
        _context.People.Add(person);
        await _context.SaveChangesAsync();
        return person;
    }

    public async Task<Person> UpdateAsync(Person person)
    {
        _context.People.Update(person);
        await _context.SaveChangesAsync();
        return person;
    }

    public async Task DeleteAsync(int id)
    {
        var person = await _context.People.FindAsync(id);
        if (person != null)
        {
            _context.People.Remove(person);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.People.AnyAsync(p => p.Id == id);
    }
}
