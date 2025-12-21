using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Repositories;

/// <summary>
/// Tests for household role management repository methods.
/// </summary>
public class HouseholdRepositoryRoleTests : IDisposable
{
    private readonly RushtonRootsDbContext _context;
    private readonly HouseholdRepository _repository;

    public HouseholdRepositoryRoleTests()
    {
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RushtonRootsDbContext(options);
        _repository = new HouseholdRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetPersonIdFromUserIdAsync_WithValidUserId_ReturnsPersonId()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "test@example.com",
            PersonId = 123
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPersonIdFromUserIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public async Task GetPersonIdFromUserIdAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.GetPersonIdFromUserIdAsync(invalidUserId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPersonIdFromUserIdAsync_WithUserWithoutPerson_ReturnsNull()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "test@example.com",
            PersonId = null
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPersonIdFromUserIdAsync(user.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetMemberRoleAsync_WithExistingPermission_ReturnsPermission()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 };
        var permission = new HouseholdPermission
        {
            Id = 1,
            HouseholdId = 1,
            PersonId = 1,
            Role = "ADMIN"
        };

        _context.Households.Add(household);
        _context.People.Add(person);
        _context.HouseholdPermissions.Add(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetMemberRoleAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ADMIN", result.Role);
        Assert.Equal(1, result.HouseholdId);
        Assert.Equal(1, result.PersonId);
    }

    [Fact]
    public async Task GetMemberRoleAsync_WithNoPermission_ReturnsNull()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 };

        _context.Households.Add(household);
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetMemberRoleAsync(1, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithExistingPermission_UpdatesRole()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 };
        var permission = new HouseholdPermission
        {
            Id = 1,
            HouseholdId = 1,
            PersonId = 1,
            Role = "EDITOR"
        };

        _context.Households.Add(household);
        _context.People.Add(person);
        _context.HouseholdPermissions.Add(permission);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateMemberRoleAsync(1, 1, "ADMIN");

        // Assert
        var updatedPermission = await _context.HouseholdPermissions.FindAsync(1);
        Assert.NotNull(updatedPermission);
        Assert.Equal("ADMIN", updatedPermission.Role);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithNoExistingPermission_CreatesPermission()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 };

        _context.Households.Add(household);
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateMemberRoleAsync(1, 1, "EDITOR");

        // Assert
        var permissions = await _context.HouseholdPermissions.ToListAsync();
        Assert.Single(permissions);
        Assert.Equal("EDITOR", permissions[0].Role);
        Assert.Equal(1, permissions[0].HouseholdId);
        Assert.Equal(1, permissions[0].PersonId);
    }

    [Fact]
    public async Task IsHouseholdAdminAsync_WithAdminRole_ReturnsTrue()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 };
        var permission = new HouseholdPermission
        {
            Id = 1,
            HouseholdId = 1,
            PersonId = 1,
            Role = "ADMIN"
        };

        _context.Households.Add(household);
        _context.People.Add(person);
        _context.HouseholdPermissions.Add(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsHouseholdAdminAsync(1, 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHouseholdAdminAsync_WithEditorRole_ReturnsFalse()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 };
        var permission = new HouseholdPermission
        {
            Id = 1,
            HouseholdId = 1,
            PersonId = 1,
            Role = "EDITOR"
        };

        _context.Households.Add(household);
        _context.People.Add(person);
        _context.HouseholdPermissions.Add(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsHouseholdAdminAsync(1, 1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsHouseholdAdminAsync_WithNoPermission_ReturnsFalse()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 };

        _context.Households.Add(household);
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsHouseholdAdminAsync(1, 1);

        // Assert
        Assert.False(result);
    }
}
