using FakeItEasy;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class ActivityFeedServiceTests
{
    [Fact]
    public async Task GetRecentActivitiesAsync_ReturnsActivities()
    {
        // Arrange
        var activities = new List<ActivityFeedItem>
        {
            new ActivityFeedItem
            {
                Id = 1,
                UserId = "user1",
                ActivityType = "ContributionSubmitted",
                Description = "Suggested edit to BirthDate",
                Points = 5,
                User = new ApplicationUser { Id = "user1", UserName = "John Doe" }
            },
            new ActivityFeedItem
            {
                Id = 2,
                UserId = "user2",
                ActivityType = "PersonAdded",
                Description = "Added new person",
                Points = 20,
                User = new ApplicationUser { Id = "user2", UserName = "Jane Smith" }
            }
        };

        var mockRepository = A.Fake<IActivityFeedRepository>();
        A.CallTo(() => mockRepository.GetRecentActivitiesAsync(50)).Returns(activities);

        var service = new ActivityFeedService(mockRepository);

        // Act
        var result = await service.GetRecentActivitiesAsync(50);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("John Doe", result.First().UserName);
    }

    [Fact]
    public async Task RecordActivityAsync_CreatesActivityItem()
    {
        // Arrange
        var userId = "user1";
        var activityType = "ContributionApproved";
        var description = "Your edit was approved";
        var points = 10;

        ActivityFeedItem? capturedActivity = null;

        var mockRepository = A.Fake<IActivityFeedRepository>();
        A.CallTo(() => mockRepository.CreateAsync(A<ActivityFeedItem>._))
            .Invokes((ActivityFeedItem a) => capturedActivity = a)
            .Returns(Task.FromResult(new ActivityFeedItem()));

        var service = new ActivityFeedService(mockRepository);

        // Act
        await service.RecordActivityAsync(userId, activityType, "Person", 5, description, points);

        // Assert
        A.CallTo(() => mockRepository.CreateAsync(A<ActivityFeedItem>._))
            .MustHaveHappenedOnceExactly();

        Assert.NotNull(capturedActivity);
        Assert.Equal(userId, capturedActivity.UserId);
        Assert.Equal(activityType, capturedActivity.ActivityType);
        Assert.Equal(description, capturedActivity.Description);
        Assert.Equal(points, capturedActivity.Points);
        Assert.Equal("Person", capturedActivity.EntityType);
        Assert.Equal(5, capturedActivity.EntityId);
        Assert.True(capturedActivity.IsPublic);
    }

    [Fact]
    public async Task GetUserActivitiesAsync_ReturnsUserSpecificActivities()
    {
        // Arrange
        var userId = "user1";
        var activities = new List<ActivityFeedItem>
        {
            new ActivityFeedItem
            {
                Id = 1,
                UserId = userId,
                ActivityType = "ContributionSubmitted",
                Description = "Suggested edit",
                User = new ApplicationUser { Id = userId, UserName = "John Doe" }
            }
        };

        var mockRepository = A.Fake<IActivityFeedRepository>();
        A.CallTo(() => mockRepository.GetUserActivitiesAsync(userId, 50)).Returns(activities);

        var service = new ActivityFeedService(mockRepository);

        // Act
        var result = await service.GetUserActivitiesAsync(userId, 50);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(userId, result.First().UserId);
    }

    [Fact]
    public async Task GetPublicActivitiesAsync_ReturnsOnlyPublicActivities()
    {
        // Arrange
        var activities = new List<ActivityFeedItem>
        {
            new ActivityFeedItem
            {
                Id = 1,
                UserId = "user1",
                IsPublic = true,
                User = new ApplicationUser { UserName = "User1" }
            },
            new ActivityFeedItem
            {
                Id = 2,
                UserId = "user2",
                IsPublic = true,
                User = new ApplicationUser { UserName = "User2" }
            }
        };

        var mockRepository = A.Fake<IActivityFeedRepository>();
        A.CallTo(() => mockRepository.GetPublicActivitiesAsync(50)).Returns(activities);

        var service = new ActivityFeedService(mockRepository);

        // Act
        var result = await service.GetPublicActivitiesAsync(50);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, a => Assert.True(a.IsPublic));
    }
}
