using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;
using System;

namespace RushtonRoots.UnitTests.Controllers;

public class CalendarControllerTests
{
    private readonly ILogger<CalendarController> _mockLogger;
    private readonly CalendarController _controller;

    public CalendarControllerTests()
    {
        _mockLogger = A.Fake<ILogger<CalendarController>>();
        _controller = new CalendarController(_mockLogger);
    }

    #region Index Action Tests

    [Fact]
    public void Index_WithNoParameters_ReturnsViewWithDefaults()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Family Calendar", viewResult.ViewData["Title"]);
        Assert.Equal("dayGridMonth", viewResult.ViewData["ViewMode"]);
        Assert.NotNull(viewResult.ViewData["InitialDate"]);
        Assert.False((bool)(viewResult.ViewData["CanEdit"] ?? true)); // Default should be false when no user roles
    }

    [Fact]
    public void Index_WithMonthView_ReturnsViewWithMonthViewMode()
    {
        // Act
        var result = _controller.Index(view: "dayGridMonth");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("dayGridMonth", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithWeekView_ReturnsViewWithWeekViewMode()
    {
        // Act
        var result = _controller.Index(view: "timeGridWeek");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("timeGridWeek", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithDayView_ReturnsViewWithDayViewMode()
    {
        // Act
        var result = _controller.Index(view: "timeGridDay");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("timeGridDay", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithListView_ReturnsViewWithListViewMode()
    {
        // Act
        var result = _controller.Index(view: "listWeek");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("listWeek", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithNullView_UsesDefaultMonthView()
    {
        // Act
        var result = _controller.Index(view: null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("dayGridMonth", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithSpecificDate_ReturnsViewWithThatDate()
    {
        // Arrange
        var testDate = new DateTime(2024, 12, 25);

        // Act
        var result = _controller.Index(date: testDate);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("2024-12-25", viewResult.ViewData["InitialDate"]);
    }

    [Fact]
    public void Index_WithNullDate_UsesTodayDate()
    {
        // Act
        var result = _controller.Index(date: null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var initialDate = viewResult.ViewData["InitialDate"]?.ToString();
        Assert.NotNull(initialDate);
        Assert.Equal(DateTime.Now.ToString("yyyy-MM-dd"), initialDate);
    }

    [Fact]
    public void Index_WithAllParameters_ReturnsViewWithAllSettings()
    {
        // Arrange
        var testDate = new DateTime(2024, 6, 15);

        // Act
        var result = _controller.Index(view: "timeGridWeek", date: testDate);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Family Calendar", viewResult.ViewData["Title"]);
        Assert.Equal("timeGridWeek", viewResult.ViewData["ViewMode"]);
        Assert.Equal("2024-06-15", viewResult.ViewData["InitialDate"]);
    }

    [Theory]
    [InlineData("dayGridMonth")]
    [InlineData("timeGridWeek")]
    [InlineData("timeGridDay")]
    [InlineData("listWeek")]
    public void Index_WithVariousViewModes_ReturnsViewWithCorrectMode(string viewMode)
    {
        // Act
        var result = _controller.Index(view: viewMode);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(viewMode, viewResult.ViewData["ViewMode"]);
    }

    #endregion

    #region Create Action Tests

    [Fact]
    public void Create_WithNoParameters_ReturnsViewWithDefaults()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Create Event", viewResult.ViewData["Title"]);
        Assert.NotNull(viewResult.ViewData["EventDate"]);
        Assert.Equal("", viewResult.ViewData["StartTime"]);
        Assert.Equal("", viewResult.ViewData["EndTime"]);
    }

    [Fact]
    public void Create_WithSpecificDate_ReturnsViewWithThatDate()
    {
        // Arrange
        var testDate = new DateTime(2024, 12, 25);

        // Act
        var result = _controller.Create(date: testDate);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("2024-12-25", viewResult.ViewData["EventDate"]);
    }

    [Fact]
    public void Create_WithNullDate_UsesTodayDate()
    {
        // Act
        var result = _controller.Create(date: null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var eventDate = viewResult.ViewData["EventDate"]?.ToString();
        Assert.NotNull(eventDate);
        Assert.Equal(DateTime.Now.ToString("yyyy-MM-dd"), eventDate);
    }

    [Fact]
    public void Create_WithStartTime_ReturnsViewWithStartTime()
    {
        // Arrange
        var startTime = new DateTime(2024, 12, 25, 14, 30, 0);

        // Act
        var result = _controller.Create(startTime: startTime);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("2024-12-25T14:30", viewResult.ViewData["StartTime"]);
    }

    [Fact]
    public void Create_WithEndTime_ReturnsViewWithEndTime()
    {
        // Arrange
        var endTime = new DateTime(2024, 12, 25, 16, 0, 0);

        // Act
        var result = _controller.Create(endTime: endTime);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("2024-12-25T16:00", viewResult.ViewData["EndTime"]);
    }

    [Fact]
    public void Create_WithAllParameters_ReturnsViewWithAllSettings()
    {
        // Arrange
        var testDate = new DateTime(2024, 7, 4);
        var startTime = new DateTime(2024, 7, 4, 10, 0, 0);
        var endTime = new DateTime(2024, 7, 4, 12, 0, 0);

        // Act
        var result = _controller.Create(date: testDate, startTime: startTime, endTime: endTime);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Create Event", viewResult.ViewData["Title"]);
        Assert.Equal("2024-07-04", viewResult.ViewData["EventDate"]);
        Assert.Equal("2024-07-04T10:00", viewResult.ViewData["StartTime"]);
        Assert.Equal("2024-07-04T12:00", viewResult.ViewData["EndTime"]);
    }

    [Fact]
    public void Create_WithNullTimes_ReturnsViewWithEmptyTimes()
    {
        // Act
        var result = _controller.Create(startTime: null, endTime: null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("", viewResult.ViewData["StartTime"]);
        Assert.Equal("", viewResult.ViewData["EndTime"]);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public void CalendarController_HasAuthorizeAttribute()
    {
        // Arrange
        var type = typeof(CalendarController);

        // Act
        var attributes = type.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(attributes);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidLogger_InitializesController()
    {
        // Arrange
        var logger = A.Fake<ILogger<CalendarController>>();

        // Act
        var controller = new CalendarController(logger);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion

    #region ViewData Tests

    [Fact]
    public void Index_AlwaysSetsTitle()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Family Calendar", viewResult.ViewData["Title"]);
    }

    [Fact]
    public void Create_AlwaysSetsTitle()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Create Event", viewResult.ViewData["Title"]);
    }

    #endregion
}
