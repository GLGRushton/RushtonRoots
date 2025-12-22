using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Tradition views
/// </summary>
/// <remarks>
/// ViewBag Contract for Index view:
/// - Traditions: List of all published traditions (for noscript fallback)
/// - Categories: List of available tradition categories
/// - FeaturedTraditions: List of featured traditions
/// - RecentTraditions: List of recently published traditions
/// - CanEdit: Boolean indicating if current user can edit traditions (Admin or HouseholdAdmin role)
/// </remarks>
[Authorize]
public class TraditionViewController : Controller
{
    private readonly ITraditionService _traditionService;

    public TraditionViewController(ITraditionService traditionService)
    {
        _traditionService = traditionService;
    }

    /// <summary>
    /// Display the traditions home page
    /// </summary>
    /// <remarks>
    /// The Angular component fetches data from API endpoints.
    /// ViewBag data is populated for noscript fallback scenarios.
    /// </remarks>
    public async Task<IActionResult> Index()
    {
        // Populate ViewBag for noscript fallback
        ViewBag.Traditions = await _traditionService.GetAllAsync(publishedOnly: true);
        ViewBag.Categories = new List<string>(); // TODO: Implement category service if needed
        ViewBag.FeaturedTraditions = new List<object>(); // TODO: Implement featured traditions if needed
        ViewBag.RecentTraditions = new List<object>(); // TODO: Implement recent traditions if needed
        ViewBag.CanEdit = User.IsInRole("Admin") || User.IsInRole("HouseholdAdmin");
        
        return View();
    }

    /// <summary>
    /// Display details of a specific tradition
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var tradition = await _traditionService.GetByIdAsync(id, incrementViewCount: true);
        if (tradition == null)
        {
            return NotFound();
        }
        return View(tradition);
    }
}
