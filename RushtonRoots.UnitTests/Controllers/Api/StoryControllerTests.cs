using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Web.Controllers.Api;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers.Api;

public class StoryControllerTests
{
    private readonly IStoryService _mockStoryService;
    private readonly StoryController _controller;

    public StoryControllerTests()
    {
        _mockStoryService = A.Fake<IStoryService>();
        _controller = new StoryController(_mockStoryService);
    }

    #region GetComments Tests

    [Fact]
    public async Task GetComments_WithValidId_ReturnsOkWithComments()
    {
        // Arrange
        var storyId = 1;
        var comments = new List<StoryComment>
        {
            new StoryComment 
            { 
                Id = 1, 
                UserName = "John Doe", 
                Content = "Great story!", 
                CreatedDateTime = DateTime.UtcNow.AddDays(-2) 
            },
            new StoryComment 
            { 
                Id = 2, 
                UserName = "Jane Smith", 
                Content = "Very touching.", 
                CreatedDateTime = DateTime.UtcNow.AddDays(-1) 
            }
        };

        A.CallTo(() => _mockStoryService.GetStoryCommentsAsync(storyId)).Returns(comments);

        // Act
        var result = await _controller.GetComments(storyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedComments = Assert.IsAssignableFrom<List<StoryComment>>(okResult.Value);
        Assert.Equal(2, returnedComments.Count);
        Assert.Equal("John Doe", returnedComments[0].UserName);
    }

    [Fact]
    public async Task GetComments_WithValidId_ReturnsEmptyListWhenNoComments()
    {
        // Arrange
        var storyId = 1;
        var comments = new List<StoryComment>();

        A.CallTo(() => _mockStoryService.GetStoryCommentsAsync(storyId)).Returns(comments);

        // Act
        var result = await _controller.GetComments(storyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedComments = Assert.IsAssignableFrom<List<StoryComment>>(okResult.Value);
        Assert.Empty(returnedComments);
    }

    [Fact]
    public async Task GetComments_WhenServiceThrows_ReturnsNotFound()
    {
        // Arrange
        var storyId = 999;
        A.CallTo(() => _mockStoryService.GetStoryCommentsAsync(storyId)).Throws<Exception>();

        // Act
        var result = await _controller.GetComments(storyId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion

    #region GetRelatedStories Tests

    [Fact]
    public async Task GetRelatedStories_WithValidId_ReturnsOkWithStories()
    {
        // Arrange
        var storyId = 1;
        var relatedStories = new List<StoryViewModel>
        {
            new StoryViewModel 
            { 
                Id = 2, 
                Title = "Related Story 1", 
                Slug = "related-story-1",
                Category = "Family History" 
            },
            new StoryViewModel 
            { 
                Id = 3, 
                Title = "Related Story 2", 
                Slug = "related-story-2",
                Category = "Family History" 
            }
        };

        A.CallTo(() => _mockStoryService.GetRelatedStoriesAsync(storyId, 5)).Returns(relatedStories);

        // Act
        var result = await _controller.GetRelatedStories(storyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStories = Assert.IsAssignableFrom<List<StoryViewModel>>(okResult.Value);
        Assert.Equal(2, returnedStories.Count);
        Assert.Equal("Related Story 1", returnedStories[0].Title);
    }

    [Fact]
    public async Task GetRelatedStories_WithCustomCount_ReturnsCorrectNumber()
    {
        // Arrange
        var storyId = 1;
        var count = 3;
        var relatedStories = new List<StoryViewModel>
        {
            new StoryViewModel { Id = 2, Title = "Story 1", Slug = "story-1" },
            new StoryViewModel { Id = 3, Title = "Story 2", Slug = "story-2" },
            new StoryViewModel { Id = 4, Title = "Story 3", Slug = "story-3" }
        };

        A.CallTo(() => _mockStoryService.GetRelatedStoriesAsync(storyId, count)).Returns(relatedStories);

        // Act
        var result = await _controller.GetRelatedStories(storyId, count);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStories = Assert.IsAssignableFrom<List<StoryViewModel>>(okResult.Value);
        Assert.Equal(3, returnedStories.Count);
    }

    [Fact]
    public async Task GetRelatedStories_WithValidId_ReturnsEmptyListWhenNoRelatedStories()
    {
        // Arrange
        var storyId = 1;
        var relatedStories = new List<StoryViewModel>();

        A.CallTo(() => _mockStoryService.GetRelatedStoriesAsync(storyId, 5)).Returns(relatedStories);

        // Act
        var result = await _controller.GetRelatedStories(storyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStories = Assert.IsAssignableFrom<List<StoryViewModel>>(okResult.Value);
        Assert.Empty(returnedStories);
    }

    [Fact]
    public async Task GetRelatedStories_WhenServiceThrows_ReturnsNotFound()
    {
        // Arrange
        var storyId = 999;
        A.CallTo(() => _mockStoryService.GetRelatedStoriesAsync(storyId, 5)).Throws<Exception>();

        // Act
        var result = await _controller.GetRelatedStories(storyId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion
}
