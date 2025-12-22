using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Web.Controllers.Api;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers.Api;

public class TraditionControllerTests
{
    private readonly ITraditionService _mockTraditionService;
    private readonly TraditionController _controller;

    public TraditionControllerTests()
    {
        _mockTraditionService = A.Fake<ITraditionService>();
        _controller = new TraditionController(_mockTraditionService);
    }

    #region GetRelatedRecipes Tests

    [Fact]
    public async Task GetRelatedRecipes_WithValidId_ReturnsOkWithRecipes()
    {
        // Arrange
        var traditionId = 1;
        var recipes = new List<RecipeViewModel>
        {
            new RecipeViewModel 
            { 
                Id = 1, 
                Name = "Grandma's Apple Pie", 
                Slug = "grandmas-apple-pie",
                PrepTimeMinutes = 30 
            },
            new RecipeViewModel 
            { 
                Id = 2, 
                Name = "Holiday Cookies", 
                Slug = "holiday-cookies",
                PrepTimeMinutes = 45 
            }
        };

        A.CallTo(() => _mockTraditionService.GetRelatedRecipesAsync(traditionId)).Returns(recipes);

        // Act
        var result = await _controller.GetRelatedRecipes(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRecipes = Assert.IsAssignableFrom<List<RecipeViewModel>>(okResult.Value);
        Assert.Equal(2, returnedRecipes.Count);
        Assert.Equal("Grandma's Apple Pie", returnedRecipes[0].Name);
    }

    [Fact]
    public async Task GetRelatedRecipes_WithValidId_ReturnsEmptyListWhenNoRecipes()
    {
        // Arrange
        var traditionId = 1;
        var recipes = new List<RecipeViewModel>();

        A.CallTo(() => _mockTraditionService.GetRelatedRecipesAsync(traditionId)).Returns(recipes);

        // Act
        var result = await _controller.GetRelatedRecipes(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRecipes = Assert.IsAssignableFrom<List<RecipeViewModel>>(okResult.Value);
        Assert.Empty(returnedRecipes);
    }

    [Fact]
    public async Task GetRelatedRecipes_WhenServiceThrows_ReturnsNotFound()
    {
        // Arrange
        var traditionId = 999;
        A.CallTo(() => _mockTraditionService.GetRelatedRecipesAsync(traditionId)).Throws<Exception>();

        // Act
        var result = await _controller.GetRelatedRecipes(traditionId);

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
        var traditionId = 1;
        var stories = new List<StoryViewModel>
        {
            new StoryViewModel 
            { 
                Id = 1, 
                Title = "Thanksgiving Stories", 
                Slug = "thanksgiving-stories",
                Category = "Traditions" 
            },
            new StoryViewModel 
            { 
                Id = 2, 
                Title = "Holiday Memories", 
                Slug = "holiday-memories",
                Category = "Traditions" 
            }
        };

        A.CallTo(() => _mockTraditionService.GetRelatedStoriesAsync(traditionId)).Returns(stories);

        // Act
        var result = await _controller.GetRelatedStories(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStories = Assert.IsAssignableFrom<List<StoryViewModel>>(okResult.Value);
        Assert.Equal(2, returnedStories.Count);
        Assert.Equal("Thanksgiving Stories", returnedStories[0].Title);
    }

    [Fact]
    public async Task GetRelatedStories_WithValidId_ReturnsEmptyListWhenNoStories()
    {
        // Arrange
        var traditionId = 1;
        var stories = new List<StoryViewModel>();

        A.CallTo(() => _mockTraditionService.GetRelatedStoriesAsync(traditionId)).Returns(stories);

        // Act
        var result = await _controller.GetRelatedStories(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStories = Assert.IsAssignableFrom<List<StoryViewModel>>(okResult.Value);
        Assert.Empty(returnedStories);
    }

    [Fact]
    public async Task GetRelatedStories_WhenServiceThrows_ReturnsNotFound()
    {
        // Arrange
        var traditionId = 999;
        A.CallTo(() => _mockTraditionService.GetRelatedStoriesAsync(traditionId)).Throws<Exception>();

        // Act
        var result = await _controller.GetRelatedStories(traditionId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion

    #region GetPastOccurrences Tests

    [Fact]
    public async Task GetPastOccurrences_WithValidId_ReturnsOkWithOccurrences()
    {
        // Arrange
        var traditionId = 1;
        var occurrences = new List<TraditionOccurrence>
        {
            new TraditionOccurrence 
            { 
                EventDate = DateTime.UtcNow.AddYears(-1), 
                Description = "Last year's celebration",
                Title = "Annual Celebration"
            },
            new TraditionOccurrence 
            { 
                EventDate = DateTime.UtcNow.AddYears(-2), 
                Description = "Two years ago",
                Title = "Past Celebration"
            }
        };

        A.CallTo(() => _mockTraditionService.GetPastOccurrencesAsync(traditionId, 5)).Returns(occurrences);

        // Act
        var result = await _controller.GetPastOccurrences(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOccurrences = Assert.IsAssignableFrom<List<TraditionOccurrence>>(okResult.Value);
        Assert.Equal(2, returnedOccurrences.Count);
        Assert.Equal("Last year's celebration", returnedOccurrences[0].Description);
    }

    [Fact]
    public async Task GetPastOccurrences_WithCustomCount_ReturnsCorrectNumber()
    {
        // Arrange
        var traditionId = 1;
        var count = 3;
        var occurrences = new List<TraditionOccurrence>
        {
            new TraditionOccurrence { EventDate = DateTime.UtcNow.AddYears(-1), Description = "Last year" },
            new TraditionOccurrence { EventDate = DateTime.UtcNow.AddYears(-2), Description = "2 years ago" },
            new TraditionOccurrence { EventDate = DateTime.UtcNow.AddYears(-3), Description = "3 years ago" }
        };

        A.CallTo(() => _mockTraditionService.GetPastOccurrencesAsync(traditionId, count)).Returns(occurrences);

        // Act
        var result = await _controller.GetPastOccurrences(traditionId, count);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOccurrences = Assert.IsAssignableFrom<List<TraditionOccurrence>>(okResult.Value);
        Assert.Equal(3, returnedOccurrences.Count);
    }

    [Fact]
    public async Task GetPastOccurrences_WithValidId_ReturnsEmptyListWhenNoOccurrences()
    {
        // Arrange
        var traditionId = 1;
        var occurrences = new List<TraditionOccurrence>();

        A.CallTo(() => _mockTraditionService.GetPastOccurrencesAsync(traditionId, 5)).Returns(occurrences);

        // Act
        var result = await _controller.GetPastOccurrences(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOccurrences = Assert.IsAssignableFrom<List<TraditionOccurrence>>(okResult.Value);
        Assert.Empty(returnedOccurrences);
    }

    [Fact]
    public async Task GetPastOccurrences_WhenServiceThrows_ReturnsNotFound()
    {
        // Arrange
        var traditionId = 999;
        A.CallTo(() => _mockTraditionService.GetPastOccurrencesAsync(traditionId, 5)).Throws<Exception>();

        // Act
        var result = await _controller.GetPastOccurrences(traditionId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion

    #region GetNextOccurrence Tests

    [Fact]
    public async Task GetNextOccurrence_WithValidId_ReturnsOkWithOccurrence()
    {
        // Arrange
        var traditionId = 1;
        var occurrence = new TraditionOccurrence 
        { 
            EventDate = DateTime.UtcNow.AddMonths(1), 
            Description = "Next month's celebration",
            Title = "Upcoming Celebration"
        };

        A.CallTo(() => _mockTraditionService.GetNextOccurrenceAsync(traditionId)).Returns(occurrence);

        // Act
        var result = await _controller.GetNextOccurrence(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOccurrence = Assert.IsType<TraditionOccurrence>(okResult.Value);
        Assert.Equal("Next month's celebration", returnedOccurrence.Description);
    }

    [Fact]
    public async Task GetNextOccurrence_WhenNoFutureOccurrence_ReturnsOkWithMessage()
    {
        // Arrange
        var traditionId = 1;
        TraditionOccurrence? occurrence = null;

        A.CallTo(() => _mockTraditionService.GetNextOccurrenceAsync(traditionId)).Returns(occurrence);

        // Act
        var result = await _controller.GetNextOccurrence(traditionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetNextOccurrence_WhenServiceThrows_ReturnsNotFound()
    {
        // Arrange
        var traditionId = 999;
        A.CallTo(() => _mockTraditionService.GetNextOccurrenceAsync(traditionId)).Throws<Exception>();

        // Act
        var result = await _controller.GetNextOccurrence(traditionId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion
}
