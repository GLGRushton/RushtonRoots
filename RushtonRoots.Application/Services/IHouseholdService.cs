using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for Household operations.
/// </summary>
public interface IHouseholdService
{
    Task<HouseholdViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<HouseholdViewModel>> GetAllAsync();
    Task<HouseholdViewModel> CreateAsync(CreateHouseholdRequest request);
    Task<HouseholdViewModel> UpdateAsync(UpdateHouseholdRequest request);
    Task DeleteAsync(int id);
    Task<IEnumerable<PersonViewModel>> GetMembersAsync(int householdId);
    Task AddMemberAsync(AddHouseholdMemberRequest request);
    Task RemoveMemberAsync(int householdId, int personId);
    Task<HouseholdViewModel> UpdateSettingsAsync(UpdateHouseholdSettingsRequest request);
    
    // New methods for Phase 3.1
    Task RemoveMemberByUserIdAsync(int householdId, string userId);
    Task UpdateMemberRoleAsync(int householdId, string userId, string role);
    Task ResendInviteAsync(int householdId, string userId);
}
