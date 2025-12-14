using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface ILifeEventRepository
{
    Task<LifeEvent?> GetByIdAsync(int id);
    Task<List<LifeEvent>> GetByPersonIdAsync(int personId);
    Task<LifeEvent> AddAsync(LifeEvent lifeEvent);
    Task<LifeEvent> UpdateAsync(LifeEvent lifeEvent);
    Task DeleteAsync(int id);
}
