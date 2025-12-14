using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class ContributionService : IContributionService
{
    private readonly IContributionRepository _contributionRepository;
    private readonly IActivityFeedService _activityFeedService;
    private readonly IContributionScoreService _scoreService;

    public ContributionService(
        IContributionRepository contributionRepository,
        IActivityFeedService activityFeedService,
        IContributionScoreService scoreService)
    {
        _contributionRepository = contributionRepository;
        _activityFeedService = activityFeedService;
        _scoreService = scoreService;
    }

    public async Task<ContributionViewModel?> GetByIdAsync(int id)
    {
        var contribution = await _contributionRepository.GetByIdAsync(id);
        return contribution != null ? MapToViewModel(contribution) : null;
    }

    public async Task<IEnumerable<ContributionViewModel>> GetByStatusAsync(string status)
    {
        var contributions = await _contributionRepository.GetByStatusAsync(status);
        return contributions.Select(MapToViewModel);
    }

    public async Task<IEnumerable<ContributionViewModel>> GetByContributorAsync(string userId)
    {
        var contributions = await _contributionRepository.GetByContributorAsync(userId);
        return contributions.Select(MapToViewModel);
    }

    public async Task<IEnumerable<ContributionViewModel>> GetPendingContributionsAsync()
    {
        var contributions = await _contributionRepository.GetPendingContributionsAsync();
        return contributions.Select(MapToViewModel);
    }

    public async Task<ContributionViewModel> CreateAsync(CreateContributionRequest request, string contributorUserId)
    {
        // Determine if citation is required (birth/death dates, locations, etc.)
        bool requiresCitation = DetermineIfCitationRequired(request.FieldName);

        var contribution = new Contribution
        {
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            FieldName = request.FieldName,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            Reason = request.Reason,
            ContributorUserId = contributorUserId,
            CitationId = request.CitationId,
            RequiresCitation = requiresCitation,
            Status = "Pending"
        };

        var created = await _contributionRepository.CreateAsync(contribution);

        // Record activity
        await _activityFeedService.RecordActivityAsync(
            contributorUserId,
            "ContributionSubmitted",
            request.EntityType,
            request.EntityId,
            $"Suggested edit to {request.FieldName}",
            5 // Points for submitting a contribution
        );

        // Update score
        await _scoreService.IncrementContributionSubmittedAsync(contributorUserId);

        return MapToViewModel(created);
    }

    public async Task<ContributionViewModel> ReviewAsync(ReviewContributionRequest request, string reviewerUserId)
    {
        var contribution = await _contributionRepository.GetByIdAsync(request.ContributionId);
        if (contribution == null)
        {
            throw new InvalidOperationException("Contribution not found");
        }

        contribution.ReviewerUserId = reviewerUserId;
        contribution.ReviewedAt = DateTime.UtcNow;
        contribution.ReviewNotes = request.Notes;

        if (request.Decision == "Approved")
        {
            contribution.Status = "Approved";
            await _scoreService.IncrementContributionApprovedAsync(contribution.ContributorUserId);
            
            // Record activity for approval
            await _activityFeedService.RecordActivityAsync(
                contribution.ContributorUserId,
                "ContributionApproved",
                contribution.EntityType,
                contribution.EntityId,
                $"Edit to {contribution.FieldName} was approved",
                10 // Points for approved contribution
            );
        }
        else if (request.Decision == "Rejected")
        {
            contribution.Status = "Rejected";
            await _scoreService.IncrementContributionRejectedAsync(contribution.ContributorUserId);
        }
        else if (request.Decision == "RequestMoreInfo")
        {
            contribution.Status = "RequestMoreInfo";
        }

        // Create approval record
        var approval = new ContributionApproval
        {
            ContributionId = contribution.Id,
            ApproverUserId = reviewerUserId,
            Decision = request.Decision,
            Notes = request.Notes,
            DecisionDate = DateTime.UtcNow,
            IsFinalDecision = request.Decision != "RequestMoreInfo"
        };

        contribution.Approvals.Add(approval);

        var updated = await _contributionRepository.UpdateAsync(contribution);
        return MapToViewModel(updated);
    }

    public async Task<bool> ApplyContributionAsync(int contributionId)
    {
        var contribution = await _contributionRepository.GetByIdAsync(contributionId);
        if (contribution == null || contribution.Status != "Approved")
        {
            return false;
        }

        // In a real implementation, this would apply the change to the actual entity
        // For now, we just mark it as applied
        // This would need to use reflection or a strategy pattern to update different entity types

        return true;
    }

    private bool DetermineIfCitationRequired(string fieldName)
    {
        // Require citations for key genealogical facts
        var citationRequiredFields = new[]
        {
            "BirthDate", "BirthPlace", "DeathDate", "DeathPlace",
            "MarriageDate", "MarriagePlace", "BurialPlace"
        };

        return citationRequiredFields.Contains(fieldName);
    }

    private ContributionViewModel MapToViewModel(Contribution contribution)
    {
        return new ContributionViewModel
        {
            Id = contribution.Id,
            EntityType = contribution.EntityType,
            EntityId = contribution.EntityId,
            FieldName = contribution.FieldName,
            OldValue = contribution.OldValue,
            NewValue = contribution.NewValue,
            Reason = contribution.Reason,
            Status = contribution.Status,
            ContributorUserId = contribution.ContributorUserId,
            ContributorName = contribution.Contributor?.UserName,
            ReviewerUserId = contribution.ReviewerUserId,
            ReviewerName = contribution.Reviewer?.UserName,
            ReviewedAt = contribution.ReviewedAt,
            ReviewNotes = contribution.ReviewNotes,
            CitationId = contribution.CitationId,
            RequiresCitation = contribution.RequiresCitation,
            CreatedDateTime = contribution.CreatedDateTime,
            UpdatedDateTime = contribution.UpdatedDateTime
        };
    }
}
