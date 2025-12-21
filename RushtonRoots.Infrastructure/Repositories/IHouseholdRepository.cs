using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Household entity operations.
/// </summary>
public interface IHouseholdRepository
{
    Task<Household?> GetByIdAsync(int id);
    Task<IEnumerable<Household>> GetAllAsync();
    Task<Household> AddAsync(Household household);
    Task<Household> UpdateAsync(Household household);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> GetMemberCountAsync(int householdId);
    Task<IEnumerable<Person>> GetMembersAsync(int householdId);
    Task AddMemberAsync(int householdId, int personId);
    Task RemoveMemberAsync(int householdId, int personId);
    Task<int?> GetPersonIdFromUserIdAsync(string userId);
    Task<HouseholdPermission?> GetMemberRoleAsync(int householdId, int personId);
    Task UpdateMemberRoleAsync(int householdId, int personId, string role);
    Task<bool> IsHouseholdAdminAsync(int householdId, int personId);
}
