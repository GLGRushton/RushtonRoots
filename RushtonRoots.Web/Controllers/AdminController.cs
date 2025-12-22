using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Admin functionality - provides system administration views
/// </summary>
/// <remarks>
/// ViewData Contract for Dashboard view:
/// - TotalUsers: Total count of application users
/// - TotalHouseholds: Total count of households
/// - TotalPersons: Total count of people
/// - MediaItems: Total count of media items (placeholder)
/// - RecentActivity: List of RecentActivity objects with ActivityType, Description, UserName, CreatedDateTime, ActionUrl
/// </remarks>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly IAdminDashboardService _adminDashboardService;

    public AdminController(
        ILogger<AdminController> logger,
        IAdminDashboardService adminDashboardService)
    {
        _logger = logger;
        _adminDashboardService = adminDashboardService;
    }

    /// <summary>
    /// Display the admin dashboard with system overview and statistics
    /// </summary>
    /// <returns>Admin dashboard view</returns>
    public async Task<IActionResult> Dashboard()
    {
        ViewData["Title"] = "Admin Dashboard";
        
        var stats = await _adminDashboardService.GetSystemStatisticsAsync();
        ViewData["TotalUsers"] = stats.TotalUsers;
        ViewData["TotalHouseholds"] = stats.TotalHouseholds;
        ViewData["TotalPersons"] = stats.TotalPersons;
        ViewData["MediaItems"] = stats.MediaItems;
        ViewData["RecentActivity"] = await _adminDashboardService.GetRecentActivityAsync();
        
        return View();
    }

    /// <summary>
    /// Display the system settings page for configuration management
    /// </summary>
    /// <returns>System settings view</returns>
    public IActionResult Settings()
    {
        ViewData["Title"] = "System Settings";
        
        // Optional: Add current settings to ViewData
        // ViewData["CurrentSettings"] = await _settingsService.GetSystemSettingsAsync();
        
        return View();
    }
}
