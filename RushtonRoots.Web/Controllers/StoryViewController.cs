using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Story views
/// </summary>
/// <remarks>
/// ViewBag Contract for Index view:
/// - Stories: List of all published stories (for noscript fallback)
/// - Categories: List of available story categories
/// - FeaturedStories: List of featured stories
/// - RecentStories: List of recently published stories
/// - CanEdit: Boolean indicating if current user can edit stories (Admin or HouseholdAdmin role)
/// </remarks>
[Authorize]
public class StoryViewController : Controller
{
    private readonly IStoryService _storyService;

    public StoryViewController(IStoryService storyService)
    {
        _storyService = storyService;
    }

    /// <summary>
    /// Display the stories home page
    /// </summary>
    /// <remarks>
    /// The Angular component fetches data from API endpoints.
    /// ViewBag data is populated for noscript fallback scenarios.
    /// </remarks>
    public async Task<IActionResult> Index()
    {
        // Populate ViewBag for noscript fallback
        ViewBag.Stories = await _storyService.GetAllAsync(publishedOnly: true);
        ViewBag.Categories = new List<string>(); // TODO: Implement category service if needed
        ViewBag.FeaturedStories = new List<object>(); // TODO: Implement featured stories if needed
        ViewBag.RecentStories = new List<object>(); // TODO: Implement recent stories if needed
        ViewBag.CanEdit = User.IsInRole("Admin") || User.IsInRole("HouseholdAdmin");
        
        return View();
    }
}
