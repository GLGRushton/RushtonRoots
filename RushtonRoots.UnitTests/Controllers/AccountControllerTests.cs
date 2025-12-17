using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Application.Services;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;

namespace RushtonRoots.UnitTests.Controllers;

public class AccountControllerTests
{
    private readonly IAccountService _mockAccountService;
    private readonly ILogger<AccountController> _mockLogger;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _mockAccountService = A.Fake<IAccountService>();
        _mockLogger = A.Fake<ILogger<AccountController>>();
        _controller = new AccountController(_mockAccountService, _mockLogger);
    }

    #region Notifications Action Tests

    [Fact]
    public void Notifications_ReturnsViewResult()
    {
        // Act
        var result = _controller.Notifications();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Notifications_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Notifications();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Notifications", viewResult.ViewData["Title"]);
    }

    [Fact]
    public void Notifications_HasAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(AccountController).GetMethod(nameof(AccountController.Notifications));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);

        // Assert
        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
    }

    [Fact]
    public void Notifications_IsHttpGetAction()
    {
        // Arrange
        var methodInfo = typeof(AccountController).GetMethod(nameof(AccountController.Notifications));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute), false);

        // Assert
        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
    }

    #endregion

    #region Settings Action Tests

    [Fact]
    public void Settings_ReturnsViewResult()
    {
        // Act
        var result = _controller.Settings();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Settings_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Settings();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Settings", viewResult.ViewData["Title"]);
    }

    [Fact]
    public void Settings_HasAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(AccountController).GetMethod(nameof(AccountController.Settings));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);

        // Assert
        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
    }

    [Fact]
    public void Settings_IsHttpGetAction()
    {
        // Arrange
        var methodInfo = typeof(AccountController).GetMethod(nameof(AccountController.Settings));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute), false);

        // Assert
        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_InitializesController()
    {
        // Arrange
        var accountService = A.Fake<IAccountService>();
        var logger = A.Fake<ILogger<AccountController>>();

        // Act
        var controller = new AccountController(accountService, logger);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion
}
