using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;
using Xunit;

namespace RushtonRoots.UnitTests.Database;

/// <summary>
/// Tests for verifying the fix for nullable AnchorPersonId to resolve the circular dependency during seeding.
/// </summary>
public class DatabaseSeederTests
{
    /// <summary>
    /// This test verifies the fix for the seeding issue:
    /// The problem was that Household requires AnchorPersonId (FK to Person),
    /// and Person requires HouseholdId (FK to Household), creating a circular dependency.
    /// 
    /// The fix: Making AnchorPersonId nullable allows us to:
    /// 1. Create a household with null AnchorPersonId
    /// 2. Create a person with that household
    /// 3. Update the household with the person's ID
    /// </summary>
    [Fact]
    public async Task CanCreateHouseholdWithNullAnchorPersonId_ThenCreatePerson_ThenUpdateHousehold()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new RushtonRootsDbContext(options);

        // Act - Simulate the seeding process that was failing before the fix
        
        // Step 1: Create household with null AnchorPersonId (this would fail before the fix)
        var household = new Household
        {
            HouseholdName = "Test Family",
            AnchorPersonId = null // This is now allowed!
        };
        context.Households.Add(household);
        await context.SaveChangesAsync();

        // Step 2: Create person with the household
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = household.Id,
            IsDeceased = false
        };
        context.People.Add(person);
        await context.SaveChangesAsync();

        // Step 3: Update household with anchor person ID
        household.AnchorPersonId = person.Id;
        await context.SaveChangesAsync();

        // Assert
        // Reload from database to verify everything was persisted correctly
        var savedHousehold = await context.Households.FindAsync(household.Id);
        var savedPerson = await context.People.FindAsync(person.Id);

        Assert.NotNull(savedHousehold);
        Assert.NotNull(savedPerson);
        Assert.Equal("Test Family", savedHousehold.HouseholdName);
        Assert.Equal(person.Id, savedHousehold.AnchorPersonId);
        Assert.Equal(household.Id, savedPerson.HouseholdId);
        Assert.Equal("John", savedPerson.FirstName);
        Assert.Equal("Doe", savedPerson.LastName);
    }

    [Fact]
    public async Task CanCreateHouseholdWithoutAnchorPerson()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new RushtonRootsDbContext(options);

        // Act - Create a household with no anchor person
        var household = new Household
        {
            HouseholdName = "New Family"
            // AnchorPersonId is not set (null by default)
        };
        context.Households.Add(household);
        await context.SaveChangesAsync();

        // Assert
        var savedHousehold = await context.Households.FindAsync(household.Id);
        Assert.NotNull(savedHousehold);
        Assert.Equal("New Family", savedHousehold.HouseholdName);
        Assert.Null(savedHousehold.AnchorPersonId);
    }
}
