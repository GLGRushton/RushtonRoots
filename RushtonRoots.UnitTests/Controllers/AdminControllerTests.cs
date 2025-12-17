using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;

namespace RushtonRoots.UnitTests.Controllers;

public class AdminControllerTests
{
    private readonly ILogger<AdminController> _mockLogger;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _mockLogger = A.Fake<ILogger<AdminController>>();
        _controller = new AdminController(_mockLogger);
    }

    #region Dashboard Action Tests

    [Fact]
    public void Dashboard_ReturnsViewResult()
    {
        // Act
        var result = _controller.Dashboard();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Dashboard_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Dashboard() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Admin Dashboard", result.ViewData["Title"]);
    }

    [Fact]
    public void Dashboard_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Dashboard() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name (same as action name)
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
        var result = _controller.Settings() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("System Settings", result.ViewData["Title"]);
    }

    [Fact]
    public void Settings_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Settings() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name (same as action name)
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public void Controller_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(AdminController);

        // Act
        var attributes = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(attributes);
    }

    [Fact]
    public void Controller_RequiresAdminRole()
    {
        // Arrange
        var controllerType = typeof(AdminController);

        // Act
        var authorizeAttributes = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();

        // Assert
        Assert.NotNull(authorizeAttributes);
        var adminAttribute = authorizeAttributes.FirstOrDefault(a => a.Roles == "Admin");
        Assert.NotNull(adminAttribute);
        Assert.Equal("Admin", adminAttribute.Roles);
    }

    [Fact]
    public void Dashboard_Action_DoesNotHaveAdditionalAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(AdminController).GetMethod(nameof(AdminController.Dashboard));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Should not have method-level authorize (uses class-level)
    }

    [Fact]
    public void Settings_Action_DoesNotHaveAdditionalAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(AdminController).GetMethod(nameof(AdminController.Settings));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Should not have method-level authorize (uses class-level)
    }

    #endregion

    #region Controller Construction Tests

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange
        var logger = A.Fake<ILogger<AdminController>>();

        // Act
        var controller = new AdminController(logger);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void Constructor_WithNullLogger_CreatesInstanceWithWarning()
    {
        // Note: C# nullable reference types provide compile-time warnings for null parameters
        // The controller doesn't perform runtime null validation as this is caught at compile time
        
        // Act
        var controller = new AdminController(null!);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion

    #region ViewData Tests

    [Fact]
    public void Dashboard_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Dashboard() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Settings_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Settings() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Dashboard_ViewData_TitleIsNotEmpty()
    {
        // Act
        var result = _controller.Dashboard() as ViewResult;

        // Assert
        Assert.NotNull(result);
        var title = result.ViewData["Title"] as string;
        Assert.NotNull(title);
        Assert.NotEmpty(title);
    }

    [Fact]
    public void Settings_ViewData_TitleIsNotEmpty()
    {
        // Act
        var result = _controller.Settings() as ViewResult;

        // Assert
        Assert.NotNull(result);
        var title = result.ViewData["Title"] as string;
        Assert.NotNull(title);
        Assert.NotEmpty(title);
    }

    #endregion
}
