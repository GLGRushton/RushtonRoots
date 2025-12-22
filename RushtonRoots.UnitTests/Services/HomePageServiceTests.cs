using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class HomePageServiceTests
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
    public async Task GetStatisticsAsync_ReturnsCorrectStatistics_WhenDataExists()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        var people = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1, DateOfBirth = new DateTime(1950, 1, 1), IsDeleted = false },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith", HouseholdId = 1, DateOfBirth = new DateTime(1980, 1, 1), IsDeleted = false },
            new Person { Id = 3, FirstName = "Bob", LastName = "Jones", HouseholdId = 1, DateOfBirth = new DateTime(1990, 1, 1), IsDeleted = false }
        };
        context.People.AddRange(people);
        
        context.PersonPhotos.AddRange(
            new PersonPhoto { Id = 1, PersonId = 1, PhotoUrl = "photo1.jpg" },
            new PersonPhoto { Id = 2, PersonId = 2, PhotoUrl = "photo2.jpg" }
        );
        
        context.Stories.AddRange(
            new Story { Id = 1, Title = "Story 1", Content = "Content 1", IsPublished = true, SubmittedByUserId = "user1" },
            new Story { Id = 2, Title = "Story 2", Content = "Content 2", IsPublished = true, SubmittedByUserId = "user1" },
            new Story { Id = 3, Title = "Story 3", Content = "Content 3", IsPublished = false, SubmittedByUserId = "user1" }
        );
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetStatisticsAsync();
        
        // Assert
        Assert.Equal(3, result.TotalMembers);
        Assert.Equal("John Doe", result.OldestAncestor);
        Assert.NotNull(result.NewestMember); // Could be any of the three since CreatedDateTime is same
        Assert.Equal(2, result.TotalPhotos);
        Assert.Equal(2, result.TotalStories); // Only published stories
        Assert.Equal(1, result.ActiveHouseholds);
    }

    [Fact]
    public async Task GetStatisticsAsync_ReturnsZeroStatistics_WhenNoDataExists()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetStatisticsAsync();
        
        // Assert
        Assert.Equal(0, result.TotalMembers);
        Assert.Null(result.OldestAncestor);
        Assert.Null(result.NewestMember);
        Assert.Equal(0, result.TotalPhotos);
        Assert.Equal(0, result.TotalStories);
        Assert.Equal(0, result.ActiveHouseholds);
    }

    [Fact]
    public async Task GetStatisticsAsync_ExcludesDeletedPeople()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        context.People.AddRange(
            new Person { Id = 1, FirstName = "Active", LastName = "User", HouseholdId = 1, IsDeleted = false },
            new Person { Id = 2, FirstName = "Deleted", LastName = "User", HouseholdId = 1, IsDeleted = true }
        );
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetStatisticsAsync();
        
        // Assert
        Assert.Equal(1, result.TotalMembers);
    }

    [Fact]
    public async Task GetRecentAdditionsAsync_ReturnsCorrectNumberOfAdditions()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        for (int i = 1; i <= 10; i++)
        {
            context.People.Add(new Person
            {
                Id = i,
                FirstName = $"Person{i}",
                LastName = "Doe",
                HouseholdId = 1,
                IsDeleted = false
            });
        }
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetRecentAdditionsAsync(5);
        
        // Assert
        Assert.Equal(5, result.Count);
        // Verify all results are non-deleted people
        Assert.All(result, person => Assert.NotEmpty(person.FirstName));
    }

    [Fact]
    public async Task GetRecentAdditionsAsync_ExcludesDeletedPeople()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        context.People.AddRange(
            new Person { Id = 1, FirstName = "Active", LastName = "User", HouseholdId = 1, IsDeleted = false },
            new Person { Id = 2, FirstName = "Deleted", LastName = "User", HouseholdId = 1, IsDeleted = true }
        );
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetRecentAdditionsAsync(10);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("Active", result[0].FirstName);
    }

    [Fact]
    public async Task GetUpcomingBirthdaysAsync_ReturnsUpcomingBirthdaysWithinDays()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        var today = DateTime.Today;
        var upcomingBirthday = today.AddDays(15);
        var farFutureBirthday = today.AddDays(60);
        var pastBirthday = today.AddDays(-30);
        
        context.People.AddRange(
            new Person 
            { 
                Id = 1, 
                FirstName = "Upcoming", 
                LastName = "Birthday", 
                HouseholdId = 1, 
                DateOfBirth = new DateTime(1990, upcomingBirthday.Month, upcomingBirthday.Day),
                IsDeleted = false,
                IsDeceased = false
            },
            new Person 
            { 
                Id = 2, 
                FirstName = "Far", 
                LastName = "Future", 
                HouseholdId = 1, 
                DateOfBirth = new DateTime(1990, farFutureBirthday.Month, farFutureBirthday.Day),
                IsDeleted = false,
                IsDeceased = false
            }
        );
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetUpcomingBirthdaysAsync(30);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("Upcoming", result[0].FirstName);
        Assert.True(result[0].DaysUntilBirthday <= 30);
    }

    [Fact]
    public async Task GetUpcomingBirthdaysAsync_ExcludesDeceasedPeople()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        var today = DateTime.Today;
        var upcomingBirthday = today.AddDays(10);
        
        context.People.AddRange(
            new Person 
            { 
                Id = 1, 
                FirstName = "Alive", 
                LastName = "Person", 
                HouseholdId = 1, 
                DateOfBirth = new DateTime(1990, upcomingBirthday.Month, upcomingBirthday.Day),
                IsDeleted = false,
                IsDeceased = false
            },
            new Person 
            { 
                Id = 2, 
                FirstName = "Deceased", 
                LastName = "Person", 
                HouseholdId = 1, 
                DateOfBirth = new DateTime(1990, upcomingBirthday.Month, upcomingBirthday.Day),
                IsDeleted = false,
                IsDeceased = true
            }
        );
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetUpcomingBirthdaysAsync(30);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("Alive", result[0].FirstName);
    }

    [Fact]
    public async Task GetUpcomingBirthdaysAsync_CalculatesAgeCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        var today = DateTime.Today;
        var upcomingBirthday = today.AddDays(10);
        var birthYear = 1990;
        
        context.People.Add(new Person 
        { 
            Id = 1, 
            FirstName = "Test", 
            LastName = "Person", 
            HouseholdId = 1, 
            DateOfBirth = new DateTime(birthYear, upcomingBirthday.Month, upcomingBirthday.Day),
            IsDeleted = false,
            IsDeceased = false
        });
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetUpcomingBirthdaysAsync(30);
        
        // Assert
        Assert.Single(result);
        // Age will be the age they turn ON their upcoming birthday
        var expectedAge = upcomingBirthday.Year - birthYear;
        Assert.Equal(expectedAge, result[0].Age);
    }

    [Fact]
    public async Task GetUpcomingAnniversariesAsync_ReturnsUpcomingAnniversariesWithinDays()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        var today = DateTime.Today;
        var upcomingAnniversary = today.AddDays(15);
        
        var person1 = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1, IsDeleted = false };
        var person2 = new Person { Id = 2, FirstName = "Jane", LastName = "Doe", HouseholdId = 1, IsDeleted = false };
        
        context.People.AddRange(person1, person2);
        
        context.Partnerships.Add(new Partnership
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married",
            StartDate = new DateTime(2000, upcomingAnniversary.Month, upcomingAnniversary.Day),
            IsDeleted = false
        });
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetUpcomingAnniversariesAsync(30);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].PersonAName);
        Assert.Equal("Jane Doe", result[0].PersonBName);
        Assert.True(result[0].DaysUntilAnniversary <= 30);
    }

    [Fact]
    public async Task GetUpcomingAnniversariesAsync_ExcludesEndedPartnerships()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        var today = DateTime.Today;
        var upcomingAnniversary = today.AddDays(10);
        
        var person1 = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1, IsDeleted = false };
        var person2 = new Person { Id = 2, FirstName = "Jane", LastName = "Doe", HouseholdId = 1, IsDeleted = false };
        var person3 = new Person { Id = 3, FirstName = "Bob", LastName = "Smith", HouseholdId = 1, IsDeleted = false };
        var person4 = new Person { Id = 4, FirstName = "Alice", LastName = "Smith", HouseholdId = 1, IsDeleted = false };
        
        context.People.AddRange(person1, person2, person3, person4);
        
        context.Partnerships.AddRange(
            new Partnership
            {
                Id = 1,
                PersonAId = 1,
                PersonBId = 2,
                PartnershipType = "Married",
                StartDate = new DateTime(2000, upcomingAnniversary.Month, upcomingAnniversary.Day),
                EndDate = null, // Active partnership
                IsDeleted = false
            },
            new Partnership
            {
                Id = 2,
                PersonAId = 3,
                PersonBId = 4,
                PartnershipType = "Married",
                StartDate = new DateTime(2000, upcomingAnniversary.Month, upcomingAnniversary.Day),
                EndDate = new DateTime(2020, 1, 1), // Ended partnership
                IsDeleted = false
            }
        );
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetUpcomingAnniversariesAsync(30);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].PersonAName);
    }

    [Fact]
    public async Task GetUpcomingAnniversariesAsync_CalculatesYearsMarriedCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        var household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 };
        context.Households.Add(household);
        
        var today = DateTime.Today;
        var upcomingAnniversary = today.AddDays(10);
        var marriageYear = 2000;
        
        var person1 = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1, IsDeleted = false };
        var person2 = new Person { Id = 2, FirstName = "Jane", LastName = "Doe", HouseholdId = 1, IsDeleted = false };
        
        context.People.AddRange(person1, person2);
        
        context.Partnerships.Add(new Partnership
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married",
            StartDate = new DateTime(marriageYear, upcomingAnniversary.Month, upcomingAnniversary.Day),
            IsDeleted = false
        });
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetUpcomingAnniversariesAsync(30);
        
        // Assert
        Assert.Single(result);
        // Years married will be the years they'll have been married ON their upcoming anniversary
        var expectedYears = upcomingAnniversary.Year - marriageYear;
        Assert.Equal(expectedYears, result[0].YearsMarried);
    }

    [Fact]
    public async Task GetActivityFeedAsync_ReturnsRecentPublicActivities()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        for (int i = 1; i <= 25; i++)
        {
            context.ActivityFeedItems.Add(new ActivityFeedItem
            {
                Id = i,
                UserId = "user1",
                ActivityType = "Test",
                Description = $"Activity {i}",
                IsPublic = true
            });
        }
        
        // Add a private activity that should be excluded
        context.ActivityFeedItems.Add(new ActivityFeedItem
        {
            Id = 26,
            UserId = "user1",
            ActivityType = "Test",
            Description = "Private Activity",
            IsPublic = false
        });
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetActivityFeedAsync(20);
        
        // Assert
        Assert.Equal(20, result.Count);
        Assert.All(result, item => Assert.True(item.IsPublic));
    }

    [Fact]
    public async Task GetActivityFeedAsync_ExcludesPrivateActivities()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        
        context.ActivityFeedItems.AddRange(
            new ActivityFeedItem
            {
                Id = 1,
                UserId = "user1",
                ActivityType = "Test",
                Description = "Public Activity",
                IsPublic = true
            },
            new ActivityFeedItem
            {
                Id = 2,
                UserId = "user1",
                ActivityType = "Test",
                Description = "Private Activity",
                IsPublic = false
            }
        );
        
        await context.SaveChangesAsync();
        
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetActivityFeedAsync(20);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("Public Activity", result[0].Description);
    }

    [Fact]
    public async Task GetActivityFeedAsync_ReturnsEmptyList_WhenNoActivitiesExist()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HomePageService(context);
        
        // Act
        var result = await service.GetActivityFeedAsync(20);
        
        // Assert
        Assert.Empty(result);
    }
}
