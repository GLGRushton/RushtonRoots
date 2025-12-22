using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class StoryServiceTests
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
    public async Task GetStoryCommentsAsync_ReturnsEmptyList_WhenNoCommentsExist()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var story = new Story { Id = 1, Title = "Test Story", Content = "Content", IsPublished = true, SubmittedByUserId = "user1" };
        context.Stories.Add(story);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetStoryCommentsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStoryCommentsAsync_ReturnsComments_WhenCommentsExist()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var story = new Story { Id = 1, Title = "Test Story", Content = "Content", IsPublished = true, SubmittedByUserId = "user1" };
        context.Stories.Add(story);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var comment1 = new Comment
        {
            Id = 1,
            Content = "Great story!",
            UserId = "user1",
            EntityType = "Story",
            EntityId = 1,
            ParentCommentId = null
        };
        var comment2 = new Comment
        {
            Id = 2,
            Content = "I agree!",
            UserId = "user1",
            EntityType = "Story",
            EntityId = 1,
            ParentCommentId = 1 // Reply to comment1
        };
        context.Comments.AddRange(comment1, comment2);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetStoryCommentsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result); // Only one top-level comment
        Assert.Equal("Great story!", result[0].Content);
        Assert.Single(result[0].Replies); // One reply
        Assert.Equal("I agree!", result[0].Replies[0].Content);
    }

    [Fact]
    public async Task GetStoryCommentsAsync_ExcludesCommentsFromOtherStories()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var story1 = new Story { Id = 1, Title = "Story 1", Content = "Content", IsPublished = true, SubmittedByUserId = "user1" };
        var story2 = new Story { Id = 2, Title = "Story 2", Content = "Content", IsPublished = true, SubmittedByUserId = "user1" };
        context.Stories.AddRange(story1, story2);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var comment1 = new Comment { Id = 1, Content = "Comment on Story 1", UserId = "user1", EntityType = "Story", EntityId = 1 };
        var comment2 = new Comment { Id = 2, Content = "Comment on Story 2", UserId = "user1", EntityType = "Story", EntityId = 2 };
        context.Comments.AddRange(comment1, comment2);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetStoryCommentsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Comment on Story 1", result[0].Content);
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_ReturnsEmptyList_WhenStoryNotFound()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        // Act
        var result = await service.GetRelatedStoriesAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_ReturnsStoriesInSameCategory()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var story1 = new Story { Id = 1, Title = "Story 1", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 10 };
        var story2 = new Story { Id = 2, Title = "Story 2", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 5 };
        var story3 = new Story { Id = 3, Title = "Story 3", Content = "Content", Category = "War Stories", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 20 };
        context.Stories.AddRange(story1, story2, story3);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedStoriesAsync(1, count: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result); // Only Story 2 is related (same category, excluding Story 1 itself)
        Assert.Equal("Story 2", result[0].Title);
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_PrioritizesStoriesWithSharedPeople()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var person1 = new Person { Id = 1, FirstName = "John", LastName = "Doe", IsDeleted = false };
        var person2 = new Person { Id = 2, FirstName = "Jane", LastName = "Smith", IsDeleted = false };
        context.People.AddRange(person1, person2);

        var story1 = new Story { Id = 1, Title = "Story 1", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 10 };
        var story2 = new Story { Id = 2, Title = "Story 2", Content = "Content", Category = "War Stories", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 5 };
        var story3 = new Story { Id = 3, Title = "Story 3", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 3 };
        context.Stories.AddRange(story1, story2, story3);

        // Story 1 and Story 2 share person1
        var sp1 = new StoryPerson { StoryId = 1, PersonId = 1 };
        var sp2 = new StoryPerson { StoryId = 2, PersonId = 1 };
        context.StoryPeople.AddRange(sp1, sp2);

        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedStoriesAsync(1, count: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        // Story 2 should be first (shared person) despite different category
        Assert.Equal("Story 2", result[0].Title);
        // Story 3 should be second (same category but no shared people)
        Assert.Equal("Story 3", result[1].Title);
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_PrioritizesStoriesInSameCollection()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var collection = new StoryCollection { Id = 1, Name = "Family Memories", CreatedByUserId = "user1", IsPublished = true };
        context.StoryCollections.Add(collection);

        var story1 = new Story { Id = 1, Title = "Story 1", Content = "Content", Category = "Childhood", CollectionId = 1, IsPublished = true, SubmittedByUserId = "user1", ViewCount = 10 };
        var story2 = new Story { Id = 2, Title = "Story 2", Content = "Content", Category = "War Stories", CollectionId = 1, IsPublished = true, SubmittedByUserId = "user1", ViewCount = 5 };
        var story3 = new Story { Id = 3, Title = "Story 3", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 20 };
        context.Stories.AddRange(story1, story2, story3);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedStoriesAsync(1, count: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        // Story 2 should be first (same collection) despite different category
        Assert.Equal("Story 2", result[0].Title);
        // Story 3 should be second (same category but different collection)
        Assert.Equal("Story 3", result[1].Title);
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_ExcludesUnpublishedStories()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var story1 = new Story { Id = 1, Title = "Story 1", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1" };
        var story2 = new Story { Id = 2, Title = "Story 2", Content = "Content", Category = "Childhood", IsPublished = false, SubmittedByUserId = "user1" };
        context.Stories.AddRange(story1, story2);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedStoriesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result); // Story 2 is unpublished so should not be included
    }

    [Fact]
    public async Task GetRelatedStoriesAsync_RespectsCountParameter()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var mockRepo = A.Fake<IStoryRepository>();
        var service = new StoryService(mockRepo, context);

        var user = new ApplicationUser { Id = "user1", UserName = "TestUser", Email = "test@example.com" };
        context.Users.Add(user);

        var story1 = new Story { Id = 1, Title = "Story 1", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1" };
        var story2 = new Story { Id = 2, Title = "Story 2", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 10 };
        var story3 = new Story { Id = 3, Title = "Story 3", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 5 };
        var story4 = new Story { Id = 4, Title = "Story 4", Content = "Content", Category = "Childhood", IsPublished = true, SubmittedByUserId = "user1", ViewCount = 3 };
        context.Stories.AddRange(story1, story2, story3, story4);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetRelatedStoriesAsync(1, count: 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Should return only 2 stories
    }
}
