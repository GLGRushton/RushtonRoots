using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository interface for ParentChild operations.
/// </summary>
public interface IParentChildRepository
{
    Task<ParentChild?> GetByIdAsync(int id);
    Task<IEnumerable<ParentChild>> GetAllAsync();
    Task<IEnumerable<ParentChild>> GetByParentIdAsync(int parentId);
    Task<IEnumerable<ParentChild>> GetByChildIdAsync(int childId);
    Task<ParentChild> AddAsync(ParentChild parentChild);
    Task<ParentChild> UpdateAsync(ParentChild parentChild);
    Task DeleteAsync(int id);
    Task<bool> HasCircularRelationshipAsync(int parentId, int childId);
    Task<bool> RelationshipExistsAsync(int parentId, int childId);
    
    // Phase 4.2: Evidence & Family Context
    Task<List<Source>> GetSourcesAsync(int relationshipId);
    Task<List<Person>> GetGrandparentsAsync(int relationshipId);
    Task<List<Person>> GetSiblingsAsync(int relationshipId);
}
