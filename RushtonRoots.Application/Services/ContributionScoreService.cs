using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class ContributionScoreService : IContributionScoreService
{
    private readonly IContributionScoreRepository _scoreRepository;

    public ContributionScoreService(IContributionScoreRepository scoreRepository)
    {
        _scoreRepository = scoreRepository;
    }

    public async Task<ContributionScoreViewModel?> GetByUserIdAsync(string userId)
    {
        var score = await _scoreRepository.GetByUserIdAsync(userId);
        return score != null ? MapToViewModel(score) : null;
    }

    public async Task<IEnumerable<ContributionScoreViewModel>> GetLeaderboardAsync(int count = 10)
    {
        var scores = await _scoreRepository.GetLeaderboardAsync(count);
        return scores.Select(MapToViewModel);
    }

    public async Task IncrementContributionSubmittedAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.ContributionsSubmitted++;
        score.TotalPoints += 5;
        score.LastActivityDate = DateTime.UtcNow;
        await UpdateRankAndSave(score);
    }

    public async Task IncrementContributionApprovedAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.ContributionsApproved++;
        score.TotalPoints += 10;
        score.LastActivityDate = DateTime.UtcNow;
        await UpdateRankAndSave(score);
    }

    public async Task IncrementContributionRejectedAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.ContributionsRejected++;
        // No points for rejected contributions
        await _scoreRepository.UpdateAsync(score);
    }

    public async Task IncrementCitationAddedAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.CitationsAdded++;
        score.TotalPoints += 8;
        score.LastActivityDate = DateTime.UtcNow;
        await UpdateRankAndSave(score);
    }

    public async Task IncrementConflictResolvedAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.ConflictsResolved++;
        score.TotalPoints += 15;
        score.LastActivityDate = DateTime.UtcNow;
        await UpdateRankAndSave(score);
    }

    public async Task IncrementPersonAddedAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.PeopleAdded++;
        score.TotalPoints += 20;
        score.LastActivityDate = DateTime.UtcNow;
        await UpdateRankAndSave(score);
    }

    public async Task IncrementPhotoUploadedAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.PhotosUploaded++;
        score.TotalPoints += 3;
        score.LastActivityDate = DateTime.UtcNow;
        await UpdateRankAndSave(score);
    }

    public async Task IncrementStoryWrittenAsync(string userId)
    {
        var score = await _scoreRepository.GetOrCreateScoreAsync(userId);
        score.StoriesWritten++;
        score.TotalPoints += 25;
        score.LastActivityDate = DateTime.UtcNow;
        await UpdateRankAndSave(score);
    }

    private async Task UpdateRankAndSave(ContributionScore score)
    {
        // Update rank based on total points
        score.CurrentRank = score.TotalPoints switch
        {
            < 50 => "Novice",
            < 200 => "Contributor",
            < 500 => "Researcher",
            < 1000 => "Historian",
            _ => "Expert"
        };

        await _scoreRepository.UpdateAsync(score);
    }

    private ContributionScoreViewModel MapToViewModel(ContributionScore score)
    {
        return new ContributionScoreViewModel
        {
            Id = score.Id,
            UserId = score.UserId,
            UserName = score.User?.UserName ?? "Unknown",
            TotalPoints = score.TotalPoints,
            ContributionsSubmitted = score.ContributionsSubmitted,
            ContributionsApproved = score.ContributionsApproved,
            ContributionsRejected = score.ContributionsRejected,
            CitationsAdded = score.CitationsAdded,
            ConflictsResolved = score.ConflictsResolved,
            PeopleAdded = score.PeopleAdded,
            PhotosUploaded = score.PhotosUploaded,
            StoriesWritten = score.StoriesWritten,
            LastActivityDate = score.LastActivityDate,
            CurrentRank = score.CurrentRank
        };
    }
}
