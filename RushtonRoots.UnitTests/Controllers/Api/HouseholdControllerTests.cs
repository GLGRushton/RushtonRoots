using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Web.Controllers.Api;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers.Api;

public class HouseholdControllerTests
{
    private readonly IHouseholdService _mockHouseholdService;
    private readonly ILogger<HouseholdController> _mockLogger;
    private readonly HouseholdController _controller;

    public HouseholdControllerTests()
    {
        _mockHouseholdService = A.Fake<IHouseholdService>();
        _mockLogger = A.Fake<ILogger<HouseholdController>>();
        _controller = new HouseholdController(_mockHouseholdService, _mockLogger);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithHouseholds()
    {
        // Arrange
        var households = new List<HouseholdViewModel>
        {
            new HouseholdViewModel { Id = 1, HouseholdName = "Smith Household", MemberCount = 4 },
            new HouseholdViewModel { Id = 2, HouseholdName = "Johnson Household", MemberCount = 3 }
        };

        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedHouseholds = Assert.IsAssignableFrom<IEnumerable<HouseholdViewModel>>(okResult.Value);
        Assert.Equal(2, returnedHouseholds.Count());
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var households = new List<HouseholdViewModel>();
        for (int i = 1; i <= 100; i++)
        {
            households.Add(new HouseholdViewModel { Id = i, HouseholdName = $"Household{i}", MemberCount = i });
        }

        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Returns(households);

        // Act
        var result = await _controller.GetAll(page: 2, pageSize: 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedHouseholds = Assert.IsAssignableFrom<IEnumerable<HouseholdViewModel>>(okResult.Value);
        Assert.Equal(10, returnedHouseholds.Count());
        Assert.Equal(11, returnedHouseholds.First().Id); // Second page starts at item 11
    }

    [Fact]
    public async Task GetAll_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockHouseholdService.GetAllAsync()).Throws<Exception>();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkWithHousehold()
    {
        // Arrange
        var householdId = 1;
        var household = new HouseholdViewModel { Id = householdId, HouseholdName = "Smith Household", MemberCount = 4 };

        A.CallTo(() => _mockHouseholdService.GetByIdAsync(householdId)).Returns(household);

        // Act
        var result = await _controller.GetById(householdId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedHousehold = Assert.IsType<HouseholdViewModel>(okResult.Value);
        Assert.Equal(householdId, returnedHousehold.Id);
        Assert.Equal("Smith Household", returnedHousehold.HouseholdName);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        A.CallTo(() => _mockHouseholdService.GetByIdAsync(householdId)).Returns((HouseholdViewModel?)null);

        // Act
        var result = await _controller.GetById(householdId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("not found", notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task GetById_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        A.CallTo(() => _mockHouseholdService.GetByIdAsync(householdId)).Throws<Exception>();

        // Act
        var result = await _controller.GetById(householdId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetMembers Tests

    [Fact]
    public async Task GetMembers_WithValidId_ReturnsOkWithMembers()
    {
        // Arrange
        var householdId = 1;
        var members = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Smith", HouseholdId = householdId },
            new PersonViewModel { Id = 2, FirstName = "Jane", LastName = "Smith", HouseholdId = householdId }
        };

        A.CallTo(() => _mockHouseholdService.GetMembersAsync(householdId)).Returns(members);

        // Act
        var result = await _controller.GetMembers(householdId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMembers = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(okResult.Value);
        Assert.Equal(2, returnedMembers.Count());
    }

    [Fact]
    public async Task GetMembers_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        A.CallTo(() => _mockHouseholdService.GetMembersAsync(householdId))
            .Throws(new NotFoundException($"Household with ID {householdId} not found."));

        // Act
        var result = await _controller.GetMembers(householdId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetMembers_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        A.CallTo(() => _mockHouseholdService.GetMembersAsync(householdId)).Throws<Exception>();

        // Act
        var result = await _controller.GetMembers(householdId);

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
        var request = new CreateHouseholdRequest
        {
            HouseholdName = "New Household",
            AnchorPersonId = 1
        };

        var createdHousehold = new HouseholdViewModel
        {
            Id = 1,
            HouseholdName = request.HouseholdName,
            AnchorPersonId = request.AnchorPersonId,
            MemberCount = 0
        };

        A.CallTo(() => _mockHouseholdService.CreateAsync(request)).Returns(createdHousehold);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(HouseholdController.GetById), createdResult.ActionName);
        var returnedHousehold = Assert.IsType<HouseholdViewModel>(createdResult.Value);
        Assert.Equal(1, returnedHousehold.Id);
    }

    [Fact]
    public async Task Create_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateHouseholdRequest
        {
            HouseholdName = "New Household"
        };

        A.CallTo(() => _mockHouseholdService.CreateAsync(request))
            .Throws(new ValidationException("Validation failed"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WhenServiceThrows_Returns500()
    {
        // Arrange
        var request = new CreateHouseholdRequest { HouseholdName = "New Household" };
        A.CallTo(() => _mockHouseholdService.CreateAsync(request)).Throws<Exception>();

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
        var householdId = 1;
        var request = new UpdateHouseholdRequest
        {
            Id = householdId,
            HouseholdName = "Updated Household",
            AnchorPersonId = 1
        };

        var updatedHousehold = new HouseholdViewModel
        {
            Id = householdId,
            HouseholdName = request.HouseholdName,
            AnchorPersonId = request.AnchorPersonId,
            MemberCount = 3
        };

        A.CallTo(() => _mockHouseholdService.UpdateAsync(request)).Returns(updatedHousehold);

        // Act
        var result = await _controller.Update(householdId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedHousehold = Assert.IsType<HouseholdViewModel>(okResult.Value);
        Assert.Equal("Updated Household", returnedHousehold.HouseholdName);
    }

    [Fact]
    public async Task Update_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var request = new UpdateHouseholdRequest
        {
            Id = 2,
            HouseholdName = "Updated Household"
        };

        // Act
        var result = await _controller.Update(householdId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WithNotFoundHousehold_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        var request = new UpdateHouseholdRequest
        {
            Id = householdId,
            HouseholdName = "Updated Household"
        };

        A.CallTo(() => _mockHouseholdService.UpdateAsync(request))
            .Throws(new NotFoundException($"Household with ID {householdId} not found."));

        // Act
        var result = await _controller.Update(householdId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        var request = new UpdateHouseholdRequest { Id = householdId, HouseholdName = "Updated" };
        A.CallTo(() => _mockHouseholdService.UpdateAsync(request)).Throws<Exception>();

        // Act
        var result = await _controller.Update(householdId, request);

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
        var householdId = 1;
        A.CallTo(() => _mockHouseholdService.DeleteAsync(householdId)).DoesNothing();

        // Act
        var result = await _controller.Delete(householdId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WithNotFoundHousehold_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        A.CallTo(() => _mockHouseholdService.DeleteAsync(householdId))
            .Throws(new NotFoundException($"Household with ID {householdId} not found."));

        // Act
        var result = await _controller.Delete(householdId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        A.CallTo(() => _mockHouseholdService.DeleteAsync(householdId)).Throws<Exception>();

        // Act
        var result = await _controller.Delete(householdId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region AddMember Tests

    [Fact]
    public async Task AddMember_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var householdId = 1;
        var request = new AddHouseholdMemberRequest
        {
            HouseholdId = householdId,
            PersonId = 10
        };

        A.CallTo(() => _mockHouseholdService.AddMemberAsync(request)).DoesNothing();

        // Act
        var result = await _controller.AddMember(householdId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddMember_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var request = new AddHouseholdMemberRequest
        {
            HouseholdId = 2,
            PersonId = 10
        };

        // Act
        var result = await _controller.AddMember(householdId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddMember_WithNotFoundHousehold_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        var request = new AddHouseholdMemberRequest
        {
            HouseholdId = householdId,
            PersonId = 10
        };

        A.CallTo(() => _mockHouseholdService.AddMemberAsync(request))
            .Throws(new NotFoundException($"Household with ID {householdId} not found."));

        // Act
        var result = await _controller.AddMember(householdId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddMember_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        var request = new AddHouseholdMemberRequest { HouseholdId = householdId, PersonId = 10 };
        A.CallTo(() => _mockHouseholdService.AddMemberAsync(request)).Throws<Exception>();

        // Act
        var result = await _controller.AddMember(householdId, request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region RemoveMember Tests

    [Fact]
    public async Task RemoveMember_WithValidIds_ReturnsNoContent()
    {
        // Arrange
        var householdId = 1;
        var personId = 10;

        A.CallTo(() => _mockHouseholdService.RemoveMemberAsync(householdId, personId)).DoesNothing();

        // Act
        var result = await _controller.RemoveMember(householdId, personId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoveMember_WithNotFoundHousehold_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        var personId = 10;

        A.CallTo(() => _mockHouseholdService.RemoveMemberAsync(householdId, personId))
            .Throws(new NotFoundException($"Household with ID {householdId} not found."));

        // Act
        var result = await _controller.RemoveMember(householdId, personId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RemoveMember_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var personId = 10;

        A.CallTo(() => _mockHouseholdService.RemoveMemberAsync(householdId, personId))
            .Throws(new ValidationException("Person is not a member of this household"));

        // Act
        var result = await _controller.RemoveMember(householdId, personId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RemoveMember_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        var personId = 10;
        A.CallTo(() => _mockHouseholdService.RemoveMemberAsync(householdId, personId)).Throws<Exception>();

        // Act
        var result = await _controller.RemoveMember(householdId, personId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region UpdateSettings Tests

    [Fact]
    public async Task UpdateSettings_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var householdId = 1;
        var request = new UpdateHouseholdSettingsRequest
        {
            Id = householdId,
            IsArchived = true
        };

        var updatedHousehold = new HouseholdViewModel
        {
            Id = householdId,
            HouseholdName = "Test Household",
            MemberCount = 3
        };

        A.CallTo(() => _mockHouseholdService.UpdateSettingsAsync(request)).Returns(updatedHousehold);

        // Act
        var result = await _controller.UpdateSettings(householdId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedHousehold = Assert.IsType<HouseholdViewModel>(okResult.Value);
        Assert.Equal(householdId, returnedHousehold.Id);
    }

    [Fact]
    public async Task UpdateSettings_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var request = new UpdateHouseholdSettingsRequest
        {
            Id = 2,
            IsArchived = true
        };

        // Act
        var result = await _controller.UpdateSettings(householdId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateSettings_WithNotFoundHousehold_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        var request = new UpdateHouseholdSettingsRequest
        {
            Id = householdId,
            IsArchived = true
        };

        A.CallTo(() => _mockHouseholdService.UpdateSettingsAsync(request))
            .Throws(new NotFoundException($"Household with ID {householdId} not found."));

        // Act
        var result = await _controller.UpdateSettings(householdId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateSettings_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        var request = new UpdateHouseholdSettingsRequest { Id = householdId, IsArchived = true };
        A.CallTo(() => _mockHouseholdService.UpdateSettingsAsync(request)).Throws<Exception>();

        // Act
        var result = await _controller.UpdateSettings(householdId, request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region RemoveMemberByUserId Tests

    [Fact]
    public async Task RemoveMemberByUserId_WithValidIds_ReturnsNoContent()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";

        A.CallTo(() => _mockHouseholdService.RemoveMemberByUserIdAsync(householdId, userId)).DoesNothing();

        // Act
        var result = await _controller.RemoveMemberByUserId(householdId, userId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoveMemberByUserId_WithNotFoundUser_ReturnsNotFound()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-999";

        A.CallTo(() => _mockHouseholdService.RemoveMemberByUserIdAsync(householdId, userId))
            .Throws(new NotFoundException($"User with ID {userId} not found or not linked to a person."));

        // Act
        var result = await _controller.RemoveMemberByUserId(householdId, userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RemoveMemberByUserId_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";

        A.CallTo(() => _mockHouseholdService.RemoveMemberByUserIdAsync(householdId, userId))
            .Throws(new ValidationException("Cannot remove a person from a household"));

        // Act
        var result = await _controller.RemoveMemberByUserId(householdId, userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RemoveMemberByUserId_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        A.CallTo(() => _mockHouseholdService.RemoveMemberByUserIdAsync(householdId, userId)).Throws<Exception>();

        // Act
        var result = await _controller.RemoveMemberByUserId(householdId, userId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region UpdateMemberRole Tests

    [Fact]
    public async Task UpdateMemberRole_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var request = new UpdateMemberRoleRequest { Role = "ADMIN" };

        A.CallTo(() => _mockHouseholdService.UpdateMemberRoleAsync(householdId, userId, request.Role)).DoesNothing();

        // Act
        var result = await _controller.UpdateMemberRole(householdId, userId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateMemberRole_WithInvalidRole_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var request = new UpdateMemberRoleRequest { Role = "INVALID" };

        A.CallTo(() => _mockHouseholdService.UpdateMemberRoleAsync(householdId, userId, request.Role))
            .Throws(new ValidationException("Role must be either 'ADMIN' or 'EDITOR'."));

        // Act
        var result = await _controller.UpdateMemberRole(householdId, userId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateMemberRole_WithNotFoundUser_ReturnsNotFound()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-999";
        var request = new UpdateMemberRoleRequest { Role = "ADMIN" };

        A.CallTo(() => _mockHouseholdService.UpdateMemberRoleAsync(householdId, userId, request.Role))
            .Throws(new NotFoundException($"User with ID {userId} not found or not linked to a person."));

        // Act
        var result = await _controller.UpdateMemberRole(householdId, userId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateMemberRole_WithNonMember_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var request = new UpdateMemberRoleRequest { Role = "ADMIN" };

        A.CallTo(() => _mockHouseholdService.UpdateMemberRoleAsync(householdId, userId, request.Role))
            .Throws(new ValidationException($"User is not a member of household {householdId}."));

        // Act
        var result = await _controller.UpdateMemberRole(householdId, userId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateMemberRole_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var request = new UpdateMemberRoleRequest { Role = "ADMIN" };
        A.CallTo(() => _mockHouseholdService.UpdateMemberRoleAsync(householdId, userId, request.Role)).Throws<Exception>();

        // Act
        var result = await _controller.UpdateMemberRole(householdId, userId, request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region ResendInvite Tests

    [Fact]
    public async Task ResendInvite_WithValidIds_ReturnsNoContent()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";

        A.CallTo(() => _mockHouseholdService.ResendInviteAsync(householdId, userId)).DoesNothing();

        // Act
        var result = await _controller.ResendInvite(householdId, userId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ResendInvite_WithNotFoundHousehold_ReturnsNotFound()
    {
        // Arrange
        var householdId = 999;
        var userId = "user-123";

        A.CallTo(() => _mockHouseholdService.ResendInviteAsync(householdId, userId))
            .Throws(new NotFoundException($"Household with ID {householdId} not found."));

        // Act
        var result = await _controller.ResendInvite(householdId, userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ResendInvite_WithNotFoundUser_ReturnsNotFound()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-999";

        A.CallTo(() => _mockHouseholdService.ResendInviteAsync(householdId, userId))
            .Throws(new NotFoundException($"User with ID {userId} not found or not linked to a person."));

        // Act
        var result = await _controller.ResendInvite(householdId, userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ResendInvite_WithNonMember_ReturnsBadRequest()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";

        A.CallTo(() => _mockHouseholdService.ResendInviteAsync(householdId, userId))
            .Throws(new ValidationException($"User is not a member of household {householdId}."));

        // Act
        var result = await _controller.ResendInvite(householdId, userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ResendInvite_WhenServiceThrows_Returns500()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        A.CallTo(() => _mockHouseholdService.ResendInviteAsync(householdId, userId)).Throws<Exception>();

        // Act
        var result = await _controller.ResendInvite(householdId, userId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion
}
