using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Web.Controllers.Api;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers.Api;

public class ParentChildControllerTests
{
    private readonly IParentChildService _mockParentChildService;
    private readonly ILogger<ParentChildController> _mockLogger;
    private readonly ParentChildController _controller;

    public ParentChildControllerTests()
    {
        _mockParentChildService = A.Fake<IParentChildService>();
        _mockLogger = A.Fake<ILogger<ParentChildController>>();
        _controller = new ParentChildController(_mockParentChildService, _mockLogger);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithRelationships()
    {
        // Arrange
        var relationships = new List<ParentChildViewModel>
        {
            new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" },
            new ParentChildViewModel { Id = 2, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" }
        };

        A.CallTo(() => _mockParentChildService.GetAllAsync()).Returns(relationships);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRelationships = Assert.IsAssignableFrom<IEnumerable<ParentChildViewModel>>(okResult.Value);
        Assert.Equal(2, returnedRelationships.Count());
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var relationships = new List<ParentChildViewModel>();
        for (int i = 1; i <= 100; i++)
        {
            relationships.Add(new ParentChildViewModel { Id = i, ParentPersonId = i, ChildPersonId = i + 1, RelationshipType = "Biological" });
        }

        A.CallTo(() => _mockParentChildService.GetAllAsync()).Returns(relationships);

        // Act
        var result = await _controller.GetAll(page: 2, pageSize: 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRelationships = Assert.IsAssignableFrom<IEnumerable<ParentChildViewModel>>(okResult.Value);
        Assert.Equal(10, returnedRelationships.Count());
        Assert.Equal(11, returnedRelationships.First().Id); // Second page starts at item 11
    }

    [Fact]
    public async Task GetAll_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetAllAsync()).Throws<Exception>();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkWithRelationship()
    {
        // Arrange
        var relationship = new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" };
        A.CallTo(() => _mockParentChildService.GetByIdAsync(1)).Returns(relationship);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRelationship = Assert.IsType<ParentChildViewModel>(okResult.Value);
        Assert.Equal(1, returnedRelationship.Id);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetByIdAsync(999)).Returns((ParentChildViewModel?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetByIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetByPersonId Tests

    [Fact]
    public async Task GetByPersonId_ReturnsCombinedRelationships()
    {
        // Arrange
        var asParent = new List<ParentChildViewModel>
        {
            new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" }
        };
        var asChild = new List<ParentChildViewModel>
        {
            new ParentChildViewModel { Id = 2, ParentPersonId = 3, ChildPersonId = 1, RelationshipType = "Biological" }
        };

        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(1)).Returns(asParent);
        A.CallTo(() => _mockParentChildService.GetByChildIdAsync(1)).Returns(asChild);

        // Act
        var result = await _controller.GetByPersonId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRelationships = Assert.IsAssignableFrom<IEnumerable<ParentChildViewModel>>(okResult.Value);
        Assert.Equal(2, returnedRelationships.Count());
    }

    [Fact]
    public async Task GetByPersonId_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetByPersonId(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetParents Tests

    [Fact]
    public async Task GetParents_ReturnsOkWithParents()
    {
        // Arrange
        var parents = new List<ParentChildViewModel>
        {
            new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" },
            new ParentChildViewModel { Id = 2, ParentPersonId = 2, ChildPersonId = 3, RelationshipType = "Biological" }
        };

        A.CallTo(() => _mockParentChildService.GetByChildIdAsync(3)).Returns(parents);

        // Act
        var result = await _controller.GetParents(3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedParents = Assert.IsAssignableFrom<IEnumerable<ParentChildViewModel>>(okResult.Value);
        Assert.Equal(2, returnedParents.Count());
    }

    [Fact]
    public async Task GetParents_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetByChildIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetParents(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetChildren Tests

    [Fact]
    public async Task GetChildren_ReturnsOkWithChildren()
    {
        // Arrange
        var children = new List<ParentChildViewModel>
        {
            new ParentChildViewModel { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" },
            new ParentChildViewModel { Id = 2, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" }
        };

        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(1)).Returns(children);

        // Act
        var result = await _controller.GetChildren(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedChildren = Assert.IsAssignableFrom<IEnumerable<ParentChildViewModel>>(okResult.Value);
        Assert.Equal(2, returnedChildren.Count());
    }

    [Fact]
    public async Task GetChildren_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetByParentIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetChildren(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateParentChildRequest
        {
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        var createdRelationship = new ParentChildViewModel
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _mockParentChildService.CreateAsync(request)).Returns(createdRelationship);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        var returnedRelationship = Assert.IsType<ParentChildViewModel>(createdAtActionResult.Value);
        Assert.Equal(1, returnedRelationship.Id);
    }

    [Fact]
    public async Task Create_WithSelfParent_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateParentChildRequest
        {
            ParentPersonId = 1,
            ChildPersonId = 1, // Same person
            RelationshipType = "Biological"
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("A person cannot be their own parent", badRequestResult.Value);
    }

    [Fact]
    public async Task Create_WhenServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateParentChildRequest
        {
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _mockParentChildService.CreateAsync(request)).Throws(new ArgumentException("Invalid relationship"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid relationship", badRequestResult.Value);
    }

    [Fact]
    public async Task Create_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        var request = new CreateParentChildRequest
        {
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _mockParentChildService.CreateAsync(request)).Throws<Exception>();

        // Act
        var result = await _controller.Create(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new UpdateParentChildRequest
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Adopted"
        };

        var updatedRelationship = new ParentChildViewModel
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Adopted"
        };

        A.CallTo(() => _mockParentChildService.UpdateAsync(request)).Returns(updatedRelationship);

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRelationship = Assert.IsType<ParentChildViewModel>(okResult.Value);
        Assert.Equal(1, returnedRelationship.Id);
    }

    [Fact]
    public async Task Update_WithMismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateParentChildRequest
        {
            Id = 2, // Mismatched ID
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("ID in URL does not match ID in request body", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_WithSelfParent_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateParentChildRequest
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 1, // Same person
            RelationshipType = "Biological"
        };

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("A person cannot be their own parent", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateParentChildRequest
        {
            Id = 999,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _mockParentChildService.UpdateAsync(request)).Throws(new KeyNotFoundException("Relationship not found"));

        // Act
        var result = await _controller.Update(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateParentChildRequest
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _mockParentChildService.UpdateAsync(request)).Throws(new ArgumentException("Invalid update"));

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid update", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        var request = new UpdateParentChildRequest
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _mockParentChildService.UpdateAsync(request)).Throws<Exception>();

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.DeleteAsync(999)).Throws(new KeyNotFoundException("Relationship not found"));

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.DeleteAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Phase 4.2: Evidence & Family Context Tests

    [Fact]
    public async Task GetEvidence_WithValidId_ReturnsOkWithSources()
    {
        // Arrange
        var sources = new List<SourceViewModel>
        {
            new SourceViewModel { Id = 1, Title = "Birth Certificate", SourceType = "Document" },
            new SourceViewModel { Id = 2, Title = "Census Record", SourceType = "Document" }
        };

        A.CallTo(() => _mockParentChildService.GetEvidenceAsync(1)).Returns(sources);

        // Act
        var result = await _controller.GetEvidence(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSources = Assert.IsAssignableFrom<IEnumerable<SourceViewModel>>(okResult.Value);
        Assert.Equal(2, returnedSources.Count());
    }

    [Fact]
    public async Task GetEvidence_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetEvidenceAsync(999)).Throws(new KeyNotFoundException("Relationship not found"));

        // Act
        var result = await _controller.GetEvidence(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetEvidence_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetEvidenceAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetEvidence(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetRelatedEvents_WithValidId_ReturnsOkWithEvents()
    {
        // Arrange
        var events = new List<LifeEventViewModel>
        {
            new LifeEventViewModel { Id = 1, PersonId = 1, EventType = "Birth", Title = "Parent Birth" },
            new LifeEventViewModel { Id = 2, PersonId = 2, EventType = "Birth", Title = "Child Birth" }
        };

        A.CallTo(() => _mockParentChildService.GetRelatedEventsAsync(1)).Returns(events);

        // Act
        var result = await _controller.GetRelatedEvents(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEvents = Assert.IsAssignableFrom<IEnumerable<LifeEventViewModel>>(okResult.Value);
        Assert.Equal(2, returnedEvents.Count());
    }

    [Fact]
    public async Task GetRelatedEvents_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetRelatedEventsAsync(999)).Throws(new KeyNotFoundException("Relationship not found"));

        // Act
        var result = await _controller.GetRelatedEvents(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetRelatedEvents_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetRelatedEventsAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetRelatedEvents(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetGrandparents_WithValidId_ReturnsOkWithPersons()
    {
        // Arrange
        var grandparents = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 10, FirstName = "GrandPa", LastName = "Smith" },
            new PersonViewModel { Id = 11, FirstName = "GrandMa", LastName = "Smith" }
        };

        A.CallTo(() => _mockParentChildService.GetGrandparentsAsync(1)).Returns(grandparents);

        // Act
        var result = await _controller.GetGrandparents(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPersons = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(okResult.Value);
        Assert.Equal(2, returnedPersons.Count());
    }

    [Fact]
    public async Task GetGrandparents_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetGrandparentsAsync(999)).Throws(new KeyNotFoundException("Relationship not found"));

        // Act
        var result = await _controller.GetGrandparents(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetGrandparents_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetGrandparentsAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetGrandparents(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetSiblings_WithValidId_ReturnsOkWithPersons()
    {
        // Arrange
        var siblings = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 3, FirstName = "Jack", LastName = "Doe" },
            new PersonViewModel { Id = 4, FirstName = "Jill", LastName = "Doe" }
        };

        A.CallTo(() => _mockParentChildService.GetSiblingsAsync(1)).Returns(siblings);

        // Act
        var result = await _controller.GetSiblings(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPersons = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(okResult.Value);
        Assert.Equal(2, returnedPersons.Count());
    }

    [Fact]
    public async Task GetSiblings_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetSiblingsAsync(999)).Throws(new KeyNotFoundException("Relationship not found"));

        // Act
        var result = await _controller.GetSiblings(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetSiblings_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.GetSiblingsAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetSiblings(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region VerifyRelationship Tests (Phase 4.3)

    [Fact]
    public async Task VerifyRelationship_ReturnsOkWithVerifiedRelationship()
    {
        // Arrange
        var relationship = new ParentChildViewModel
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _mockParentChildService.VerifyRelationshipAsync(1, A<string>._)).Returns(relationship);

        // Act
        var result = await _controller.VerifyRelationship(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRelationship = Assert.IsType<ParentChildViewModel>(okResult.Value);
        Assert.Equal(1, returnedRelationship.Id);
    }

    [Fact]
    public async Task VerifyRelationship_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.VerifyRelationshipAsync(999, A<string>._))
            .Throws(new NotFoundException("Relationship not found"));

        // Act
        var result = await _controller.VerifyRelationship(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task VerifyRelationship_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockParentChildService.VerifyRelationshipAsync(1, A<string>._)).Throws<Exception>();

        // Act
        var result = await _controller.VerifyRelationship(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region UpdateNotes Tests (Phase 4.3)

    [Fact]
    public async Task UpdateNotes_WithValidRequest_ReturnsOkWithUpdatedRelationship()
    {
        // Arrange
        var request = new UpdateParentChildNotesRequest
        {
            Notes = "Verified via birth certificate"
        };

        var relationship = new ParentChildViewModel
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            Notes = request.Notes
        };

        A.CallTo(() => _mockParentChildService.UpdateNotesAsync(1, request.Notes)).Returns(relationship);

        // Act
        var result = await _controller.UpdateNotes(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRelationship = Assert.IsType<ParentChildViewModel>(okResult.Value);
        Assert.Equal(1, returnedRelationship.Id);
        Assert.Equal(request.Notes, returnedRelationship.Notes);
    }

    [Fact]
    public async Task UpdateNotes_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateParentChildNotesRequest { Notes = "Test notes" };
        A.CallTo(() => _mockParentChildService.UpdateNotesAsync(999, request.Notes))
            .Throws(new NotFoundException("Relationship not found"));

        // Act
        var result = await _controller.UpdateNotes(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateNotes_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateParentChildNotesRequest { Notes = "Test notes" };
        A.CallTo(() => _mockParentChildService.UpdateNotesAsync(1, request.Notes))
            .Throws(new ValidationException("Invalid notes"));

        // Act
        var result = await _controller.UpdateNotes(1, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateNotes_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        var request = new UpdateParentChildNotesRequest { Notes = "Test notes" };
        A.CallTo(() => _mockParentChildService.UpdateNotesAsync(1, request.Notes)).Throws<Exception>();

        // Act
        var result = await _controller.UpdateNotes(1, request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion
}
