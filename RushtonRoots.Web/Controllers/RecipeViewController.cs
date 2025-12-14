using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Recipe views
/// </summary>
[Authorize]
public class RecipeViewController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
