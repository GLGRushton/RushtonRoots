using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Application.Validators;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

/// <summary>
/// Tests for household role management service methods.
/// </summary>
public class HouseholdServiceRoleTests
{
    private readonly IHouseholdRepository _mockRepository;
    private readonly IHouseholdMapper _mockMapper;
    private readonly IHouseholdValidator _mockValidator;
    private readonly HouseholdService _service;

    public HouseholdServiceRoleTests()
    {
        _mockRepository = A.Fake<IHouseholdRepository>();
        _mockMapper = A.Fake<IHouseholdMapper>();
        _mockValidator = A.Fake<IHouseholdValidator>();
        _service = new HouseholdService(_mockRepository, _mockMapper, _mockValidator);
    }

    #region RemoveMemberByUserIdAsync Tests

    [Fact]
    public async Task RemoveMemberByUserIdAsync_WithValidUserId_CallsRemoveMemberAsync()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var personId = 10;

        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns(personId);
        A.CallTo(() => _mockRepository.RemoveMemberAsync(householdId, personId))
            .Throws(new InvalidOperationException("Cannot remove a person from a household"));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _service.RemoveMemberByUserIdAsync(householdId, userId));

        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RemoveMemberByUserIdAsync_WithInvalidHouseholdId_ThrowsValidationException()
    {
        // Arrange
        var householdId = 0;
        var userId = "user-123";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.RemoveMemberByUserIdAsync(householdId, userId));

        Assert.Contains("Invalid household ID", exception.Message);
    }

    [Fact]
    public async Task RemoveMemberByUserIdAsync_WithNullUserId_ThrowsValidationException()
    {
        // Arrange
        var householdId = 1;
        string? userId = null;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.RemoveMemberByUserIdAsync(householdId, userId!));

        Assert.Contains("Invalid user ID", exception.Message);
    }

    [Fact]
    public async Task RemoveMemberByUserIdAsync_WithUserNotLinkedToPerson_ThrowsNotFoundException()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-999";

        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns((int?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.RemoveMemberByUserIdAsync(householdId, userId));

        Assert.Contains("User with ID user-999 not found or not linked to a person", exception.Message);
    }

    #endregion

    #region UpdateMemberRoleAsync Tests

    [Fact]
    public async Task UpdateMemberRoleAsync_WithValidRequest_UpdatesRole()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var personId = 10;
        var role = "ADMIN";
        var members = new List<Person>
        {
            new Person { Id = 10, FirstName = "John", LastName = "Doe", HouseholdId = householdId }
        };

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns(personId);
        A.CallTo(() => _mockRepository.GetMembersAsync(householdId)).Returns(members);
        A.CallTo(() => _mockRepository.UpdateMemberRoleAsync(householdId, personId, role)).DoesNothing();

        // Act
        await _service.UpdateMemberRoleAsync(householdId, userId, role);

        // Assert
        A.CallTo(() => _mockRepository.UpdateMemberRoleAsync(householdId, personId, role)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithInvalidHouseholdId_ThrowsValidationException()
    {
        // Arrange
        var householdId = 0;
        var userId = "user-123";
        var role = "ADMIN";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.UpdateMemberRoleAsync(householdId, userId, role));

        Assert.Contains("Invalid household ID", exception.Message);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithInvalidRole_ThrowsValidationException()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var role = "INVALID";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.UpdateMemberRoleAsync(householdId, userId, role));

        Assert.Contains("Role must be either 'ADMIN' or 'EDITOR'", exception.Message);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithNonExistentHousehold_ThrowsNotFoundException()
    {
        // Arrange
        var householdId = 999;
        var userId = "user-123";
        var role = "ADMIN";

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.UpdateMemberRoleAsync(householdId, userId, role));

        Assert.Contains("Household with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithUserNotLinkedToPerson_ThrowsNotFoundException()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-999";
        var role = "ADMIN";

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns((int?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.UpdateMemberRoleAsync(householdId, userId, role));

        Assert.Contains("User with ID user-999 not found or not linked to a person", exception.Message);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithUserNotMember_ThrowsValidationException()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var personId = 10;
        var role = "ADMIN";
        var members = new List<Person>
        {
            new Person { Id = 99, FirstName = "Jane", LastName = "Doe", HouseholdId = householdId }
        };

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns(personId);
        A.CallTo(() => _mockRepository.GetMembersAsync(householdId)).Returns(members);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.UpdateMemberRoleAsync(householdId, userId, role));

        Assert.Contains("User is not a member of household", exception.Message);
    }

    [Theory]
    [InlineData("ADMIN")]
    [InlineData("EDITOR")]
    public async Task UpdateMemberRoleAsync_WithValidRoles_Succeeds(string role)
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var personId = 10;
        var members = new List<Person>
        {
            new Person { Id = 10, FirstName = "John", LastName = "Doe", HouseholdId = householdId }
        };

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns(personId);
        A.CallTo(() => _mockRepository.GetMembersAsync(householdId)).Returns(members);
        A.CallTo(() => _mockRepository.UpdateMemberRoleAsync(householdId, personId, role)).DoesNothing();

        // Act
        await _service.UpdateMemberRoleAsync(householdId, userId, role);

        // Assert
        A.CallTo(() => _mockRepository.UpdateMemberRoleAsync(householdId, personId, role)).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region ResendInviteAsync Tests

    [Fact]
    public async Task ResendInviteAsync_WithValidRequest_Succeeds()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var personId = 10;
        var members = new List<Person>
        {
            new Person { Id = 10, FirstName = "John", LastName = "Doe", HouseholdId = householdId }
        };

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns(personId);
        A.CallTo(() => _mockRepository.GetMembersAsync(householdId)).Returns(members);

        // Act
        await _service.ResendInviteAsync(householdId, userId);

        // Assert - Should complete without throwing
        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockRepository.GetMembersAsync(householdId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ResendInviteAsync_WithInvalidHouseholdId_ThrowsValidationException()
    {
        // Arrange
        var householdId = 0;
        var userId = "user-123";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.ResendInviteAsync(householdId, userId));

        Assert.Contains("Invalid household ID", exception.Message);
    }

    [Fact]
    public async Task ResendInviteAsync_WithNullUserId_ThrowsValidationException()
    {
        // Arrange
        var householdId = 1;
        string? userId = null;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.ResendInviteAsync(householdId, userId!));

        Assert.Contains("Invalid user ID", exception.Message);
    }

    [Fact]
    public async Task ResendInviteAsync_WithNonExistentHousehold_ThrowsNotFoundException()
    {
        // Arrange
        var householdId = 999;
        var userId = "user-123";

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.ResendInviteAsync(householdId, userId));

        Assert.Contains("Household with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task ResendInviteAsync_WithUserNotLinkedToPerson_ThrowsNotFoundException()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-999";

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns((int?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.ResendInviteAsync(householdId, userId));

        Assert.Contains("User with ID user-999 not found or not linked to a person", exception.Message);
    }

    [Fact]
    public async Task ResendInviteAsync_WithUserNotMember_ThrowsValidationException()
    {
        // Arrange
        var householdId = 1;
        var userId = "user-123";
        var personId = 10;
        var members = new List<Person>
        {
            new Person { Id = 99, FirstName = "Jane", LastName = "Doe", HouseholdId = householdId }
        };

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetPersonIdFromUserIdAsync(userId)).Returns(personId);
        A.CallTo(() => _mockRepository.GetMembersAsync(householdId)).Returns(members);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _service.ResendInviteAsync(householdId, userId));

        Assert.Contains("User is not a member of household", exception.Message);
    }

    #endregion
}
