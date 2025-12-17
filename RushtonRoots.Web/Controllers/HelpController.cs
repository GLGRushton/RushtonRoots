using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Help/Documentation - provides public help pages and documentation
/// </summary>
public class HelpController : Controller
{
    private readonly ILogger<HelpController> _logger;

    public HelpController(ILogger<HelpController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the help index/landing page with table of contents
    /// </summary>
    /// <returns>Help index view</returns>
    public IActionResult Index()
    {
        ViewData["Title"] = "Help & Documentation";
        return View();
    }

    /// <summary>
    /// Display the getting started guide
    /// </summary>
    /// <returns>Getting started view</returns>
    public IActionResult GettingStarted()
    {
        ViewData["Title"] = "Getting Started";
        return View();
    }

    /// <summary>
    /// Display account management help
    /// </summary>
    /// <returns>Account help view</returns>
    public IActionResult Account()
    {
        ViewData["Title"] = "Account Help";
        return View();
    }

    /// <summary>
    /// Display calendar management help
    /// </summary>
    /// <returns>Calendar help view</returns>
    public IActionResult Calendar()
    {
        ViewData["Title"] = "Calendar Help";
        return View();
    }

    /// <summary>
    /// Display person management help
    /// </summary>
    /// <returns>Person management help view</returns>
    public IActionResult PersonManagement()
    {
        ViewData["Title"] = "Person Management Help";
        return View();
    }

    /// <summary>
    /// Display household management help
    /// </summary>
    /// <returns>Household management help view</returns>
    public IActionResult HouseholdManagement()
    {
        ViewData["Title"] = "Household Management Help";
        return View();
    }

    /// <summary>
    /// Display relationship management help
    /// </summary>
    /// <returns>Relationship management help view</returns>
    public IActionResult RelationshipManagement()
    {
        ViewData["Title"] = "Relationship Management Help";
        return View();
    }

    /// <summary>
    /// Display recipes feature help
    /// </summary>
    /// <returns>Recipes help view</returns>
    public IActionResult Recipes()
    {
        ViewData["Title"] = "Recipes Help";
        return View();
    }

    /// <summary>
    /// Display stories feature help
    /// </summary>
    /// <returns>Stories help view</returns>
    public IActionResult Stories()
    {
        ViewData["Title"] = "Stories Help";
        return View();
    }

    /// <summary>
    /// Display traditions feature help
    /// </summary>
    /// <returns>Traditions help view</returns>
    public IActionResult Traditions()
    {
        ViewData["Title"] = "Traditions Help";
        return View();
    }

    /// <summary>
    /// Display wiki feature help
    /// </summary>
    /// <returns>Wiki help view</returns>
    public IActionResult Wiki()
    {
        ViewData["Title"] = "Wiki Help";
        return View();
    }
}
