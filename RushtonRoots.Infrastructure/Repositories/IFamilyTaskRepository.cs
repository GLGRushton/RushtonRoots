using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IFamilyTaskRepository
{
    Task<FamilyTask?> GetByIdAsync(int id);
    Task<IEnumerable<FamilyTask>> GetAllAsync();
    Task<IEnumerable<FamilyTask>> GetByHouseholdIdAsync(int householdId);
    Task<IEnumerable<FamilyTask>> GetByAssignedUserIdAsync(string userId);
    Task<IEnumerable<FamilyTask>> GetByStatusAsync(string status);
    Task<IEnumerable<FamilyTask>> GetByEventIdAsync(int eventId);
    Task<FamilyTask> AddAsync(FamilyTask familyTask);
    Task<FamilyTask> UpdateAsync(FamilyTask familyTask);
    Task DeleteAsync(int id);
}
