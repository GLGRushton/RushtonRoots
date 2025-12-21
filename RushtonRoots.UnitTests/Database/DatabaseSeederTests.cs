using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

    [Fact]
    public async Task SeedAsync_CreatesAllHouseholdsAndPeople()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new RushtonRootsDbContext(options);

        var userManager = A.Fake<UserManager<ApplicationUser>>(x => x.WithArgumentsForConstructor(() => 
            new UserManager<ApplicationUser>(
                A.Fake<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!, null!, null!, null!, null!)));
        var roleManager = A.Fake<RoleManager<IdentityRole>>(x => x.WithArgumentsForConstructor(() =>
            new RoleManager<IdentityRole>(
                A.Fake<IRoleStore<IdentityRole>>(),
                null!, null!, null!, null!)));
        var logger = A.Fake<ILogger<DatabaseSeeder>>();

        A.CallTo(() => roleManager.RoleExistsAsync(A<string>._)).Returns(false);
        A.CallTo(() => roleManager.CreateAsync(A<IdentityRole>._)).Returns(IdentityResult.Success);
        A.CallTo(() => userManager.FindByEmailAsync(A<string>._)).Returns((ApplicationUser?)null);
        A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>._, A<string>._)).Returns(IdentityResult.Success);
        A.CallTo(() => userManager.AddToRoleAsync(A<ApplicationUser>._, A<string>._)).Returns(IdentityResult.Success);

        var seeder = new DatabaseSeeder(context, userManager, roleManager, logger);

        // Act
        await seeder.SeedAsync();

        // Assert
        // Verify households
        var households = await context.Households.ToListAsync();
        Assert.Equal(7, households.Count);
        
        var householdNames = households.Select(h => h.HouseholdName).OrderBy(n => n).ToList();
        Assert.Contains("Sue and Dave", householdNames);
        Assert.Contains("Daniel and Sarah", householdNames);
        Assert.Contains("Robert and Laura", householdNames);
        Assert.Contains("Sarah and Colin", householdNames);
        Assert.Contains("Anne and Paul", householdNames);
        Assert.Contains("George and Helen", householdNames);
        Assert.Contains("Catherine and Dominic", householdNames);

        // Verify people count
        var people = await context.People.ToListAsync();
        Assert.Equal(22, people.Count); // Total people seeded

        // Verify specific people
        var susan = people.FirstOrDefault(p => p.FirstName == "Susan" && p.LastName == "Rushton");
        Assert.NotNull(susan);
        Assert.Equal("Female", susan.Gender);
        Assert.Equal(new DateTime(1958, 7, 10), susan.DateOfBirth);

        var david = people.FirstOrDefault(p => p.FirstName == "David" && p.LastName == "Rushton");
        Assert.NotNull(david);
        Assert.Equal("Male", david.Gender);
        Assert.Equal(new DateTime(1958, 5, 6), david.DateOfBirth);

        // Verify partnerships
        var partnerships = await context.Partnerships.ToListAsync();
        Assert.Equal(7, partnerships.Count); // One partnership per household

        // Verify parent-child relationships
        var parentChildren = await context.ParentChildren.ToListAsync();
        // Susan and David have 7 children: Daniel, Robert, Charles, Sarah M., Anne, George, Catherine
        // Each child has 2 parent relationships (one for each parent) = 14
        // Plus grandchildren: Lilly, Isla (2 each = 4), Cian, Niamh, Aoife (2 each = 6), William, Darcie (2 each = 4)
        // Total grandchildren parent relationships = 14
        // Total: 14 (children of Susan/David) + 14 (grandchildren) = 28
        Assert.Equal(28, parentChildren.Count);

        // Verify Susan and David's children
        var susanChildren = parentChildren.Where(pc => pc.ParentPersonId == susan.Id).ToList();
        Assert.Equal(7, susanChildren.Count); // 7 children

        var davidChildren = parentChildren.Where(pc => pc.ParentPersonId == david.Id).ToList();
        Assert.Equal(7, davidChildren.Count); // 7 children
    }

    [Fact]
    public async Task SeedAsync_DoesNotSeedWhenDataAlreadyExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new RushtonRootsDbContext(options);

        // Add existing household
        context.Households.Add(new Household { HouseholdName = "Existing" });
        await context.SaveChangesAsync();

        var userManager = A.Fake<UserManager<ApplicationUser>>(x => x.WithArgumentsForConstructor(() => 
            new UserManager<ApplicationUser>(
                A.Fake<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!, null!, null!, null!, null!)));
        var roleManager = A.Fake<RoleManager<IdentityRole>>(x => x.WithArgumentsForConstructor(() =>
            new RoleManager<IdentityRole>(
                A.Fake<IRoleStore<IdentityRole>>(),
                null!, null!, null!, null!)));
        var logger = A.Fake<ILogger<DatabaseSeeder>>();

        A.CallTo(() => roleManager.RoleExistsAsync(A<string>._)).Returns(false);
        A.CallTo(() => roleManager.CreateAsync(A<IdentityRole>._)).Returns(IdentityResult.Success);

        var seeder = new DatabaseSeeder(context, userManager, roleManager, logger);

        // Act
        await seeder.SeedAsync();

        // Assert
        var households = await context.Households.ToListAsync();
        Assert.Single(households); // Only the existing one
        Assert.Equal("Existing", households[0].HouseholdName);
    }
}
