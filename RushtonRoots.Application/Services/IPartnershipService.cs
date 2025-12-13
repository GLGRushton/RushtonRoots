using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for Partnership operations.
/// </summary>
public interface IPartnershipService
{
    Task<PartnershipViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<PartnershipViewModel>> GetAllAsync();
    Task<IEnumerable<PartnershipViewModel>> GetByPersonIdAsync(int personId);
    Task<PartnershipViewModel> CreateAsync(CreatePartnershipRequest request);
    Task<PartnershipViewModel> UpdateAsync(UpdatePartnershipRequest request);
    Task DeleteAsync(int id);
}
