using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class ConflictResolutionService : IConflictResolutionService
{
    private readonly IConflictResolutionRepository _conflictRepository;
    private readonly IActivityFeedService _activityFeedService;
    private readonly IContributionScoreService _scoreService;

    public ConflictResolutionService(
        IConflictResolutionRepository conflictRepository,
        IActivityFeedService activityFeedService,
        IContributionScoreService scoreService)
    {
        _conflictRepository = conflictRepository;
        _activityFeedService = activityFeedService;
        _scoreService = scoreService;
    }

    public async Task<ConflictResolutionViewModel?> GetByIdAsync(int id)
    {
        var conflict = await _conflictRepository.GetByIdAsync(id);
        return conflict != null ? MapToViewModel(conflict) : null;
    }

    public async Task<IEnumerable<ConflictResolutionViewModel>> GetOpenConflictsAsync()
    {
        var conflicts = await _conflictRepository.GetOpenConflictsAsync();
        return conflicts.Select(MapToViewModel);
    }

    public async Task<ConflictResolutionViewModel> ResolveConflictAsync(ResolveConflictRequest request, string resolverUserId)
    {
        var conflict = await _conflictRepository.GetByIdAsync(request.ConflictId);
        if (conflict == null)
        {
            throw new InvalidOperationException("Conflict not found");
        }

        conflict.Resolution = request.Resolution;
        conflict.ResolutionNotes = request.ResolutionNotes;
        conflict.ResolvedByUserId = resolverUserId;
        conflict.ResolvedAt = DateTime.UtcNow;
        conflict.AcceptedCitationId = request.AcceptedCitationId;
        conflict.Status = "Resolved";

        var updated = await _conflictRepository.UpdateAsync(conflict);

        // Record activity
        await _activityFeedService.RecordActivityAsync(
            resolverUserId,
            "ConflictResolved",
            conflict.EntityType,
            conflict.EntityId,
            $"Resolved conflict on {conflict.FieldName}",
            15
        );

        // Update score
        await _scoreService.IncrementConflictResolvedAsync(resolverUserId);

        return MapToViewModel(updated);
    }

    private ConflictResolutionViewModel MapToViewModel(ConflictResolution conflict)
    {
        return new ConflictResolutionViewModel
        {
            Id = conflict.Id,
            EntityType = conflict.EntityType,
            EntityId = conflict.EntityId,
            FieldName = conflict.FieldName,
            ContributionId = conflict.ContributionId,
            ConflictType = conflict.ConflictType,
            CurrentValue = conflict.CurrentValue,
            ConflictingValue = conflict.ConflictingValue,
            Status = conflict.Status,
            Resolution = conflict.Resolution,
            ResolutionNotes = conflict.ResolutionNotes,
            ResolvedByUserId = conflict.ResolvedByUserId,
            ResolvedByName = conflict.ResolvedBy?.UserName,
            ResolvedAt = conflict.ResolvedAt,
            AcceptedCitationId = conflict.AcceptedCitationId,
            CreatedDateTime = conflict.CreatedDateTime
        };
    }
}
