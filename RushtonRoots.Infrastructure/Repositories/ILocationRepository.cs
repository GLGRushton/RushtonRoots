using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(int id);
    Task<List<Location>> GetAllAsync();
    Task<List<Location>> SearchAsync(string searchTerm);
    Task<Location> AddAsync(Location location);
    Task<Location> UpdateAsync(Location location);
    Task DeleteAsync(int id);
}
