using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IContributionService
{
    Task<ContributionViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<ContributionViewModel>> GetByStatusAsync(string status);
    Task<IEnumerable<ContributionViewModel>> GetByContributorAsync(string userId);
    Task<IEnumerable<ContributionViewModel>> GetPendingContributionsAsync();
    Task<ContributionViewModel> CreateAsync(CreateContributionRequest request, string contributorUserId);
    Task<ContributionViewModel> ReviewAsync(ReviewContributionRequest request, string reviewerUserId);
    Task<bool> ApplyContributionAsync(int contributionId);
}
