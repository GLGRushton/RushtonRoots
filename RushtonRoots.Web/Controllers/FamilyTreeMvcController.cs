using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for the Family Tree - provides interactive family tree visualization views
/// </summary>
[Authorize]
public class FamilyTreeMvcController : Controller
{
    private readonly ILogger<FamilyTreeMvcController> _logger;

    public FamilyTreeMvcController(ILogger<FamilyTreeMvcController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the interactive family tree visualization
    /// </summary>
    /// <param name="personId">Optional focus on specific person (default: root person)</param>
    /// <param name="view">Optional view mode (descendant, pedigree, fan - default: descendant)</param>
    /// <param name="generations">Number of generations to display (default: 3)</param>
    /// <returns>Family tree view</returns>
    [Route("FamilyTree")]
    public IActionResult Index(int? personId = null, string? view = null, int generations = 3)
    {
        ViewData["Title"] = "Family Tree";
        ViewData["PersonId"] = personId;
        ViewData["ViewMode"] = view ?? "descendant";
        ViewData["Generations"] = generations;
        
        return View();
    }
}
