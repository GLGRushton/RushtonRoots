using FakeItEasy;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class ContributionScoreServiceTests
{
    [Fact]
    public async Task GetByUserIdAsync_ReturnsScore_WhenExists()
    {
        // Arrange
        var userId = "user1";
        var score = new ContributionScore
        {
            Id = 1,
            UserId = userId,
            TotalPoints = 100,
            ContributionsSubmitted = 10,
            ContributionsApproved = 8,
            CurrentRank = "Contributor",
            User = new ApplicationUser { Id = userId, UserName = "John Doe" }
        };

        var mockRepository = A.Fake<IContributionScoreRepository>();
        A.CallTo(() => mockRepository.GetByUserIdAsync(userId)).Returns(score);

        var service = new ContributionScoreService(mockRepository);

        // Act
        var result = await service.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(100, result.TotalPoints);
        Assert.Equal("Contributor", result.CurrentRank);
    }

    [Fact]
    public async Task IncrementContributionSubmittedAsync_IncreasesPointsAndCount()
    {
        // Arrange
        var userId = "user1";
        var score = new ContributionScore
        {
            Id = 1,
            UserId = userId,
            TotalPoints = 0,
            ContributionsSubmitted = 0,
            CurrentRank = "Novice"
        };

        var mockRepository = A.Fake<IContributionScoreRepository>();
        A.CallTo(() => mockRepository.GetOrCreateScoreAsync(userId)).Returns(score);
        A.CallTo(() => mockRepository.UpdateAsync(A<ContributionScore>._)).Returns(score);

        var service = new ContributionScoreService(mockRepository);

        // Act
        await service.IncrementContributionSubmittedAsync(userId);

        // Assert
        Assert.Equal(1, score.ContributionsSubmitted);
        Assert.Equal(5, score.TotalPoints);
        Assert.NotNull(score.LastActivityDate);
        
        A.CallTo(() => mockRepository.UpdateAsync(A<ContributionScore>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task IncrementContributionApprovedAsync_UpdatesRankBasedOnPoints()
    {
        // Arrange
        var userId = "user1";
        var score = new ContributionScore
        {
            Id = 1,
            UserId = userId,
            TotalPoints = 40, // Just below Contributor threshold
            ContributionsApproved = 0,
            CurrentRank = "Novice"
        };

        var mockRepository = A.Fake<IContributionScoreRepository>();
        A.CallTo(() => mockRepository.GetOrCreateScoreAsync(userId)).Returns(score);
        A.CallTo(() => mockRepository.UpdateAsync(A<ContributionScore>._)).Returns(score);

        var service = new ContributionScoreService(mockRepository);

        // Act
        await service.IncrementContributionApprovedAsync(userId);

        // Assert
        Assert.Equal(1, score.ContributionsApproved);
        Assert.Equal(50, score.TotalPoints); // 40 + 10
        Assert.Equal("Contributor", score.CurrentRank); // Should upgrade to Contributor
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsTopScorers()
    {
        // Arrange
        var scores = new List<ContributionScore>
        {
            new ContributionScore
            {
                Id = 1,
                UserId = "user1",
                TotalPoints = 500,
                User = new ApplicationUser { UserName = "TopContributor" }
            },
            new ContributionScore
            {
                Id = 2,
                UserId = "user2",
                TotalPoints = 300,
                User = new ApplicationUser { UserName = "SecondPlace" }
            }
        };

        var mockRepository = A.Fake<IContributionScoreRepository>();
        A.CallTo(() => mockRepository.GetLeaderboardAsync(10)).Returns(scores);

        var service = new ContributionScoreService(mockRepository);

        // Act
        var result = await service.GetLeaderboardAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("TopContributor", result.First().UserName);
    }

    [Fact]
    public async Task IncrementPersonAddedAsync_Awards20Points()
    {
        // Arrange
        var userId = "user1";
        var score = new ContributionScore
        {
            Id = 1,
            UserId = userId,
            TotalPoints = 0,
            PeopleAdded = 0,
            CurrentRank = "Novice"
        };

        var mockRepository = A.Fake<IContributionScoreRepository>();
        A.CallTo(() => mockRepository.GetOrCreateScoreAsync(userId)).Returns(score);
        A.CallTo(() => mockRepository.UpdateAsync(A<ContributionScore>._)).Returns(score);

        var service = new ContributionScoreService(mockRepository);

        // Act
        await service.IncrementPersonAddedAsync(userId);

        // Assert
        Assert.Equal(1, score.PeopleAdded);
        Assert.Equal(20, score.TotalPoints);
    }

    [Fact]
    public async Task RankProgression_WorksCorrectly()
    {
        // Arrange
        var userId = "user1";
        var score = new ContributionScore
        {
            Id = 1,
            UserId = userId,
            TotalPoints = 0,
            CurrentRank = "Novice"
        };

        var mockRepository = A.Fake<IContributionScoreRepository>();
        A.CallTo(() => mockRepository.GetOrCreateScoreAsync(userId)).Returns(score);
        A.CallTo(() => mockRepository.UpdateAsync(A<ContributionScore>._)).Returns(score);

        var service = new ContributionScoreService(mockRepository);

        // Act & Assert - Test rank progression
        
        // Novice: < 50 points
        score.TotalPoints = 30;
        await service.IncrementContributionSubmittedAsync(userId);
        Assert.Equal("Novice", score.CurrentRank);

        // Contributor: 50-199 points
        score.TotalPoints = 100;
        await service.IncrementContributionSubmittedAsync(userId);
        Assert.Equal("Contributor", score.CurrentRank);

        // Researcher: 200-499 points
        score.TotalPoints = 300;
        await service.IncrementContributionSubmittedAsync(userId);
        Assert.Equal("Researcher", score.CurrentRank);

        // Historian: 500-999 points
        score.TotalPoints = 600;
        await service.IncrementContributionSubmittedAsync(userId);
        Assert.Equal("Historian", score.CurrentRank);

        // Expert: 1000+ points
        score.TotalPoints = 1200;
        await service.IncrementContributionSubmittedAsync(userId);
        Assert.Equal("Expert", score.CurrentRank);
    }
}
