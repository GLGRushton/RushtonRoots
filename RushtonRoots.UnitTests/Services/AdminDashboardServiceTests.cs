using Microsoft.EntityFrameworkCore;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class AdminDashboardServiceTests
{
    private RushtonRootsDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new RushtonRootsDbContext(options);
        
        // Note: In-memory database doesn't automatically call SaveChanges override
        // CreatedDateTime must be set manually in tests
        return context;
    }

    [Fact]
    public async Task GetSystemStatisticsAsync_ReturnsCorrectStatistics_WhenDataExists()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();

        // Add test users to context
        context.Users.AddRange(
            new ApplicationUser { Id = "user1", UserName = "user1@test.com" },
            new ApplicationUser { Id = "user2", UserName = "user2@test.com" },
            new ApplicationUser { Id = "user3", UserName = "user3@test.com" }
        );
        
        var household1 = new Household { Id = 1, HouseholdName = "Test Household 1", AnchorPersonId = 1 };
        var household2 = new Household { Id = 2, HouseholdName = "Test Household 2", AnchorPersonId = 2 };
        context.Households.AddRange(household1, household2);
        
        var people = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1, IsDeleted = false },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith", HouseholdId = 1, IsDeleted = false },
            new Person { Id = 3, FirstName = "Bob", LastName = "Jones", HouseholdId = 2, IsDeleted = false },
            new Person { Id = 4, FirstName = "Deleted", LastName = "Person", HouseholdId = 2, IsDeleted = true }
        };
        context.People.AddRange(people);
        
        context.PersonPhotos.AddRange(
            new PersonPhoto { Id = 1, PersonId = 1, PhotoUrl = "photo1.jpg" },
            new PersonPhoto { Id = 2, PersonId = 2, PhotoUrl = "photo2.jpg" },
            new PersonPhoto { Id = 3, PersonId = 3, PhotoUrl = "photo3.jpg" }
        );
        
        await context.SaveChangesAsync();
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetSystemStatisticsAsync();
        
        // Assert
        Assert.Equal(3, result.TotalUsers);
        Assert.Equal(2, result.TotalHouseholds);
        Assert.Equal(3, result.TotalPersons); // Only non-deleted persons
        Assert.Equal(3, result.MediaItems);
    }

    [Fact]
    public async Task GetSystemStatisticsAsync_ReturnsZeroStatistics_WhenNoDataExists()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetSystemStatisticsAsync();
        
        // Assert
        Assert.Equal(0, result.TotalUsers);
        Assert.Equal(0, result.TotalHouseholds);
        Assert.Equal(0, result.TotalPersons);
        Assert.Equal(0, result.MediaItems);
    }

    [Fact]
    public async Task GetSystemStatisticsAsync_ExcludesDeletedPersons()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        

        context.Users.Add(new ApplicationUser { Id = "user1", UserName = "user1@test.com" });
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        context.People.AddRange(
            new Person { Id = 1, FirstName = "Active", LastName = "User", HouseholdId = 1, IsDeleted = false },
            new Person { Id = 2, FirstName = "Deleted", LastName = "User1", HouseholdId = 1, IsDeleted = true },
            new Person { Id = 3, FirstName = "Deleted", LastName = "User2", HouseholdId = 1, IsDeleted = true }
        );
        
        await context.SaveChangesAsync();
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetSystemStatisticsAsync();
        
        // Assert
        Assert.Equal(1, result.TotalPersons);
    }

    [Fact]
    public async Task GetRecentActivityAsync_ReturnsCorrectNumberOfActivities()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        
        // Add test user to context
        context.Users.Add(new ApplicationUser { Id = "user1", UserName = "testuser@test.com" });
        
        for (int i = 1; i <= 25; i++)
        {
            context.ActivityFeedItems.Add(new ActivityFeedItem
            {
                Id = i,
                UserId = "user1",
                ActivityType = "Test",
                EntityType = "TestEntity",
                Description = $"Activity {i}",
                IsPublic = true,
                CreatedDateTime = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        
        await context.SaveChangesAsync();
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetRecentActivityAsync(20);
        
        // Assert
        Assert.Equal(20, result.Count);
        // Verify activities are ordered by most recent first
        Assert.True(result[0].CreatedDateTime >= result[1].CreatedDateTime);
    }

    [Fact]
    public async Task GetRecentActivityAsync_IncludesUserNames()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        
        // Add test users to context
        context.Users.AddRange(
            new ApplicationUser { Id = "user1", UserName = "user1@test.com" },
            new ApplicationUser { Id = "user2", UserName = "user2@test.com" }
        );
        
        context.ActivityFeedItems.AddRange(
            new ActivityFeedItem
            {
                Id = 1,
                UserId = "user1",
                ActivityType = "PersonAdded",
                EntityType = "Person",
                Description = "Added a new person",
                IsPublic = true,
                CreatedDateTime = DateTime.UtcNow
            },
            new ActivityFeedItem
            {
                Id = 2,
                UserId = "user2",
                ActivityType = "StoryPublished",
                EntityType = "Story",
                Description = "Published a story",
                IsPublic = true,
                CreatedDateTime = DateTime.UtcNow.AddMinutes(-5)
            }
        );
        
        await context.SaveChangesAsync();
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetRecentActivityAsync(20);
        
        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("user1@test.com", result.First(a => a.UserId == "user1").UserName);
        Assert.Equal("user2@test.com", result.First(a => a.UserId == "user2").UserName);
    }

    [Fact]
    public async Task GetRecentActivityAsync_HandlesUserNotFound()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        
        // Add activity with user that doesn't exist
        context.ActivityFeedItems.Add(new ActivityFeedItem
        {
            Id = 1,
            UserId = "nonexistent-user",
            ActivityType = "Test",
            EntityType = "TestEntity",
            Description = "Test activity",
            IsPublic = true,
            CreatedDateTime = DateTime.UtcNow
        });
        
        await context.SaveChangesAsync();
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetRecentActivityAsync(20);
        
        // Assert
        Assert.Single(result);
        Assert.Equal(string.Empty, result[0].UserName); // Should handle missing user gracefully
    }

    [Fact]
    public async Task GetRecentActivityAsync_ReturnsEmptyList_WhenNoActivitiesExist()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetRecentActivityAsync(20);
        
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRecentActivityAsync_OrdersByMostRecentFirst()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        
        context.Users.Add(new ApplicationUser { Id = "user1", UserName = "testuser@test.com" });
        
        var now = DateTime.UtcNow;
        // Add items in reverse chronological order to help in-memory database
        context.ActivityFeedItems.Add(new ActivityFeedItem
        {
            Id = 2,
            UserId = "user1",
            ActivityType = "Recent",
            EntityType = "Test",
            Description = "Recent activity",
            IsPublic = true,
            CreatedDateTime = now
        });
        
        context.ActivityFeedItems.Add(new ActivityFeedItem
        {
            Id = 3,
            UserId = "user1",
            ActivityType = "Middle",
            EntityType = "Test",
            Description = "Middle activity",
            IsPublic = true,
            CreatedDateTime = now.AddHours(-1)
        });
        
        context.ActivityFeedItems.Add(new ActivityFeedItem
        {
            Id = 1,
            UserId = "user1",
            ActivityType = "Old",
            EntityType = "Test",
            Description = "Old activity",
            IsPublic = true,
            CreatedDateTime = now.AddHours(-2)
        });
        
        await context.SaveChangesAsync();
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetRecentActivityAsync(20);
        
        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Recent activity", result[0].Description);
        Assert.Equal("Middle activity", result[1].Description);
        Assert.Equal("Old activity", result[2].Description);
    }

    [Fact]
    public async Task GetRecentActivityAsync_RespectsCountParameter()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        
        context.Users.Add(new ApplicationUser { Id = "user1", UserName = "testuser@test.com" });
        
        for (int i = 1; i <= 10; i++)
        {
            context.ActivityFeedItems.Add(new ActivityFeedItem
            {
                Id = i,
                UserId = "user1",
                ActivityType = "Test",
                EntityType = "TestEntity",
                Description = $"Activity {i}",
                IsPublic = true,
                CreatedDateTime = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        
        await context.SaveChangesAsync();
        
        var service = new AdminDashboardService(context);
        
        // Act
        var result = await service.GetRecentActivityAsync(5);
        
        // Assert
        Assert.Equal(5, result.Count);
    }
}
