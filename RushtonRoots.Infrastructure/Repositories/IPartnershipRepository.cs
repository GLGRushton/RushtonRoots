using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Partnership operations.
/// </summary>
public interface IPartnershipRepository
{
    Task<Partnership?> GetByIdAsync(int id);
    Task<IEnumerable<Partnership>> GetAllAsync();
    Task<IEnumerable<Partnership>> GetByPersonIdAsync(int personId);
    Task<Partnership> AddAsync(Partnership partnership);
    Task<Partnership> UpdateAsync(Partnership partnership);
    Task DeleteAsync(int id);
    Task<bool> HasCircularRelationshipAsync(int personAId, int personBId);
    Task<bool> PartnershipExistsAsync(int personAId, int personBId);
}
