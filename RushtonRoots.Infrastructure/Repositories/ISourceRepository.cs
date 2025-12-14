using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface ISourceRepository
{
    Task<Source?> GetByIdAsync(int id);
    Task<List<Source>> GetAllAsync();
    Task<List<Source>> SearchAsync(string searchTerm);
    Task<Source> AddAsync(Source source);
    Task<Source> UpdateAsync(Source source);
    Task DeleteAsync(int id);
}
