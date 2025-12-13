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
}
