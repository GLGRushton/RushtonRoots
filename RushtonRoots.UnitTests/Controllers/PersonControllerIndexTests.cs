using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Web.Controllers;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers;

/// <summary>
/// Tests for PersonController Index action
/// Specifically tests the fixes for person page bugs
/// </summary>
public class PersonControllerIndexTests
{
    private readonly IPersonService _mockPersonService;
    private readonly IHouseholdService _mockHouseholdService;
    private readonly ILogger<PersonController> _mockLogger;
    private readonly PersonController _controller;

    public PersonControllerIndexTests()
    {
        _mockPersonService = A.Fake<IPersonService>();
        _mockHouseholdService = A.Fake<IHouseholdService>();
        _mockLogger = A.Fake<ILogger<PersonController>>();
        _controller = new PersonController(_mockPersonService, _mockHouseholdService, _mockLogger);
    }

    [Fact]
    public async Task Index_WithNoFilters_CallsGetAllAsync()
    {
        // Arrange
        var people = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new PersonViewModel { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };
        var households = new List<HouseholdViewModel>();

        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count());
        A.CallTo(() => _mockPersonService.GetAllAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Index_WithSearchTerm_CallsSearchAsync()
    {
        // Arrange - Bug #1 fix: SearchTerm should trigger search
        var request = new SearchPersonRequest { SearchTerm = "John" };
        var people = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" }
        };
        var households = new List<HouseholdViewModel>();

        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>._)).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(viewResult.Model);
        Assert.Single(model);
        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>.That.Matches(r => r.SearchTerm == "John")))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockPersonService.GetAllAsync()).MustNotHaveHappened();
    }

    [Fact]
    public async Task Index_WithHouseholdId_CallsSearchAsync()
    {
        // Arrange - Bug #4 fix: HouseholdId should trigger search
        var request = new SearchPersonRequest { HouseholdId = 1 };
        var people = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 }
        };
        var households = new List<HouseholdViewModel>();

        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>._)).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(viewResult.Model);
        Assert.Single(model);
        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>.That.Matches(r => r.HouseholdId == 1)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Index_WithIsDeceased_CallsSearchAsync()
    {
        // Arrange
        var request = new SearchPersonRequest { IsDeceased = true };
        var people = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe", IsDeceased = true }
        };
        var households = new List<HouseholdViewModel>();

        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>._)).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(viewResult.Model);
        Assert.Single(model);
        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>.That.Matches(r => r.IsDeceased == true)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Index_SetsViewBagHouseholds()
    {
        // Arrange - Bug #5 fix: Households should be available in ViewBag
        var households = new List<HouseholdViewModel>
        {
            new HouseholdViewModel { Id = 1, HouseholdName = "Smith Family" },
            new HouseholdViewModel { Id = 2, HouseholdName = "Jones Family" }
        };
        var people = new List<PersonViewModel>();

        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData["Households"]);
        var viewBagHouseholds = Assert.IsAssignableFrom<IEnumerable<HouseholdViewModel>>(viewResult.ViewData["Households"]);
        Assert.Equal(2, viewBagHouseholds.Count());
    }

    [Fact]
    public async Task Index_SetsViewBagSearchRequest()
    {
        // Arrange - Bug #1 fix: SearchRequest should be available in ViewBag
        var request = new SearchPersonRequest { SearchTerm = "John" };
        var people = new List<PersonViewModel>();
        var households = new List<HouseholdViewModel>();

        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>._)).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData["SearchRequest"]);
        var viewBagRequest = Assert.IsType<SearchPersonRequest>(viewResult.ViewData["SearchRequest"]);
        Assert.Equal("John", viewBagRequest.SearchTerm);
    }

    [Fact]
    public async Task Index_WithNullRequest_CreatesEmptySearchRequest()
    {
        // Arrange
        var people = new List<PersonViewModel>();
        var households = new List<HouseholdViewModel>();

        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewBagRequest = Assert.IsType<SearchPersonRequest>(viewResult.ViewData["SearchRequest"]);
        Assert.NotNull(viewBagRequest);
        Assert.Null(viewBagRequest.SearchTerm);
        Assert.Null(viewBagRequest.HouseholdId);
        Assert.Null(viewBagRequest.IsDeceased);
    }

    [Fact]
    public async Task Index_WithMultipleFilters_CallsSearchAsync()
    {
        // Arrange - Bug #4 fix: Multiple filters should work together
        var request = new SearchPersonRequest
        {
            SearchTerm = "John",
            HouseholdId = 1,
            IsDeceased = false
        };
        var people = new List<PersonViewModel>
        {
            new PersonViewModel
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                HouseholdId = 1,
                IsDeceased = false
            }
        };
        var households = new List<HouseholdViewModel>();

        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>._)).Returns(people);
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.Index(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(viewResult.Model);
        Assert.Single(model);
        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>.That.Matches(r =>
            r.SearchTerm == "John" &&
            r.HouseholdId == 1 &&
            r.IsDeceased == false
        ))).MustHaveHappenedOnceExactly();
    }
}
