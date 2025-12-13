using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for Person operations.
/// </summary>
public interface IPersonService
{
    Task<PersonViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<PersonViewModel>> GetAllAsync();
    Task<IEnumerable<PersonViewModel>> SearchAsync(SearchPersonRequest request);
    Task<IEnumerable<PersonViewModel>> GetByHouseholdIdAsync(int householdId);
    Task<PersonViewModel> CreateAsync(CreatePersonRequest request);
    Task<PersonViewModel> UpdateAsync(UpdatePersonRequest request);
    Task DeleteAsync(int id);
}
