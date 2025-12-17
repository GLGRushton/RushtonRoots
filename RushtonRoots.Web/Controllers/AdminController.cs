using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Admin functionality - provides system administration views
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the admin dashboard with system overview and statistics
    /// </summary>
    /// <returns>Admin dashboard view</returns>
    public IActionResult Dashboard()
    {
        ViewData["Title"] = "Admin Dashboard";
        
        // Optional: Add system statistics to ViewData
        // ViewData["TotalUsers"] = await _userService.GetUserCountAsync();
        // ViewData["TotalHouseholds"] = await _householdService.GetHouseholdCountAsync();
        // ViewData["TotalPersons"] = await _personService.GetPersonCountAsync();
        
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
