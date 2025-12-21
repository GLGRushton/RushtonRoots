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

        // Helper to parse UK date format (DD/MM/YYYY)
        DateTime ParseUkDate(string ukDate)
        {
            var parts = ukDate.Split('/');
            int day = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int year = int.Parse(parts[2]);
            return new DateTime(year, month, day);
        }

        // ===== Household 1: Sue and Dave =====
        var householdSueAndDave = new Household { HouseholdName = "Sue and Dave" };
        _context.Households.Add(householdSueAndDave);
        await _context.SaveChangesAsync();

        var susan = new Person
        {
            FirstName = "Susan",
            LastName = "Rushton",
            Gender = "Female",
            DateOfBirth = ParseUkDate("10/07/1958"),
            HouseholdId = householdSueAndDave.Id,
            IsDeceased = false
        };
        _context.People.Add(susan);

        var david = new Person
        {
            FirstName = "David",
            LastName = "Rushton",
            Gender = "Male",
            DateOfBirth = ParseUkDate("06/05/1958"),
            HouseholdId = householdSueAndDave.Id,
            IsDeceased = false
        };
        _context.People.Add(david);

        var charlesRushton = new Person
        {
            FirstName = "Charles",
            LastName = "Rushton",
            Gender = "Male",
            DateOfBirth = ParseUkDate("01/05/2001"),
            HouseholdId = householdSueAndDave.Id,
            IsDeceased = false
        };
        _context.People.Add(charlesRushton);

        await _context.SaveChangesAsync();

        // Set Susan as household admin/anchor
        householdSueAndDave.AnchorPersonId = susan.Id;
        
        // Create partnership between Susan and David
        var partnershipSueDave = new Partnership
        {
            PersonAId = susan.Id,
            PersonBId = david.Id,
            PartnershipType = "Married"
        };
        _context.Partnerships.Add(partnershipSueDave);

        await _context.SaveChangesAsync();

        // ===== Household 2: Daniel and Sarah =====
        var householdDanielAndSarah = new Household { HouseholdName = "Daniel and Sarah" };
        _context.Households.Add(householdDanielAndSarah);
        await _context.SaveChangesAsync();

        var daniel = new Person
        {
            FirstName = "Daniel",
            LastName = "Rushton",
            Gender = "Male",
            DateOfBirth = ParseUkDate("10/08/1981"),
            HouseholdId = householdDanielAndSarah.Id,
            IsDeceased = false
        };
        _context.People.Add(daniel);

        var sarahRushton = new Person
        {
            FirstName = "Sarah",
            LastName = "Rushton",
            Gender = "Female",
            DateOfBirth = ParseUkDate("16/06/1981"),
            HouseholdId = householdDanielAndSarah.Id,
            IsDeceased = false
        };
        _context.People.Add(sarahRushton);

        await _context.SaveChangesAsync();

        householdDanielAndSarah.AnchorPersonId = daniel.Id;

        var partnershipDanielSarah = new Partnership
        {
            PersonAId = daniel.Id,
            PersonBId = sarahRushton.Id,
            PartnershipType = "Married"
        };
        _context.Partnerships.Add(partnershipDanielSarah);

        // Daniel is child of Susan and David
        var parentChildSusanDaniel = new ParentChild
        {
            ParentPersonId = susan.Id,
            ChildPersonId = daniel.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSusanDaniel);

        var parentChildDavidDaniel = new ParentChild
        {
            ParentPersonId = david.Id,
            ChildPersonId = daniel.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildDavidDaniel);

        await _context.SaveChangesAsync();

        // ===== Household 3: Robert and Laura =====
        var householdRobertAndLaura = new Household { HouseholdName = "Robert and Laura" };
        _context.Households.Add(householdRobertAndLaura);
        await _context.SaveChangesAsync();

        var robert = new Person
        {
            FirstName = "Robert",
            LastName = "Rushton",
            Gender = "Male",
            DateOfBirth = ParseUkDate("21/09/1982"),
            HouseholdId = householdRobertAndLaura.Id,
            IsDeceased = false
        };
        _context.People.Add(robert);

        var laura = new Person
        {
            FirstName = "Laura",
            LastName = "Rushton",
            Gender = "Female",
            DateOfBirth = ParseUkDate("29/11/1981"),
            HouseholdId = householdRobertAndLaura.Id,
            IsDeceased = false
        };
        _context.People.Add(laura);

        var lilly = new Person
        {
            FirstName = "Lilly",
            LastName = "Rushton",
            Gender = "Female",
            DateOfBirth = ParseUkDate("18/07/2014"),
            HouseholdId = householdRobertAndLaura.Id,
            IsDeceased = false
        };
        _context.People.Add(lilly);

        var isla = new Person
        {
            FirstName = "Isla",
            LastName = "Rushton",
            Gender = "Female",
            DateOfBirth = ParseUkDate("18/07/2014"),
            HouseholdId = householdRobertAndLaura.Id,
            IsDeceased = false
        };
        _context.People.Add(isla);

        await _context.SaveChangesAsync();

        householdRobertAndLaura.AnchorPersonId = robert.Id;

        var partnershipRobertLaura = new Partnership
        {
            PersonAId = robert.Id,
            PersonBId = laura.Id,
            PartnershipType = "Married"
        };
        _context.Partnerships.Add(partnershipRobertLaura);

        // Robert is child of Susan and David
        var parentChildSusanRobert = new ParentChild
        {
            ParentPersonId = susan.Id,
            ChildPersonId = robert.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSusanRobert);

        var parentChildDavidRobert = new ParentChild
        {
            ParentPersonId = david.Id,
            ChildPersonId = robert.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildDavidRobert);

        // Lilly and Isla are children of Robert and Laura
        var parentChildRobertLilly = new ParentChild
        {
            ParentPersonId = robert.Id,
            ChildPersonId = lilly.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildRobertLilly);

        var parentChildLauraLilly = new ParentChild
        {
            ParentPersonId = laura.Id,
            ChildPersonId = lilly.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildLauraLilly);

        var parentChildRobertIsla = new ParentChild
        {
            ParentPersonId = robert.Id,
            ChildPersonId = isla.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildRobertIsla);

        var parentChildLauraIsla = new ParentChild
        {
            ParentPersonId = laura.Id,
            ChildPersonId = isla.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildLauraIsla);

        await _context.SaveChangesAsync();

        // ===== Household 4: Sarah and Colin =====
        var householdSarahAndColin = new Household { HouseholdName = "Sarah and Colin" };
        _context.Households.Add(householdSarahAndColin);
        await _context.SaveChangesAsync();

        var sarahMarnane = new Person
        {
            FirstName = "Sarah",
            LastName = "Marnane",
            Gender = "Female",
            DateOfBirth = ParseUkDate("28/05/1987"),
            HouseholdId = householdSarahAndColin.Id,
            IsDeceased = false
        };
        _context.People.Add(sarahMarnane);

        var colin = new Person
        {
            FirstName = "Colin",
            LastName = "Marnane",
            Gender = "Male",
            DateOfBirth = ParseUkDate("22/04/1986"),
            HouseholdId = householdSarahAndColin.Id,
            IsDeceased = false
        };
        _context.People.Add(colin);

        var cian = new Person
        {
            FirstName = "Cian",
            LastName = "Marnane",
            Gender = "Male",
            DateOfBirth = ParseUkDate("14/07/2012"),
            HouseholdId = householdSarahAndColin.Id,
            IsDeceased = false
        };
        _context.People.Add(cian);

        var niamh = new Person
        {
            FirstName = "Niamh",
            LastName = "Marnane",
            Gender = "Female",
            DateOfBirth = ParseUkDate("19/03/2014"),
            HouseholdId = householdSarahAndColin.Id,
            IsDeceased = false
        };
        _context.People.Add(niamh);

        var aoife = new Person
        {
            FirstName = "Aoife",
            LastName = "Marnane",
            Gender = "Female",
            DateOfBirth = ParseUkDate("07/07/2018"),
            HouseholdId = householdSarahAndColin.Id,
            IsDeceased = false
        };
        _context.People.Add(aoife);

        await _context.SaveChangesAsync();

        householdSarahAndColin.AnchorPersonId = sarahMarnane.Id;

        var partnershipSarahColin = new Partnership
        {
            PersonAId = sarahMarnane.Id,
            PersonBId = colin.Id,
            PartnershipType = "Married"
        };
        _context.Partnerships.Add(partnershipSarahColin);

        // Sarah Marnane is child of Susan and David
        var parentChildSusanSarahM = new ParentChild
        {
            ParentPersonId = susan.Id,
            ChildPersonId = sarahMarnane.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSusanSarahM);

        var parentChildDavidSarahM = new ParentChild
        {
            ParentPersonId = david.Id,
            ChildPersonId = sarahMarnane.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildDavidSarahM);

        // Cian, Niamh, and Aoife are children of Colin and Sarah Marnane
        var parentChildColinCian = new ParentChild
        {
            ParentPersonId = colin.Id,
            ChildPersonId = cian.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildColinCian);

        var parentChildSarahMCian = new ParentChild
        {
            ParentPersonId = sarahMarnane.Id,
            ChildPersonId = cian.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSarahMCian);

        var parentChildColinNiamh = new ParentChild
        {
            ParentPersonId = colin.Id,
            ChildPersonId = niamh.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildColinNiamh);

        var parentChildSarahMNiamh = new ParentChild
        {
            ParentPersonId = sarahMarnane.Id,
            ChildPersonId = niamh.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSarahMNiamh);

        var parentChildColinAoife = new ParentChild
        {
            ParentPersonId = colin.Id,
            ChildPersonId = aoife.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildColinAoife);

        var parentChildSarahMAoife = new ParentChild
        {
            ParentPersonId = sarahMarnane.Id,
            ChildPersonId = aoife.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSarahMAoife);

        await _context.SaveChangesAsync();

        // ===== Household 5: Anne and Paul =====
        var householdAnneAndPaul = new Household { HouseholdName = "Anne and Paul" };
        _context.Households.Add(householdAnneAndPaul);
        await _context.SaveChangesAsync();

        var anne = new Person
        {
            FirstName = "Anne",
            LastName = "Kurlej",
            Gender = "Female",
            DateOfBirth = ParseUkDate("17/08/1988"),
            HouseholdId = householdAnneAndPaul.Id,
            IsDeceased = false
        };
        _context.People.Add(anne);

        var paul = new Person
        {
            FirstName = "Paul",
            LastName = "Kurlej",
            Gender = "Male",
            DateOfBirth = ParseUkDate("11/02/1986"),
            HouseholdId = householdAnneAndPaul.Id,
            IsDeceased = false
        };
        _context.People.Add(paul);

        var william = new Person
        {
            FirstName = "William",
            LastName = "Kurlej",
            Gender = "Male",
            DateOfBirth = ParseUkDate("05/01/2014"),
            HouseholdId = householdAnneAndPaul.Id,
            IsDeceased = false
        };
        _context.People.Add(william);

        var darcie = new Person
        {
            FirstName = "Darcie",
            LastName = "Kurlej",
            Gender = "Female",
            DateOfBirth = ParseUkDate("11/08/2017"),
            HouseholdId = householdAnneAndPaul.Id,
            IsDeceased = false
        };
        _context.People.Add(darcie);

        await _context.SaveChangesAsync();

        householdAnneAndPaul.AnchorPersonId = anne.Id;

        var partnershipAnnePaul = new Partnership
        {
            PersonAId = anne.Id,
            PersonBId = paul.Id,
            PartnershipType = "Married"
        };
        _context.Partnerships.Add(partnershipAnnePaul);

        // Anne is child of Susan and David
        var parentChildSusanAnne = new ParentChild
        {
            ParentPersonId = susan.Id,
            ChildPersonId = anne.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSusanAnne);

        var parentChildDavidAnne = new ParentChild
        {
            ParentPersonId = david.Id,
            ChildPersonId = anne.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildDavidAnne);

        // William and Darcie are children of Anne and Paul
        var parentChildAnneWilliam = new ParentChild
        {
            ParentPersonId = anne.Id,
            ChildPersonId = william.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildAnneWilliam);

        var parentChildPaulWilliam = new ParentChild
        {
            ParentPersonId = paul.Id,
            ChildPersonId = william.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildPaulWilliam);

        var parentChildAnneDarcie = new ParentChild
        {
            ParentPersonId = anne.Id,
            ChildPersonId = darcie.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildAnneDarcie);

        var parentChildPaulDarcie = new ParentChild
        {
            ParentPersonId = paul.Id,
            ChildPersonId = darcie.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildPaulDarcie);

        await _context.SaveChangesAsync();

        // ===== Household 6: George and Helen =====
        var householdGeorgeAndHelen = new Household { HouseholdName = "George and Helen" };
        _context.Households.Add(householdGeorgeAndHelen);
        await _context.SaveChangesAsync();

        var george = new Person
        {
            FirstName = "George",
            LastName = "Rushton",
            Gender = "Male",
            DateOfBirth = ParseUkDate("10/06/1994"),
            HouseholdId = householdGeorgeAndHelen.Id,
            IsDeceased = false
        };
        _context.People.Add(george);

        var helen = new Person
        {
            FirstName = "Helen",
            LastName = "Ashton",
            Gender = "Female",
            DateOfBirth = ParseUkDate("27/04/1996"),
            HouseholdId = householdGeorgeAndHelen.Id,
            IsDeceased = false
        };
        _context.People.Add(helen);

        await _context.SaveChangesAsync();

        householdGeorgeAndHelen.AnchorPersonId = george.Id;

        var partnershipGeorgeHelen = new Partnership
        {
            PersonAId = george.Id,
            PersonBId = helen.Id,
            PartnershipType = "Married"
        };
        _context.Partnerships.Add(partnershipGeorgeHelen);

        // George is child of Susan and David
        var parentChildSusanGeorge = new ParentChild
        {
            ParentPersonId = susan.Id,
            ChildPersonId = george.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSusanGeorge);

        var parentChildDavidGeorge = new ParentChild
        {
            ParentPersonId = david.Id,
            ChildPersonId = george.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildDavidGeorge);

        await _context.SaveChangesAsync();

        // ===== Household 7: Catherine and Dominic =====
        var householdCatherineAndDominic = new Household { HouseholdName = "Catherine and Dominic" };
        _context.Households.Add(householdCatherineAndDominic);
        await _context.SaveChangesAsync();

        var catherine = new Person
        {
            FirstName = "Catherine",
            LastName = "Gayler",
            Gender = "Female",
            DateOfBirth = ParseUkDate("11/10/1997"),
            HouseholdId = householdCatherineAndDominic.Id,
            IsDeceased = false
        };
        _context.People.Add(catherine);

        var dominic = new Person
        {
            FirstName = "Dominic",
            LastName = "Gayler",
            Gender = "Male",
            DateOfBirth = ParseUkDate("21/10/1997"),
            HouseholdId = householdCatherineAndDominic.Id,
            IsDeceased = false
        };
        _context.People.Add(dominic);

        await _context.SaveChangesAsync();

        householdCatherineAndDominic.AnchorPersonId = catherine.Id;

        var partnershipCatherineDominic = new Partnership
        {
            PersonAId = catherine.Id,
            PersonBId = dominic.Id,
            PartnershipType = "Married"
        };
        _context.Partnerships.Add(partnershipCatherineDominic);

        // Catherine is child of Susan and David
        var parentChildSusanCatherine = new ParentChild
        {
            ParentPersonId = susan.Id,
            ChildPersonId = catherine.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSusanCatherine);

        var parentChildDavidCatherine = new ParentChild
        {
            ParentPersonId = david.Id,
            ChildPersonId = catherine.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildDavidCatherine);

        // Charles is also child of Susan and David (already created in household 1)
        var parentChildSusanCharles = new ParentChild
        {
            ParentPersonId = susan.Id,
            ChildPersonId = charlesRushton.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildSusanCharles);

        var parentChildDavidCharles = new ParentChild
        {
            ParentPersonId = david.Id,
            ChildPersonId = charlesRushton.Id,
            RelationshipType = "Biological"
        };
        _context.ParentChildren.Add(parentChildDavidCharles);

        await _context.SaveChangesAsync();

        // Create initial admin user linked to Susan
        var adminEmail = "admin@rushtonroots.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PersonId = susan.Id
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
                    HouseholdId = householdSueAndDave.Id,
                    PersonId = susan.Id,
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
