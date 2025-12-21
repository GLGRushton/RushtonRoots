using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for ParentChild operations.
/// </summary>
public interface IParentChildService
{
    Task<ParentChildViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<ParentChildViewModel>> GetAllAsync();
    Task<IEnumerable<ParentChildViewModel>> GetByParentIdAsync(int parentId);
    Task<IEnumerable<ParentChildViewModel>> GetByChildIdAsync(int childId);
    Task<ParentChildViewModel> CreateAsync(CreateParentChildRequest request);
    Task<ParentChildViewModel> UpdateAsync(UpdateParentChildRequest request);
    Task DeleteAsync(int id);
    
    // Phase 4.2: Evidence & Family Context
    Task<IEnumerable<SourceViewModel>> GetEvidenceAsync(int relationshipId);
    Task<IEnumerable<LifeEventViewModel>> GetRelatedEventsAsync(int relationshipId);
    Task<IEnumerable<PersonViewModel>> GetGrandparentsAsync(int relationshipId);
    Task<IEnumerable<PersonViewModel>> GetSiblingsAsync(int relationshipId);
    
    // Phase 4.3: Verification System
    Task<ParentChildViewModel> VerifyRelationshipAsync(int relationshipId, string verifiedBy);
    Task<ParentChildViewModel> UpdateNotesAsync(int relationshipId, string notes);
}
