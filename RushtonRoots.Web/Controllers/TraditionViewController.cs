using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Tradition views
/// </summary>
[Authorize]
public class TraditionViewController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
