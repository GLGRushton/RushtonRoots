using FakeItEasy;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class ContributionServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsContributionViewModel_WhenContributionExists()
    {
        // Arrange
        var contributionId = 1;
        var contribution = new Contribution
        {
            Id = contributionId,
            EntityType = "Person",
            EntityId = 5,
            FieldName = "BirthDate",
            OldValue = "1950-01-01",
            NewValue = "1950-02-15",
            Reason = "Found birth certificate",
            Status = "Pending",
            ContributorUserId = "user1",
            Contributor = new ApplicationUser { Id = "user1", UserName = "John Doe" },
            RequiresCitation = true
        };

        var mockRepository = A.Fake<IContributionRepository>();
        var mockActivityService = A.Fake<IActivityFeedService>();
        var mockScoreService = A.Fake<IContributionScoreService>();

        A.CallTo(() => mockRepository.GetByIdAsync(contributionId)).Returns(contribution);

        var service = new ContributionService(mockRepository, mockActivityService, mockScoreService);

        // Act
        var result = await service.GetByIdAsync(contributionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contributionId, result.Id);
        Assert.Equal("Person", result.EntityType);
        Assert.Equal("BirthDate", result.FieldName);
        Assert.Equal("Pending", result.Status);
        Assert.True(result.RequiresCitation);
    }

    [Fact]
    public async Task CreateAsync_CreatesContribution_AndRecordsActivity()
    {
        // Arrange
        var request = new CreateContributionRequest
        {
            EntityType = "Person",
            EntityId = 5,
            FieldName = "BirthPlace",
            OldValue = "Unknown",
            NewValue = "New York, NY",
            Reason = "Found census record",
            CitationId = 10
        };

        var userId = "user1";
        var createdContribution = new Contribution
        {
            Id = 1,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            FieldName = request.FieldName,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            Reason = request.Reason,
            ContributorUserId = userId,
            CitationId = request.CitationId,
            Status = "Pending",
            RequiresCitation = true,
            Contributor = new ApplicationUser { Id = userId, UserName = "John Doe" }
        };

        var mockRepository = A.Fake<IContributionRepository>();
        var mockActivityService = A.Fake<IActivityFeedService>();
        var mockScoreService = A.Fake<IContributionScoreService>();

        A.CallTo(() => mockRepository.CreateAsync(A<Contribution>._)).Returns(createdContribution);

        var service = new ContributionService(mockRepository, mockActivityService, mockScoreService);

        // Act
        var result = await service.CreateAsync(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Pending", result.Status);
        
        // Verify activity was recorded
        A.CallTo(() => mockActivityService.RecordActivityAsync(
            userId,
            "ContributionSubmitted",
            request.EntityType,
            request.EntityId,
            A<string>._,
            5
        )).MustHaveHappenedOnceExactly();

        // Verify score was incremented
        A.CallTo(() => mockScoreService.IncrementContributionSubmittedAsync(userId))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ReviewAsync_ApprovesContribution_AndUpdatesScore()
    {
        // Arrange
        var contributionId = 1;
        var reviewRequest = new ReviewContributionRequest
        {
            ContributionId = contributionId,
            Decision = "Approved",
            Notes = "Looks good, citation verified"
        };

        var contribution = new Contribution
        {
            Id = contributionId,
            EntityType = "Person",
            EntityId = 5,
            FieldName = "BirthDate",
            NewValue = "1950-02-15",
            ContributorUserId = "user1",
            Status = "Pending",
            Approvals = new List<ContributionApproval>(),
            Contributor = new ApplicationUser { Id = "user1", UserName = "John Doe" }
        };

        var mockRepository = A.Fake<IContributionRepository>();
        var mockActivityService = A.Fake<IActivityFeedService>();
        var mockScoreService = A.Fake<IContributionScoreService>();

        A.CallTo(() => mockRepository.GetByIdAsync(contributionId)).Returns(contribution);
        A.CallTo(() => mockRepository.UpdateAsync(A<Contribution>._)).Returns(contribution);

        var service = new ContributionService(mockRepository, mockActivityService, mockScoreService);

        // Act
        var result = await service.ReviewAsync(reviewRequest, "reviewer1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Approved", result.Status);
        
        // Verify score was incremented for approved contribution
        A.CallTo(() => mockScoreService.IncrementContributionApprovedAsync("user1"))
            .MustHaveHappenedOnceExactly();

        // Verify activity was recorded
        A.CallTo(() => mockActivityService.RecordActivityAsync(
            "user1",
            "ContributionApproved",
            contribution.EntityType,
            contribution.EntityId,
            A<string>._,
            10
        )).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ReviewAsync_RejectsContribution_AndUpdatesScore()
    {
        // Arrange
        var contributionId = 1;
        var reviewRequest = new ReviewContributionRequest
        {
            ContributionId = contributionId,
            Decision = "Rejected",
            Notes = "Insufficient evidence"
        };

        var contribution = new Contribution
        {
            Id = contributionId,
            EntityType = "Person",
            EntityId = 5,
            ContributorUserId = "user1",
            Status = "Pending",
            Approvals = new List<ContributionApproval>()
        };

        var mockRepository = A.Fake<IContributionRepository>();
        var mockActivityService = A.Fake<IActivityFeedService>();
        var mockScoreService = A.Fake<IContributionScoreService>();

        A.CallTo(() => mockRepository.GetByIdAsync(contributionId)).Returns(contribution);
        A.CallTo(() => mockRepository.UpdateAsync(A<Contribution>._)).Returns(contribution);

        var service = new ContributionService(mockRepository, mockActivityService, mockScoreService);

        // Act
        var result = await service.ReviewAsync(reviewRequest, "reviewer1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rejected", result.Status);
        
        // Verify score was incremented for rejected contribution
        A.CallTo(() => mockScoreService.IncrementContributionRejectedAsync("user1"))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetPendingContributionsAsync_ReturnsPendingContributions()
    {
        // Arrange
        var contributions = new List<Contribution>
        {
            new Contribution
            {
                Id = 1,
                Status = "Pending",
                Contributor = new ApplicationUser { UserName = "User1" }
            },
            new Contribution
            {
                Id = 2,
                Status = "Pending",
                Contributor = new ApplicationUser { UserName = "User2" }
            }
        };

        var mockRepository = A.Fake<IContributionRepository>();
        var mockActivityService = A.Fake<IActivityFeedService>();
        var mockScoreService = A.Fake<IContributionScoreService>();

        A.CallTo(() => mockRepository.GetPendingContributionsAsync()).Returns(contributions);

        var service = new ContributionService(mockRepository, mockActivityService, mockScoreService);

        // Act
        var result = await service.GetPendingContributionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
