using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Person entity operations.
/// </summary>
public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(int id);
    Task<IEnumerable<Person>> GetAllAsync();
    Task<IEnumerable<Person>> SearchAsync(SearchPersonRequest request);
    Task<IEnumerable<Person>> GetByHouseholdIdAsync(int householdId);
    Task<Person> AddAsync(Person person);
    Task<Person> UpdateAsync(Person person);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
