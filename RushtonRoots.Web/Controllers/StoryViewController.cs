using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Story views
/// </summary>
[Authorize]
public class StoryViewController : Controller
{
    /// <summary>
    /// Display the stories home page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }
}
