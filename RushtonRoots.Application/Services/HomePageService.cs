using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service implementation for home page data operations.
/// </summary>
public class HomePageService : IHomePageService
{
    private readonly RushtonRootsDbContext _context;

    public HomePageService(RushtonRootsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<HomePageStatistics> GetStatisticsAsync()
    {
        var statistics = new HomePageStatistics
        {
            TotalMembers = await _context.People.CountAsync(p => !p.IsDeleted),
            TotalPhotos = await _context.PersonPhotos.CountAsync(),
            TotalStories = await _context.Stories.CountAsync(s => s.IsPublished),
            ActiveHouseholds = await _context.Households.CountAsync()
        };

        // Get oldest ancestor (person with earliest birth date)
        var oldestPerson = await _context.People
            .Where(p => !p.IsDeleted && p.DateOfBirth.HasValue)
            .OrderBy(p => p.DateOfBirth)
            .FirstOrDefaultAsync();

        if (oldestPerson != null)
        {
            statistics.OldestAncestor = $"{oldestPerson.FirstName} {oldestPerson.LastName}";
        }

        // Get newest member (most recently added person)
        var newestPerson = await _context.People
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedDateTime)
            .FirstOrDefaultAsync();

        if (newestPerson != null)
        {
            statistics.NewestMember = $"{newestPerson.FirstName} {newestPerson.LastName}";
        }

        return statistics;
    }

    /// <inheritdoc />
    public async Task<List<RecentAddition>> GetRecentAdditionsAsync(int count = 5)
    {
        var recentPeople = await _context.People
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedDateTime)
            .Take(count)
            .Select(p => new RecentAddition
            {
                PersonId = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                PhotoUrl = p.PhotoUrl,
                AddedDate = p.CreatedDateTime,
                RelationshipDescription = null // Could be enhanced later
            })
            .ToListAsync();

        return recentPeople;
    }

    /// <inheritdoc />
    public async Task<List<UpcomingBirthday>> GetUpcomingBirthdaysAsync(int days = 30)
    {
        var today = DateTime.Today;
        var endDate = today.AddDays(days);

        var peopleWithBirthdays = await _context.People
            .Where(p => !p.IsDeleted && !p.IsDeceased && p.DateOfBirth.HasValue)
            .ToListAsync();

        var upcomingBirthdays = new List<UpcomingBirthday>();

        foreach (var person in peopleWithBirthdays)
        {
            if (person.DateOfBirth == null) continue;

            var birthDate = person.DateOfBirth.Value;
            var thisYearBirthday = new DateTime(today.Year, birthDate.Month, birthDate.Day);
            
            // If birthday already passed this year, check next year
            if (thisYearBirthday < today)
            {
                thisYearBirthday = thisYearBirthday.AddYears(1);
            }

            var daysUntil = (thisYearBirthday - today).Days;

            if (daysUntil <= days)
            {
                // Calculate the age they will turn on their upcoming birthday
                var age = thisYearBirthday.Year - birthDate.Year;

                upcomingBirthdays.Add(new UpcomingBirthday
                {
                    PersonId = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    PhotoUrl = person.PhotoUrl,
                    BirthDate = birthDate,
                    NextBirthday = thisYearBirthday,
                    Age = age,
                    DaysUntilBirthday = daysUntil
                });
            }
        }

        return upcomingBirthdays
            .OrderBy(b => b.DaysUntilBirthday)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<UpcomingAnniversary>> GetUpcomingAnniversariesAsync(int days = 30)
    {
        var today = DateTime.Today;
        var endDate = today.AddDays(days);

        var partnerships = await _context.Partnerships
            .Where(p => !p.IsDeleted && p.StartDate.HasValue && !p.EndDate.HasValue)
            .Include(p => p.PersonA)
            .Include(p => p.PersonB)
            .ToListAsync();

        var upcomingAnniversaries = new List<UpcomingAnniversary>();

        foreach (var partnership in partnerships)
        {
            if (partnership.StartDate == null || partnership.PersonA == null || partnership.PersonB == null)
                continue;

            var startDate = partnership.StartDate.Value;
            var thisYearAnniversary = new DateTime(today.Year, startDate.Month, startDate.Day);
            
            // If anniversary already passed this year, check next year
            if (thisYearAnniversary < today)
            {
                thisYearAnniversary = thisYearAnniversary.AddYears(1);
            }

            var daysUntil = (thisYearAnniversary - today).Days;

            if (daysUntil <= days)
            {
                // Calculate the number of years they'll have been married on their upcoming anniversary
                var yearsMarried = thisYearAnniversary.Year - startDate.Year;

                upcomingAnniversaries.Add(new UpcomingAnniversary
                {
                    PartnershipId = partnership.Id,
                    PersonAName = $"{partnership.PersonA.FirstName} {partnership.PersonA.LastName}",
                    PersonBName = $"{partnership.PersonB.FirstName} {partnership.PersonB.LastName}",
                    PersonAPhotoUrl = partnership.PersonA.PhotoUrl,
                    PersonBPhotoUrl = partnership.PersonB.PhotoUrl,
                    StartDate = startDate,
                    NextAnniversary = thisYearAnniversary,
                    YearsMarried = yearsMarried,
                    DaysUntilAnniversary = daysUntil
                });
            }
        }

        return upcomingAnniversaries
            .OrderBy(a => a.DaysUntilAnniversary)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<ActivityFeedItemViewModel>> GetActivityFeedAsync(int count = 20)
    {
        var activityItems = await _context.ActivityFeedItems
            .Where(a => a.IsPublic)
            .OrderByDescending(a => a.CreatedDateTime)
            .Take(count)
            .Select(a => new ActivityFeedItemViewModel
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = string.Empty, // Will be populated from User if needed
                ActivityType = a.ActivityType,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Description = a.Description,
                ActionUrl = a.ActionUrl,
                Points = a.Points,
                IsPublic = a.IsPublic,
                CreatedDateTime = a.CreatedDateTime
            })
            .ToListAsync();

        // If we need username, load it separately
        if (activityItems.Any())
        {
            var userIds = activityItems.Select(a => a.UserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? string.Empty);

            foreach (var item in activityItems)
            {
                if (users.TryGetValue(item.UserId, out var username))
                {
                    item.UserName = username;
                }
            }
        }

        return activityItems;
    }
}
