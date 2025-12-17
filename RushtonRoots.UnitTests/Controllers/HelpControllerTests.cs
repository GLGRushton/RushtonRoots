using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;

namespace RushtonRoots.UnitTests.Controllers;

public class HelpControllerTests
{
    private readonly ILogger<HelpController> _mockLogger;
    private readonly HelpController _controller;

    public HelpControllerTests()
    {
        _mockLogger = A.Fake<ILogger<HelpController>>();
        _controller = new HelpController(_mockLogger);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange
        var logger = A.Fake<ILogger<HelpController>>();

        // Act
        var controller = new HelpController(logger);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion

    #region Index Action Tests

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Index_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Help & Documentation", result.ViewData["Title"]);
    }

    [Fact]
    public void Index_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name (same as action name)
    }

    #endregion

    #region GettingStarted Action Tests

    [Fact]
    public void GettingStarted_ReturnsViewResult()
    {
        // Act
        var result = _controller.GettingStarted();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void GettingStarted_SetsCorrectTitle()
    {
        // Act
        var result = _controller.GettingStarted() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Getting Started", result.ViewData["Title"]);
    }

    #endregion

    #region Account Action Tests

    [Fact]
    public void Account_ReturnsViewResult()
    {
        // Act
        var result = _controller.Account();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Account_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Account() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Account Help", result.ViewData["Title"]);
    }

    #endregion

    #region Calendar Action Tests

    [Fact]
    public void Calendar_ReturnsViewResult()
    {
        // Act
        var result = _controller.Calendar();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Calendar_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Calendar() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Calendar Help", result.ViewData["Title"]);
    }

    #endregion

    #region PersonManagement Action Tests

    [Fact]
    public void PersonManagement_ReturnsViewResult()
    {
        // Act
        var result = _controller.PersonManagement();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void PersonManagement_SetsCorrectTitle()
    {
        // Act
        var result = _controller.PersonManagement() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Person Management Help", result.ViewData["Title"]);
    }

    #endregion

    #region HouseholdManagement Action Tests

    [Fact]
    public void HouseholdManagement_ReturnsViewResult()
    {
        // Act
        var result = _controller.HouseholdManagement();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void HouseholdManagement_SetsCorrectTitle()
    {
        // Act
        var result = _controller.HouseholdManagement() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Household Management Help", result.ViewData["Title"]);
    }

    #endregion

    #region RelationshipManagement Action Tests

    [Fact]
    public void RelationshipManagement_ReturnsViewResult()
    {
        // Act
        var result = _controller.RelationshipManagement();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void RelationshipManagement_SetsCorrectTitle()
    {
        // Act
        var result = _controller.RelationshipManagement() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Relationship Management Help", result.ViewData["Title"]);
    }

    #endregion

    #region Recipes Action Tests

    [Fact]
    public void Recipes_ReturnsViewResult()
    {
        // Act
        var result = _controller.Recipes();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Recipes_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Recipes() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Recipes Help", result.ViewData["Title"]);
    }

    #endregion

    #region Stories Action Tests

    [Fact]
    public void Stories_ReturnsViewResult()
    {
        // Act
        var result = _controller.Stories();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Stories_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Stories() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Stories Help", result.ViewData["Title"]);
    }

    #endregion

    #region Traditions Action Tests

    [Fact]
    public void Traditions_ReturnsViewResult()
    {
        // Act
        var result = _controller.Traditions();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Traditions_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Traditions() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Traditions Help", result.ViewData["Title"]);
    }

    #endregion

    #region Wiki Action Tests

    [Fact]
    public void Wiki_ReturnsViewResult()
    {
        // Act
        var result = _controller.Wiki();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Wiki_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Wiki() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Wiki Help", result.ViewData["Title"]);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public void Controller_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(HelpController);

        // Act
        var attributes = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        // Help pages should be publicly accessible (no authorization required)
        Assert.Empty(attributes);
    }

    [Fact]
    public void Index_Action_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(HelpController).GetMethod(nameof(HelpController.Index));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Public help page
    }

    #endregion

    #region ViewData Tests

    [Fact]
    public void Index_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Index_ViewData_TitleIsNotEmpty()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        var title = result.ViewData["Title"] as string;
        Assert.NotNull(title);
        Assert.NotEmpty(title);
    }

    [Fact]
    public void GettingStarted_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.GettingStarted() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Account_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Account() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Calendar_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Calendar() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void PersonManagement_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.PersonManagement() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void HouseholdManagement_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.HouseholdManagement() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void RelationshipManagement_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.RelationshipManagement() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Recipes_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Recipes() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Stories_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Stories() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Traditions_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Traditions() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    [Fact]
    public void Wiki_ViewData_ContainsTitle()
    {
        // Act
        var result = _controller.Wiki() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ViewData.ContainsKey("Title"));
    }

    #endregion

    #region Action Count Tests

    [Fact]
    public void Controller_HasExpectedNumberOfActions()
    {
        // Arrange
        var controllerType = typeof(HelpController);
        var publicMethods = controllerType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        
        // Filter to only action methods (returning IActionResult)
        var actionMethods = publicMethods.Where(m => typeof(IActionResult).IsAssignableFrom(m.ReturnType)).ToList();

        // Assert
        // Should have 11 action methods (Index + 10 help topics)
        Assert.Equal(11, actionMethods.Count);
    }

    #endregion
}
