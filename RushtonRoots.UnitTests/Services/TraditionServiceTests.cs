using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class TraditionServiceTests
{
    private RushtonRootsDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new RushtonRootsDbContext(options);
        return context;
    }

    [Fact]
    public async Task GetRelatedRecipesAsync_ReturnsEmptyList_WhenTraditionNotFound()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        // Act
        var result = await service.GetRelatedRecipesAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRelatedRecipesAsync_ReturnsRecipesThatMentionTradition()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var tradition = new Tradition { Id = 1, Name = "Christmas", Description = "Yearly celebration", Category = "Holiday", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var recipe1 = new Recipe { Id = 1, Name = "Christmas Cookies", Ingredients = "Flour, Sugar", Instructions = "Mix and bake", Category = "Dessert", IsPublished = true, SubmittedByUserId = "user1" };
        var recipe2 = new Recipe { Id = 2, Name = "Easter Eggs", Ingredients = "Eggs, Dye", Instructions = "Boil and dye", Category = "Dessert", IsPublished = true, SubmittedByUserId = "user1" };
        var recipe3 = new Recipe { Id = 3, Name = "Holiday Cake", Ingredients = "Flour, Sugar", Instructions = "Bake", Notes = "Perfect for Christmas", Category = "Dessert", IsPublished = true, SubmittedByUserId = "user1" };
        context.Recipes.AddRange(recipe1, recipe2, recipe3);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedRecipesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Christmas Cookies (name) and Holiday Cake (notes)
        Assert.Contains(result, r => r.Name == "Christmas Cookies");
        Assert.Contains(result, r => r.Name == "Holiday Cake");
    }

    [Fact]
    public async Task GetRelatedRecipesAsync_ExcludesUnpublishedRecipes()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var tradition = new Tradition { Id = 1, Name = "Christmas", Description = "Yearly celebration", Category = "Holiday", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var recipe1 = new Recipe { Id = 1, Name = "Christmas Cookies", Ingredients = "Flour", Instructions = "Bake", Category = "Dessert", IsPublished = true, SubmittedByUserId = "user1" };
        var recipe2 = new Recipe { Id = 2, Name = "Christmas Cake", Ingredients = "Flour", Instructions = "Bake", Category = "Dessert", IsPublished = false, SubmittedByUserId = "user1" };
        context.Recipes.AddRange(recipe1, recipe2);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedRecipesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result); // Only published recipe
        Assert.Equal("Christmas Cookies", result[0].Name);
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_ReturnsEmptyList_WhenTraditionNotFound()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        // Act
        var result = await service.GetRelatedStoriesAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_ReturnsStoriesThatMentionTradition()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var tradition = new Tradition { Id = 1, Name = "Thanksgiving", Description = "Yearly celebration", Category = "Holiday", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var story1 = new Story { Id = 1, Title = "Thanksgiving Memories", Content = "Great times", Category = "Holiday", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 10 };
        var story2 = new Story { Id = 2, Title = "Summer Vacation", Content = "Beach fun", Category = "Vacation", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 5 };
        var story3 = new Story { Id = 3, Title = "Holiday Story", Content = "We celebrated Thanksgiving with family", Category = "Holiday", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 3 };
        context.Stories.AddRange(story1, story2, story3);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedStoriesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Thanksgiving Memories (title) and Holiday Story (content)
        Assert.Contains(result, s => s.Title == "Thanksgiving Memories");
        Assert.Contains(result, s => s.Title == "Holiday Story");
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_IncludesStoriesAboutStartedByPerson()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", IsDeleted = false };
        context.People.Add(person);

        var tradition = new Tradition { Id = 1, Name = "Family BBQ", Description = "Summer tradition", Category = "Food", StartedByPersonId = 1, IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var story1 = new Story { Id = 1, Title = "Summer Story", Content = "Great times", Category = "Summer", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 10 };
        var story2 = new Story { Id = 2, Title = "Winter Story", Content = "Cold days", Category = "Winter", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 5 };
        context.Stories.AddRange(story1, story2);

        var sp1 = new StoryPerson { StoryId = 1, PersonId = 1 }; // Story 1 is about John Doe
        context.StoryPeople.Add(sp1);

        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedStoriesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Summer Story", result[0].Title);
    }

    [Fact]
    public async Task GetPastOccurrencesAsync_ReturnsEmptyList_WhenNoOccurrences()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var tradition = new Tradition { Id = 1, Name = "Test", Description = "Test", Category = "Test", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPastOccurrencesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPastOccurrencesAsync_ReturnsPastOccurrencesInDescendingOrder()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var tradition = new Tradition { Id = 1, Name = "Annual Reunion", Description = "Family reunion", Category = "Event", Frequency = "Yearly", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var now = DateTime.UtcNow;
        var occurrence1 = new TraditionTimeline { Id = 1, TraditionId = 1, EventDate = now.AddDays(-365), Title = "2023 Reunion", Description = "Last year", EventType = "Celebration", RecordedByUserId = "user1" };
        var occurrence2 = new TraditionTimeline { Id = 2, TraditionId = 1, EventDate = now.AddDays(-730), Title = "2022 Reunion", Description = "Two years ago", EventType = "Celebration", RecordedByUserId = "user1" };
        var occurrence3 = new TraditionTimeline { Id = 3, TraditionId = 1, EventDate = now.AddDays(30), Title = "2025 Reunion", Description = "Next month", EventType = "Planned", RecordedByUserId = "user1" };
        context.TraditionTimelines.AddRange(occurrence1, occurrence2, occurrence3);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPastOccurrencesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Only past occurrences
        Assert.Equal("2023 Reunion", result[0].Title); // Most recent first
        Assert.Equal("2022 Reunion", result[1].Title);
    }

    [Fact]
    public async Task GetPastOccurrencesAsync_RespectsCountParameter()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var tradition = new Tradition { Id = 1, Name = "Monthly Meeting", Description = "Regular meeting", Category = "Event", Frequency = "Monthly", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var now = DateTime.UtcNow;
        for (int i = 1; i <= 10; i++)
        {
            var occurrence = new TraditionTimeline
            {
                Id = i,
                TraditionId = 1,
                EventDate = now.AddDays(-30 * i),
                Title = $"Meeting {i}",
                Description = $"Past meeting {i}",
                EventType = "Meeting",
                RecordedByUserId = "user1"
            };
            context.TraditionTimelines.Add(occurrence);
        }
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPastOccurrencesAsync(1, count: 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetNextOccurrenceAsync_ReturnsNull_WhenTraditionNotFound()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        // Act
        var result = await service.GetNextOccurrenceAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNextOccurrenceAsync_ReturnsFutureOccurrence_WhenExists()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var tradition = new Tradition { Id = 1, Name = "Summer Party", Description = "Annual party", Category = "Event", Frequency = "Yearly", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var now = DateTime.UtcNow;
        var futureOccurrence = new TraditionTimeline
        {
            Id = 1,
            TraditionId = 1,
            EventDate = now.AddDays(30),
            Title = "Next Party",
            Description = "Coming up",
            EventType = "Planned",
            RecordedByUserId = "user1"
        };
        context.TraditionTimelines.Add(futureOccurrence);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetNextOccurrenceAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Next Party", result.Title);
        Assert.True(result.EventDate > now);
    }

    [Fact]
    public async Task GetNextOccurrenceAsync_CalculatesNextOccurrence_WhenNoFutureEntryExists()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var now = DateTime.UtcNow;
        var tradition = new Tradition
        {
            Id = 1,
            Name = "Birthday Party",
            Description = "Annual celebration",
            Category = "Event",
            Frequency = "Yearly",
            StartedDate = now.AddYears(-2), // Started 2 years ago
            IsPublished = true,
            SubmittedByUserId = "user1"
        };
        context.Traditions.Add(tradition);

        var pastOccurrence = new TraditionTimeline
        {
            Id = 1,
            TraditionId = 1,
            EventDate = now.AddYears(-1), // Last year
            Title = "Last Party",
            Description = "Past party",
            EventType = "Celebration",
            RecordedByUserId = "user1"
        };
        context.TraditionTimelines.Add(pastOccurrence);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetNextOccurrenceAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Next Birthday Party", result.Title);
        Assert.True(result.EventDate > now);
        Assert.Equal("Calculated", result.EventType);
    }

    [Fact]
    public async Task GetNextOccurrenceAsync_HandlesMonthlyFrequency()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var now = DateTime.UtcNow;
        var tradition = new Tradition
        {
            Id = 1,
            Name = "Monthly Dinner",
            Description = "Family dinner",
            Category = "Event",
            Frequency = "Monthly",
            StartedDate = now.AddMonths(-3),
            IsPublished = true,
            SubmittedByUserId = "user1"
        };
        context.Traditions.Add(tradition);

        var pastOccurrence = new TraditionTimeline
        {
            Id = 1,
            TraditionId = 1,
            EventDate = now.AddMonths(-1),
            Title = "Last Dinner",
            Description = "Past dinner",
            EventType = "Celebration",
            RecordedByUserId = "user1"
        };
        context.TraditionTimelines.Add(pastOccurrence);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetNextOccurrenceAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.EventDate > now);
        // Should be approximately 1 month from now (allowing for some variance due to calculation)
        Assert.True((result.EventDate - now).TotalDays < 45); // Less than 45 days
    }

    [Fact]
    public async Task GetNextOccurrenceAsync_SelectsNearestFutureOccurrence_WhenMultipleExist()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<ITraditionRepository>();
        var service = new TraditionService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var tradition = new Tradition { Id = 1, Name = "Event", Description = "Test", Category = "Event", Frequency = "Monthly", IsPublished = true, SubmittedByUserId = "user1" };
        context.Traditions.Add(tradition);

        var now = DateTime.UtcNow;
        var occurrence1 = new TraditionTimeline { Id = 1, TraditionId = 1, EventDate = now.AddDays(10), Title = "Soon", Description = "Coming soon", EventType = "Planned", RecordedByUserId = "user1" };
        var occurrence2 = new TraditionTimeline { Id = 2, TraditionId = 1, EventDate = now.AddDays(100), Title = "Later", Description = "Much later", EventType = "Planned", RecordedByUserId = "user1" };
        context.TraditionTimelines.AddRange(occurrence1, occurrence2);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetNextOccurrenceAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Soon", result.Title); // Should return the nearest future occurrence
    }
}
