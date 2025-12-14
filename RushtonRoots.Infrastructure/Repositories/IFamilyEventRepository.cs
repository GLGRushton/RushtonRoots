using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IFamilyEventRepository
{
    Task<FamilyEvent?> GetByIdAsync(int id);
    Task<IEnumerable<FamilyEvent>> GetAllAsync();
    Task<IEnumerable<FamilyEvent>> GetByHouseholdIdAsync(int householdId);
    Task<IEnumerable<FamilyEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<FamilyEvent>> GetUpcomingEventsAsync(int count = 10);
    Task<FamilyEvent> AddAsync(FamilyEvent familyEvent);
    Task<FamilyEvent> UpdateAsync(FamilyEvent familyEvent);
    Task DeleteAsync(int id);
}
