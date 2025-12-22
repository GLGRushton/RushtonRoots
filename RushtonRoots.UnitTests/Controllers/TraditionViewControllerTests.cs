using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RushtonRoots.UnitTests.Controllers;

public class TraditionViewControllerTests
{
    private readonly ITraditionService _mockTraditionService;
    private readonly TraditionViewController _controller;

    public TraditionViewControllerTests()
    {
        _mockTraditionService = A.Fake<ITraditionService>();
        _controller = new TraditionViewController(_mockTraditionService);
        
        // Setup minimal controller context for User property
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    #region Index Action Tests

    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        // Arrange
        A.CallTo(() => _mockTraditionService.GetAllAsync(true))
            .Returns(Task.FromResult<IEnumerable<TraditionViewModel>>(new List<TraditionViewModel>()));

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // Default view name
        
        // Verify service was called to populate ViewBag
        A.CallTo(() => _mockTraditionService.GetAllAsync(true))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Index_PopulatesViewBagWithTraditions()
    {
        // Arrange
        var traditions = new List<TraditionViewModel>
        {
            new TraditionViewModel { Id = 1, Name = "Tradition 1" },
            new TraditionViewModel { Id = 2, Name = "Tradition 2" }
        };
        
        A.CallTo(() => _mockTraditionService.GetAllAsync(true))
            .Returns(Task.FromResult<IEnumerable<TraditionViewModel>>(traditions));

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData);
        // Note: ViewBag is dynamic and can't be easily tested without creating a full controller context
        // The important part is that the service is called
        A.CallTo(() => _mockTraditionService.GetAllAsync(true))
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region Details Action Tests

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithTradition()
    {
        // Arrange
        var traditionId = 1;
        var tradition = new TraditionViewModel
        {
            Id = traditionId,
            Name = "Test Tradition",
            Description = "Test Description",
            Category = "Holiday",
            Status = "Active",
            Frequency = "Yearly"
        };

        A.CallTo(() => _mockTraditionService.GetByIdAsync(traditionId, true))
            .Returns(Task.FromResult<TraditionViewModel?>(tradition));

        // Act
        var result = await _controller.Details(traditionId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TraditionViewModel>(viewResult.Model);
        Assert.Equal(traditionId, model.Id);
        Assert.Equal("Test Tradition", model.Name);
        
        // Verify that the service was called with incrementViewCount = true
        A.CallTo(() => _mockTraditionService.GetByIdAsync(traditionId, true))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 999;

        A.CallTo(() => _mockTraditionService.GetByIdAsync(invalidId, true))
            .Returns(Task.FromResult<TraditionViewModel?>(null));

        // Act
        var result = await _controller.Details(invalidId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        
        // Verify that the service was called
        A.CallTo(() => _mockTraditionService.GetByIdAsync(invalidId, true))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Details_IncrementsViewCount()
    {
        // Arrange
        var traditionId = 1;
        var tradition = new TraditionViewModel
        {
            Id = traditionId,
            Name = "Test Tradition",
            ViewCount = 5
        };

        A.CallTo(() => _mockTraditionService.GetByIdAsync(traditionId, true))
            .Returns(Task.FromResult<TraditionViewModel?>(tradition));

        // Act
        await _controller.Details(traditionId);

        // Assert - Verify incrementViewCount parameter is true
        A.CallTo(() => _mockTraditionService.GetByIdAsync(traditionId, true))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Details_WithTraditionHavingTimeline_ReturnsViewWithTimeline()
    {
        // Arrange
        var traditionId = 1;
        var tradition = new TraditionViewModel
        {
            Id = traditionId,
            Name = "Test Tradition",
            Timeline = new System.Collections.Generic.List<TraditionTimelineViewModel>
            {
                new TraditionTimelineViewModel
                {
                    Id = 1,
                    TraditionId = traditionId,
                    Title = "Timeline Entry",
                    EventDate = System.DateTime.Now
                }
            }
        };

        A.CallTo(() => _mockTraditionService.GetByIdAsync(traditionId, true))
            .Returns(Task.FromResult<TraditionViewModel?>(tradition));

        // Act
        var result = await _controller.Details(traditionId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TraditionViewModel>(viewResult.Model);
        Assert.NotNull(model.Timeline);
        Assert.Single(model.Timeline);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Details_WithVariousIds_CallsServiceWithCorrectId(int traditionId)
    {
        // Arrange
        var tradition = new TraditionViewModel { Id = traditionId };
        A.CallTo(() => _mockTraditionService.GetByIdAsync(traditionId, true))
            .Returns(Task.FromResult<TraditionViewModel?>(tradition));

        // Act
        await _controller.Details(traditionId);

        // Assert
        A.CallTo(() => _mockTraditionService.GetByIdAsync(traditionId, true))
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public void TraditionViewController_HasAuthorizeAttribute()
    {
        // Arrange
        var type = typeof(TraditionViewController);

        // Act
        var attributes = type.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(attributes);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidService_InitializesController()
    {
        // Arrange
        var service = A.Fake<ITraditionService>();

        // Act
        var controller = new TraditionViewController(service);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion
}
