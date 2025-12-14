using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IConflictResolutionRepository
{
    Task<ConflictResolution?> GetByIdAsync(int id);
    Task<IEnumerable<ConflictResolution>> GetByStatusAsync(string status);
    Task<IEnumerable<ConflictResolution>> GetByEntityAsync(string entityType, int entityId);
    Task<IEnumerable<ConflictResolution>> GetOpenConflictsAsync();
    Task<ConflictResolution> CreateAsync(ConflictResolution conflict);
    Task<ConflictResolution> UpdateAsync(ConflictResolution conflict);
    Task DeleteAsync(int id);
}
