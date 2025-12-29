using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for home page
/// </summary>
/// <remarks>
/// ViewBag Contract for Index view:
/// - TotalMembers: Total count of non-deleted people
/// - TotalPhotos: Total count of media items (placeholder)
/// - TotalStories: Total count of published stories
/// - ActiveHouseholds: Total count of households
/// - OldestAncestor: PersonSummary object with id, fullName, photoUrl, birthDate, deathDate, age
/// - NewestMember: PersonSummary object with id, fullName, photoUrl, birthDate, deathDate, age
/// - RecentAdditions: List of recently added people with person object and metadata
/// - UpcomingBirthdays: List of upcoming birthdays transformed to UpcomingEvent interface
/// - UpcomingAnniversaries: List of upcoming anniversaries transformed to UpcomingEvent interface
/// - ActivityFeed: List of recent activities with user information, icon, color, and title
/// </remarks>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomePageService _homePageService;
    private readonly RushtonRootsDbContext _context;

    public HomeController(ILogger<HomeController> logger, IHomePageService homePageService, RushtonRootsDbContext context)
    {
        _logger = logger;
        _homePageService = homePageService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Home";
        
        var statistics = await _homePageService.GetStatisticsAsync();
        
        // Transform statistics to match frontend interface
        ViewBag.TotalMembers = statistics.TotalMembers;
        ViewBag.TotalPhotos = statistics.TotalPhotos;
        ViewBag.TotalStories = statistics.TotalStories;
        ViewBag.ActiveHouseholds = statistics.ActiveHouseholds;
        
        // Transform OldestAncestor and NewestMember to PersonSummary objects
        ViewBag.OldestAncestor = await GetPersonSummaryByNameAsync(statistics.OldestAncestor, isOldest: true);
        ViewBag.NewestMember = await GetPersonSummaryByNameAsync(statistics.NewestMember, isOldest: false);
        
        // Transform RecentAdditions to include person object with fullName
        var recentAdditions = await _homePageService.GetRecentAdditionsAsync();
        ViewBag.RecentAdditions = recentAdditions.Select(ra => new
        {
            person = new
            {
                id = ra.PersonId,
                fullName = $"{ra.FirstName} {ra.LastName}",
                photoUrl = ra.PhotoUrl
            },
            addedDate = ra.AddedDate,
            addedBy = "System" // Could be enhanced with actual user info
        }).ToList();
        
        // Transform UpcomingBirthdays to match UpcomingEvent interface
        var upcomingBirthdays = await _homePageService.GetUpcomingBirthdaysAsync();
        ViewBag.UpcomingBirthdays = upcomingBirthdays.Select(ub => new
        {
            id = ub.PersonId,
            personId = ub.PersonId,
            personName = $"{ub.FirstName} {ub.LastName}",
            photoUrl = ub.PhotoUrl,
            eventType = "birthday",
            eventDate = ub.NextBirthday,
            daysUntil = ub.DaysUntilBirthday,
            description = $"Turning {ub.Age}"
        }).ToList();
        
        // Transform UpcomingAnniversaries to match UpcomingEvent interface
        var upcomingAnniversaries = await _homePageService.GetUpcomingAnniversariesAsync();
        ViewBag.UpcomingAnniversaries = upcomingAnniversaries.Select(ua => new
        {
            id = ua.PartnershipId,
            personId = ua.PartnershipId, // Using partnership ID since it's a couple event
            personName = $"{ua.PersonAName} & {ua.PersonBName}",
            photoUrl = ua.PersonAPhotoUrl, // Show first person's photo
            eventType = "anniversary",
            eventDate = ua.NextAnniversary,
            daysUntil = ua.DaysUntilAnniversary,
            description = $"{ua.YearsMarried} years"
        }).ToList();
        
        // Transform ActivityFeed to include icon, color, and title
        var activityFeed = await _homePageService.GetActivityFeedAsync();
        var activityFeedWithUsers = new List<object>();
        
        foreach (var item in activityFeed)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == item.UserId);
            
            var userName = "Unknown User";
            if (user?.Person != null)
            {
                userName = $"{user.Person.FirstName} {user.Person.LastName}".Trim();
            }
            else if (user != null)
            {
                userName = user.UserName ?? "Unknown User";
            }
            
            activityFeedWithUsers.Add(new
            {
                id = item.Id.ToString(),
                type = item.ActivityType,
                icon = GetActivityIcon(item.ActivityType),
                color = GetActivityColor(item.ActivityType),
                title = GetActivityTitle(item.ActivityType),
                description = item.Description,
                timestamp = item.CreatedDateTime,
                userName = userName,
                userPhotoUrl = user?.Person?.PhotoUrl,
                relatedUrl = item.ActionUrl
            });
        }
        
        ViewBag.ActivityFeed = activityFeedWithUsers;
        
        return View();
    }
    
    private async Task<object?> GetPersonSummaryByNameAsync(string? fullName, bool isOldest)
    {
        if (string.IsNullOrEmpty(fullName))
            return null;
        
        var person = isOldest
            ? await _context.People
                .Where(p => !p.IsDeleted && p.DateOfBirth.HasValue)
                .OrderBy(p => p.DateOfBirth)
                .FirstOrDefaultAsync()
            : await _context.People
                .Where(p => !p.IsDeleted && p.DateOfBirth.HasValue)
                .OrderByDescending(p => p.DateOfBirth)
                .FirstOrDefaultAsync();
        
        if (person == null)
            return null;
        
        return new
        {
            id = person.Id,
            fullName = $"{person.FirstName} {person.LastName}",
            photoUrl = person.PhotoUrl,
            birthDate = person.DateOfBirth?.ToString("yyyy-MM-dd"),
            deathDate = person.DateOfDeath?.ToString("yyyy-MM-dd"),
            age = person.DateOfBirth.HasValue 
                ? (person.DateOfDeath ?? DateTime.Today).Year - person.DateOfBirth.Value.Year 
                : (int?)null
        };
    }
    
    private string GetActivityIcon(string activityType)
    {
        return activityType switch
        {
            "member_added" => "person_add",
            "photo_uploaded" => "photo_camera",
            "story_published" => "menu_book",
            "comment_posted" => "comment",
            _ => "notifications"
        };
    }
    
    private string GetActivityColor(string activityType)
    {
        return activityType switch
        {
            "member_added" => "#4caf50",
            "photo_uploaded" => "#2196f3",
            "story_published" => "#ff9800",
            "comment_posted" => "#9c27b0",
            _ => "#757575"
        };
    }
    
    private string GetActivityTitle(string activityType)
    {
        return activityType switch
        {
            "member_added" => "New Member Added",
            "photo_uploaded" => "Photo Uploaded",
            "story_published" => "Story Published",
            "comment_posted" => "Comment Posted",
            _ => "Activity"
        };
    }

    public IActionResult StyleGuide()
    {
        ViewData["Title"] = "Style Guide";
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
