using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IContributionRepository
{
    Task<Contribution?> GetByIdAsync(int id);
    Task<IEnumerable<Contribution>> GetByStatusAsync(string status);
    Task<IEnumerable<Contribution>> GetByContributorAsync(string userId);
    Task<IEnumerable<Contribution>> GetByEntityAsync(string entityType, int entityId);
    Task<IEnumerable<Contribution>> GetPendingContributionsAsync();
    Task<Contribution> CreateAsync(Contribution contribution);
    Task<Contribution> UpdateAsync(Contribution contribution);
    Task DeleteAsync(int id);
}
