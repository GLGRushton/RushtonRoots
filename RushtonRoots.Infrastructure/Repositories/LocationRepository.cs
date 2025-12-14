using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly RushtonRootsDbContext _context;

    public LocationRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        return await _context.Locations.FindAsync(id);
    }

    public async Task<List<Location>> GetAllAsync()
    {
        return await _context.Locations
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<List<Location>> SearchAsync(string searchTerm)
    {
        return await _context.Locations
            .Where(l => l.Name.Contains(searchTerm) ||
                       (l.City != null && l.City.Contains(searchTerm)) ||
                       (l.Country != null && l.Country.Contains(searchTerm)))
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Location> AddAsync(Location location)
    {
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task<Location> UpdateAsync(Location location)
    {
        _context.Locations.Update(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task DeleteAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location != null)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }
    }
}
