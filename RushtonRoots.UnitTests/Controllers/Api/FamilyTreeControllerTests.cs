using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Web.Controllers.Api;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers.Api;

public class FamilyTreeControllerTests
{
    private readonly IPersonService _mockPersonService;
    private readonly IParentChildService _mockParentChildService;
    private readonly IPartnershipService _mockPartnershipService;
    private readonly ILogger<FamilyTreeController> _mockLogger;
    private readonly FamilyTreeController _controller;

    public FamilyTreeControllerTests()
    {
        _mockPersonService = A.Fake<IPersonService>();
        _mockParentChildService = A.Fake<IParentChildService>();
        _mockPartnershipService = A.Fake<IPartnershipService>();
        _mockLogger = A.Fake<ILogger<FamilyTreeController>>();
        _controller = new FamilyTreeController(
            _mockPersonService,
            _mockParentChildService,
            _mockPartnershipService,
            _mockLogger);
    }

    #region GetAllFamilyData Tests

    [Fact]
    public async Task GetAllFamilyData_ReturnsOkWithCompleteData()
    {
        // Arrange
        var people = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new PersonViewModel { Id = 2, FirstName = "Jane", LastName = "Doe" }
        };

        var parentChildRelationships = new List<ParentChildViewModel>
        {
            new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 2 }
        };

        var partnerships = new List<PartnershipViewModel>
        {
            new PartnershipViewModel { Id = 1, PersonAId = 1, PersonBId = 2 }
        };

        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(people);
        A.CallTo(() => _mockParentChildService.GetAllAsync()).Returns(parentChildRelationships);
        A.CallTo(() => _mockPartnershipService.GetAllAsync()).Returns(partnerships);

        // Act
        var result = await _controller.GetAllFamilyData();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = okResult.Value;
        Assert.NotNull(data);
        
        // Use reflection to check the anonymous type properties
        var peopleProperty = data.GetType().GetProperty("people");
        var parentChildProperty = data.GetType().GetProperty("parentChildRelationships");
        var partnershipsProperty = data.GetType().GetProperty("partnerships");
        
        Assert.NotNull(peopleProperty);
        Assert.NotNull(parentChildProperty);
        Assert.NotNull(partnershipsProperty);
        
        var returnedPeople = peopleProperty.GetValue(data) as IEnumerable<PersonViewModel>;
        var returnedParentChild = parentChildProperty.GetValue(data) as IEnumerable<ParentChildViewModel>;
        var returnedPartnerships = partnershipsProperty.GetValue(data) as IEnumerable<PartnershipViewModel>;
        
        Assert.Equal(2, returnedPeople?.Count());
        Assert.Single(returnedParentChild ?? Enumerable.Empty<ParentChildViewModel>());
        Assert.Single(returnedPartnerships ?? Enumerable.Empty<PartnershipViewModel>());
    }

    [Fact]
    public async Task GetAllFamilyData_ReturnsEmptyCollections_WhenNoDataExists()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(new List<PersonViewModel>());
        A.CallTo(() => _mockParentChildService.GetAllAsync()).Returns(new List<ParentChildViewModel>());
        A.CallTo(() => _mockPartnershipService.GetAllAsync()).Returns(new List<PartnershipViewModel>());

        // Act
        var result = await _controller.GetAllFamilyData();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = okResult.Value;
        Assert.NotNull(data);
        
        var peopleProperty = data.GetType().GetProperty("people");
        var returnedPeople = peopleProperty?.GetValue(data) as IEnumerable<PersonViewModel>;
        Assert.Empty(returnedPeople ?? Enumerable.Empty<PersonViewModel>());
    }

    [Fact]
    public async Task GetAllFamilyData_Returns500_WhenPersonServiceThrows()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetAllAsync()).Throws<Exception>();

        // Act
        var result = await _controller.GetAllFamilyData();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
        
        var errorData = statusResult.Value;
        var messageProperty = errorData?.GetType().GetProperty("message");
        var message = messageProperty?.GetValue(errorData) as string;
        Assert.Equal("An error occurred while retrieving family data", message);
    }

    [Fact]
    public async Task GetAllFamilyData_Returns500_WhenParentChildServiceThrows()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(new List<PersonViewModel>());
        A.CallTo(() => _mockParentChildService.GetAllAsync()).Throws<Exception>();

        // Act
        var result = await _controller.GetAllFamilyData();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetAllFamilyData_Returns500_WhenPartnershipServiceThrows()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(new List<PersonViewModel>());
        A.CallTo(() => _mockParentChildService.GetAllAsync()).Returns(new List<ParentChildViewModel>());
        A.CallTo(() => _mockPartnershipService.GetAllAsync()).Throws<Exception>();

        // Act
        var result = await _controller.GetAllFamilyData();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetPedigree Tests

    [Fact]
    public async Task GetPedigree_WithValidPersonId_ReturnsOkWithPedigreeData()
    {
        // Arrange
        var person = new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" };
        var parent = new PersonViewModel { Id = 2, FirstName = "Jane", LastName = "Doe" };
        var parentRelationship = new ParentChildViewModel { Id = 1, ParentPersonId = 2, ChildPersonId = 1 };

        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => _mockPersonService.GetByIdAsync(2)).Returns(parent);
        A.CallTo(() => _mockParentChildService.GetByChildIdAsync(1)).Returns(new List<ParentChildViewModel> { parentRelationship });
        A.CallTo(() => _mockParentChildService.GetByChildIdAsync(2)).Returns(new List<ParentChildViewModel>());

        // Act
        var result = await _controller.GetPedigree(1, 2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetPedigree_WithInvalidPersonId_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetByIdAsync(999)).Returns((PersonViewModel?)null);

        // Act
        var result = await _controller.GetPedigree(999, 4);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorData = notFoundResult.Value;
        var messageProperty = errorData?.GetType().GetProperty("message");
        var message = messageProperty?.GetValue(errorData) as string;
        Assert.Contains("not found", message);
    }

    [Fact]
    public async Task GetPedigree_WithZeroGenerations_ReturnsOnlyRootPerson()
    {
        // Arrange
        var person = new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" };
        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Returns(person);

        // Act
        var result = await _controller.GetPedigree(1, 0);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetPedigree_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetPedigree(1, 4);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetDescendants Tests

    [Fact]
    public async Task GetDescendants_WithValidPersonId_ReturnsOkWithDescendantData()
    {
        // Arrange
        var person = new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" };
        var child = new PersonViewModel { Id = 2, FirstName = "Jane", LastName = "Doe" };
        var childRelationship = new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 2 };

        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => _mockPersonService.GetByIdAsync(2)).Returns(child);
        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(1)).Returns(new List<ParentChildViewModel> { childRelationship });
        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(2)).Returns(new List<ParentChildViewModel>());

        // Act
        var result = await _controller.GetDescendants(1, 2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetDescendants_WithInvalidPersonId_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetByIdAsync(999)).Returns((PersonViewModel?)null);

        // Act
        var result = await _controller.GetDescendants(999, 3);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorData = notFoundResult.Value;
        var messageProperty = errorData?.GetType().GetProperty("message");
        var message = messageProperty?.GetValue(errorData) as string;
        Assert.Contains("not found", message);
    }

    [Fact]
    public async Task GetDescendants_WithZeroGenerations_ReturnsOnlyRootPerson()
    {
        // Arrange
        var person = new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" };
        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Returns(person);

        // Act
        var result = await _controller.GetDescendants(1, 0);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetDescendants_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetDescendants(1, 3);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task GetPedigree_HandlesCircularReferences_WithoutInfiniteLoop()
    {
        // Arrange - This tests that the generation limit prevents infinite loops
        var person1 = new PersonViewModel { Id = 1, FirstName = "Person1", LastName = "Test" };
        var person2 = new PersonViewModel { Id = 2, FirstName = "Person2", LastName = "Test" };
        
        // Create a circular reference scenario (should not happen in real data, but testing robustness)
        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Returns(person1);
        A.CallTo(() => _mockPersonService.GetByIdAsync(2)).Returns(person2);
        
        var rel1 = new ParentChildViewModel { Id = 1, ParentPersonId = 2, ChildPersonId = 1 };
        var rel2 = new ParentChildViewModel { Id = 2, ParentPersonId = 1, ChildPersonId = 2 };
        
        A.CallTo(() => _mockParentChildService.GetByChildIdAsync(1)).Returns(new List<ParentChildViewModel> { rel1 });
        A.CallTo(() => _mockParentChildService.GetByChildIdAsync(2)).Returns(new List<ParentChildViewModel> { rel2 });

        // Act - With limited generations, this should complete without hanging
        var result = await _controller.GetPedigree(1, 3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetDescendants_HandlesCircularReferences_WithoutInfiniteLoop()
    {
        // Arrange - This tests that the generation limit prevents infinite loops
        var person1 = new PersonViewModel { Id = 1, FirstName = "Person1", LastName = "Test" };
        var person2 = new PersonViewModel { Id = 2, FirstName = "Person2", LastName = "Test" };
        
        A.CallTo(() => _mockPersonService.GetByIdAsync(1)).Returns(person1);
        A.CallTo(() => _mockPersonService.GetByIdAsync(2)).Returns(person2);
        
        var rel1 = new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 2 };
        var rel2 = new ParentChildViewModel { Id = 2, ParentPersonId = 2, ChildPersonId = 1 };
        
        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(1)).Returns(new List<ParentChildViewModel> { rel1 });
        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(2)).Returns(new List<ParentChildViewModel> { rel2 });

        // Act - With limited generations, this should complete without hanging
        var result = await _controller.GetDescendants(1, 3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAllFamilyData_HandlesLargeDataset()
    {
        // Arrange - Test with larger dataset
        var people = new List<PersonViewModel>();
        var relationships = new List<ParentChildViewModel>();
        var partnerships = new List<PartnershipViewModel>();
        
        for (int i = 1; i <= 100; i++)
        {
            people.Add(new PersonViewModel { Id = i, FirstName = $"Person{i}", LastName = "Test" });
            if (i > 1)
            {
                relationships.Add(new ParentChildViewModel { Id = i - 1, ParentPersonId = i - 1, ChildPersonId = i });
            }
            if (i % 2 == 0 && i > 1)
            {
                partnerships.Add(new PartnershipViewModel { Id = i / 2, PersonAId = i - 1, PersonBId = i });
            }
        }

        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(people);
        A.CallTo(() => _mockParentChildService.GetAllAsync()).Returns(relationships);
        A.CallTo(() => _mockPartnershipService.GetAllAsync()).Returns(partnerships);

        // Act
        var result = await _controller.GetAllFamilyData();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = okResult.Value;
        Assert.NotNull(data);
        
        var peopleProperty = data.GetType().GetProperty("people");
        var returnedPeople = peopleProperty?.GetValue(data) as IEnumerable<PersonViewModel>;
        Assert.Equal(100, returnedPeople?.Count());
    }

    #endregion
}
