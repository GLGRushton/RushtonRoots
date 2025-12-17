using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;

namespace RushtonRoots.UnitTests.Controllers;

public class InfoControllerTests
{
    private readonly ILogger<InfoController> _mockLogger;
    private readonly InfoController _controller;

    public InfoControllerTests()
    {
        _mockLogger = A.Fake<ILogger<InfoController>>();
        _controller = new InfoController(_mockLogger);
    }

    #region About Action Tests

    [Fact]
    public void About_ReturnsViewResult()
    {
        // Act
        var result = _controller.About();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void About_SetsCorrectTitle()
    {
        // Act
        var result = _controller.About() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("About RushtonRoots", result.ViewData["Title"]);
    }

    [Fact]
    public void About_SetsDescriptionMetaTag()
    {
        // Act
        var result = _controller.About() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Description", result.ViewData.Keys);
        Assert.NotNull(result.ViewData["Description"]);
    }

    [Fact]
    public void About_SetsOgMetaTags()
    {
        // Act
        var result = _controller.About() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("OgTitle", result.ViewData.Keys);
        Assert.Contains("OgDescription", result.ViewData.Keys);
        Assert.Contains("OgType", result.ViewData.Keys);
        Assert.Equal("website", result.ViewData["OgType"]);
    }

    [Fact]
    public void About_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.About() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name (same as action name)
    }

    #endregion

    #region Contact Action Tests

    [Fact]
    public void Contact_ReturnsViewResult()
    {
        // Act
        var result = _controller.Contact();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Contact_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Contact() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Contact Us", result.ViewData["Title"]);
    }

    [Fact]
    public void Contact_SetsDescriptionMetaTag()
    {
        // Act
        var result = _controller.Contact() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Description", result.ViewData.Keys);
        Assert.NotNull(result.ViewData["Description"]);
    }

    [Fact]
    public void Contact_SetsOgMetaTags()
    {
        // Act
        var result = _controller.Contact() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("OgTitle", result.ViewData.Keys);
        Assert.Contains("OgDescription", result.ViewData.Keys);
        Assert.Contains("OgType", result.ViewData.Keys);
    }

    [Fact]
    public void Contact_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Contact() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name
    }

    #endregion

    #region Mission Action Tests

    [Fact]
    public void Mission_ReturnsViewResult()
    {
        // Act
        var result = _controller.Mission();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Mission_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Mission() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Our Mission", result.ViewData["Title"]);
    }

    [Fact]
    public void Mission_SetsDescriptionMetaTag()
    {
        // Act
        var result = _controller.Mission() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Description", result.ViewData.Keys);
        Assert.NotNull(result.ViewData["Description"]);
    }

    [Fact]
    public void Mission_SetsOgMetaTags()
    {
        // Act
        var result = _controller.Mission() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("OgTitle", result.ViewData.Keys);
        Assert.Contains("OgDescription", result.ViewData.Keys);
        Assert.Contains("OgType", result.ViewData.Keys);
    }

    [Fact]
    public void Mission_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Mission() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name
    }

    #endregion

    #region Privacy Action Tests

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Privacy() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Privacy Policy", result.ViewData["Title"]);
    }

    [Fact]
    public void Privacy_SetsDescriptionMetaTag()
    {
        // Act
        var result = _controller.Privacy() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Description", result.ViewData.Keys);
        Assert.NotNull(result.ViewData["Description"]);
    }

    [Fact]
    public void Privacy_SetsOgMetaTags()
    {
        // Act
        var result = _controller.Privacy() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("OgTitle", result.ViewData.Keys);
        Assert.Contains("OgDescription", result.ViewData.Keys);
        Assert.Contains("OgType", result.ViewData.Keys);
    }

    [Fact]
    public void Privacy_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Privacy() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name
    }

    #endregion

    #region Terms Action Tests

    [Fact]
    public void Terms_ReturnsViewResult()
    {
        // Act
        var result = _controller.Terms();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Terms_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Terms() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Terms of Service", result.ViewData["Title"]);
    }

    [Fact]
    public void Terms_SetsDescriptionMetaTag()
    {
        // Act
        var result = _controller.Terms() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Description", result.ViewData.Keys);
        Assert.NotNull(result.ViewData["Description"]);
    }

    [Fact]
    public void Terms_SetsOgMetaTags()
    {
        // Act
        var result = _controller.Terms() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("OgTitle", result.ViewData.Keys);
        Assert.Contains("OgDescription", result.ViewData.Keys);
        Assert.Contains("OgType", result.ViewData.Keys);
    }

    [Fact]
    public void Terms_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Terms() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name
    }

    #endregion

    #region Story Action Tests

    [Fact]
    public void Story_ReturnsViewResult()
    {
        // Act
        var result = _controller.Story();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Story_SetsCorrectTitle()
    {
        // Act
        var result = _controller.Story() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Our Family Story", result.ViewData["Title"]);
    }

    [Fact]
    public void Story_SetsDescriptionMetaTag()
    {
        // Act
        var result = _controller.Story() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Description", result.ViewData.Keys);
        Assert.NotNull(result.ViewData["Description"]);
    }

    [Fact]
    public void Story_SetsOgMetaTags()
    {
        // Act
        var result = _controller.Story() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("OgTitle", result.ViewData.Keys);
        Assert.Contains("OgDescription", result.ViewData.Keys);
        Assert.Contains("OgType", result.ViewData.Keys);
    }

    [Fact]
    public void Story_SetsArticleOgType()
    {
        // Act
        var result = _controller.Story() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("article", result.ViewData["OgType"]);
    }

    [Fact]
    public void Story_SetsArticleSection()
    {
        // Act
        var result = _controller.Story() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Contains("ArticleSection", result.ViewData.Keys);
        Assert.Equal("Family History", result.ViewData["ArticleSection"]);
    }

    [Fact]
    public void Story_ReturnsViewWithCorrectViewName()
    {
        // Act
        var result = _controller.Story() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName); // Default view name
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public void Controller_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(InfoController);

        // Act
        var attributes = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.Empty(attributes); // Public controller - no authorization required
    }

    [Fact]
    public void About_Action_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.About));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Public action
    }

    [Fact]
    public void Contact_Action_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.Contact));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Public action
    }

    [Fact]
    public void Mission_Action_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.Mission));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Public action
    }

    [Fact]
    public void Privacy_Action_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.Privacy));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Public action
    }

    [Fact]
    public void Terms_Action_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.Terms));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Public action
    }

    [Fact]
    public void Story_Action_DoesNotHaveAuthorizeAttribute()
    {
        // Arrange
        var methodInfo = typeof(InfoController).GetMethod(nameof(InfoController.Story));

        // Act
        var attributes = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotNull(attributes);
        Assert.Empty(attributes); // Public action
    }

    #endregion

    #region Controller Construction Tests

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange
        var logger = A.Fake<ILogger<InfoController>>();

        // Act
        var controller = new InfoController(logger);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void Constructor_WithNullLogger_CreatesInstanceWithWarning()
    {
        // Note: C# nullable reference types provide compile-time warnings for null parameters
        // The controller doesn't perform runtime null validation as this is caught at compile time
        
        // Act
        var controller = new InfoController(null!);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion

    #region ViewData Tests

    [Fact]
    public void AllActions_SetTitleInViewData()
    {
        // Arrange & Act
        var aboutResult = _controller.About() as ViewResult;
        var contactResult = _controller.Contact() as ViewResult;
        var missionResult = _controller.Mission() as ViewResult;
        var privacyResult = _controller.Privacy() as ViewResult;
        var termsResult = _controller.Terms() as ViewResult;
        var storyResult = _controller.Story() as ViewResult;

        // Assert
        Assert.NotNull(aboutResult);
        Assert.True(aboutResult.ViewData.ContainsKey("Title"));
        Assert.NotEmpty(aboutResult.ViewData["Title"] as string ?? "");

        Assert.NotNull(contactResult);
        Assert.True(contactResult.ViewData.ContainsKey("Title"));
        Assert.NotEmpty(contactResult.ViewData["Title"] as string ?? "");

        Assert.NotNull(missionResult);
        Assert.True(missionResult.ViewData.ContainsKey("Title"));
        Assert.NotEmpty(missionResult.ViewData["Title"] as string ?? "");

        Assert.NotNull(privacyResult);
        Assert.True(privacyResult.ViewData.ContainsKey("Title"));
        Assert.NotEmpty(privacyResult.ViewData["Title"] as string ?? "");

        Assert.NotNull(termsResult);
        Assert.True(termsResult.ViewData.ContainsKey("Title"));
        Assert.NotEmpty(termsResult.ViewData["Title"] as string ?? "");

        Assert.NotNull(storyResult);
        Assert.True(storyResult.ViewData.ContainsKey("Title"));
        Assert.NotEmpty(storyResult.ViewData["Title"] as string ?? "");
    }

    [Fact]
    public void AllActions_SetSEOMetaTagsInViewData()
    {
        // Arrange & Act
        var aboutResult = _controller.About() as ViewResult;
        var contactResult = _controller.Contact() as ViewResult;
        var missionResult = _controller.Mission() as ViewResult;
        var privacyResult = _controller.Privacy() as ViewResult;
        var termsResult = _controller.Terms() as ViewResult;
        var storyResult = _controller.Story() as ViewResult;

        // Assert - All should have SEO meta tags
        Assert.NotNull(aboutResult);
        Assert.True(aboutResult.ViewData.ContainsKey("Description"));
        Assert.True(aboutResult.ViewData.ContainsKey("OgTitle"));
        Assert.True(aboutResult.ViewData.ContainsKey("OgDescription"));
        Assert.True(aboutResult.ViewData.ContainsKey("OgType"));

        Assert.NotNull(contactResult);
        Assert.True(contactResult.ViewData.ContainsKey("Description"));
        Assert.True(contactResult.ViewData.ContainsKey("OgTitle"));

        Assert.NotNull(missionResult);
        Assert.True(missionResult.ViewData.ContainsKey("Description"));
        Assert.True(missionResult.ViewData.ContainsKey("OgTitle"));

        Assert.NotNull(privacyResult);
        Assert.True(privacyResult.ViewData.ContainsKey("Description"));
        Assert.True(privacyResult.ViewData.ContainsKey("OgTitle"));

        Assert.NotNull(termsResult);
        Assert.True(termsResult.ViewData.ContainsKey("Description"));
        Assert.True(termsResult.ViewData.ContainsKey("OgTitle"));

        Assert.NotNull(storyResult);
        Assert.True(storyResult.ViewData.ContainsKey("Description"));
        Assert.True(storyResult.ViewData.ContainsKey("OgTitle"));
    }

    #endregion
}
