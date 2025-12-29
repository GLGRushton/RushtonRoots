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
        var query = _context.People
            .Include(p => p.Household)
            .Include(p => p.LifeEvents)
                .ThenInclude(e => e.Location)
            .AsQueryable();

        // Basic name search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm));
        }

        // Surname-only search
        if (!string.IsNullOrWhiteSpace(request.Surname))
        {
            var surname = request.Surname.ToLower();
            query = query.Where(p => p.LastName.ToLower().Contains(surname));
        }

        // Household filter
        if (request.HouseholdId.HasValue)
        {
            query = query.Where(p => p.HouseholdId == request.HouseholdId.Value);
        }

        // Deceased status filter
        if (request.IsDeceased.HasValue)
        {
            query = query.Where(p => p.IsDeceased == request.IsDeceased.Value);
        }

        // Birth date range filter
        if (request.BirthDateFrom.HasValue)
        {
            query = query.Where(p => p.DateOfBirth.HasValue && p.DateOfBirth.Value >= request.BirthDateFrom.Value);
        }
        if (request.BirthDateTo.HasValue)
        {
            query = query.Where(p => p.DateOfBirth.HasValue && p.DateOfBirth.Value <= request.BirthDateTo.Value);
        }

        // Death date range filter
        if (request.DeathDateFrom.HasValue)
        {
            query = query.Where(p => p.DateOfDeath.HasValue && p.DateOfDeath.Value >= request.DeathDateFrom.Value);
        }
        if (request.DeathDateTo.HasValue)
        {
            query = query.Where(p => p.DateOfDeath.HasValue && p.DateOfDeath.Value <= request.DeathDateTo.Value);
        }

        // Location-based search (people with events at specific location)
        if (request.LocationId.HasValue)
        {
            query = query.Where(p => p.LifeEvents.Any(e => e.LocationId == request.LocationId.Value));
        }

        // Event type search (people with specific event type)
        if (!string.IsNullOrWhiteSpace(request.EventType))
        {
            var eventType = request.EventType.ToLower();
            query = query.Where(p => p.LifeEvents.Any(e => e.EventType.ToLower() == eventType));
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

    public async Task<Person?> GetYoungestPersonAsync()
    {
        return await _context.People
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.DateOfBirth)
            .FirstOrDefaultAsync();
    }
}
