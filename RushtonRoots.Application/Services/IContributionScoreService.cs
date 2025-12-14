using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Services;

public interface IContributionScoreService
{
    Task<ContributionScoreViewModel?> GetByUserIdAsync(string userId);
    Task<IEnumerable<ContributionScoreViewModel>> GetLeaderboardAsync(int count = 10);
    Task IncrementContributionSubmittedAsync(string userId);
    Task IncrementContributionApprovedAsync(string userId);
    Task IncrementContributionRejectedAsync(string userId);
    Task IncrementCitationAddedAsync(string userId);
    Task IncrementConflictResolvedAsync(string userId);
    Task IncrementPersonAddedAsync(string userId);
    Task IncrementPhotoUploadedAsync(string userId);
    Task IncrementStoryWrittenAsync(string userId);
}
