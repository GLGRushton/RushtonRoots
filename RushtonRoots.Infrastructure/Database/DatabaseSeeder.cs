using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database;

/// <summary>
/// Seeds the database with initial data including roles and default users.
/// </summary>
public class DatabaseSeeder
{
    private readonly RushtonRootsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        RushtonRootsDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedInitialDataAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roles = { "Admin", "HouseholdAdmin", "FamilyMember" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                await _roleManager.CreateAsync(role);
                _logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }

    private async Task SeedInitialDataAsync()
    {
        // Check if we already have data
        if (await _context.Households.AnyAsync())
        {
            _logger.LogInformation("Database already seeded. Skipping initial data seed.");
            return;
        }

        // Create initial household
        var household = new Household
        {
            HouseholdName = "Rushton Family"
        };

        // Create anchor person (will be updated after person is created)
        var anchorPerson = new Person
        {
            FirstName = "George",
            LastName = "Rushton",
            DateOfBirth = new DateTime(1950, 1, 1),
            IsDeceased = false,
            Household = household
        };

        _context.People.Add(anchorPerson);
        await _context.SaveChangesAsync();

        // Update household with anchor person
        household.AnchorPersonId = anchorPerson.Id;
        await _context.SaveChangesAsync();

        // Create initial admin user
        var adminEmail = "admin@rushtonroots.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PersonId = anchorPerson.Id
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                await _userManager.AddToRoleAsync(adminUser, "HouseholdAdmin");
                _logger.LogInformation("Created initial admin user: {Email}", adminEmail);

                // Add household permission for admin
                var permission = new HouseholdPermission
                {
                    HouseholdId = household.Id,
                    PersonId = anchorPerson.Id,
                    Role = "ADMIN"
                };
                _context.HouseholdPermissions.Add(permission);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogError("Failed to create admin user: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        _logger.LogInformation("Database seeding completed successfully");
    }
}
