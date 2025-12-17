using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for the Family Calendar - provides calendar and event management views
/// </summary>
[Authorize]
public class CalendarController : Controller
{
    private readonly ILogger<CalendarController> _logger;

    public CalendarController(ILogger<CalendarController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the family calendar with events, birthdays, and anniversaries
    /// </summary>
    /// <param name="view">Optional calendar view mode (month, week, day, list - default: month)</param>
    /// <param name="date">Optional date to focus on (default: today)</param>
    /// <returns>Calendar view</returns>
    public IActionResult Index(string? view = null, DateTime? date = null)
    {
        ViewData["Title"] = "Family Calendar";
        ViewData["ViewMode"] = view ?? "dayGridMonth"; // FullCalendar view name
        ViewData["InitialDate"] = date?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
        
        // Determine if user can edit events
        var canEdit = User?.IsInRole("Admin") == true || User?.IsInRole("HouseholdAdmin") == true;
        ViewData["CanEdit"] = canEdit;
        
        return View();
    }

    /// <summary>
    /// Display the event creation page
    /// </summary>
    /// <param name="date">Optional date for the new event (default: today)</param>
    /// <param name="startTime">Optional start time for the event</param>
    /// <param name="endTime">Optional end time for the event</param>
    /// <returns>Create event view</returns>
    public IActionResult Create(DateTime? date = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        ViewData["Title"] = "Create Event";
        ViewData["EventDate"] = date?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
        ViewData["StartTime"] = startTime?.ToString("yyyy-MM-ddTHH:mm") ?? "";
        ViewData["EndTime"] = endTime?.ToString("yyyy-MM-ddTHH:mm") ?? "";
        
        return View();
    }
}
