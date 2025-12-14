using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IContributionScoreRepository
{
    Task<ContributionScore?> GetByIdAsync(int id);
    Task<ContributionScore?> GetByUserIdAsync(string userId);
    Task<IEnumerable<ContributionScore>> GetLeaderboardAsync(int count = 10);
    Task<ContributionScore> CreateAsync(ContributionScore score);
    Task<ContributionScore> UpdateAsync(ContributionScore score);
    Task<ContributionScore> GetOrCreateScoreAsync(string userId);
}
