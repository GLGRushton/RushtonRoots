using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IConflictResolutionService
{
    Task<ConflictResolutionViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<ConflictResolutionViewModel>> GetOpenConflictsAsync();
    Task<ConflictResolutionViewModel> ResolveConflictAsync(ResolveConflictRequest request, string resolverUserId);
}
