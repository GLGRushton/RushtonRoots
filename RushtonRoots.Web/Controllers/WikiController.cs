using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

[Authorize]
public class WikiController : Controller
{
    /// <summary>
    /// Display the wiki home page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }
}
