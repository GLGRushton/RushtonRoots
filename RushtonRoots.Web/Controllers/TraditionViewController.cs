using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for Tradition views
/// </summary>
[Authorize]
public class TraditionViewController : Controller
{
    private readonly ITraditionService _traditionService;

    public TraditionViewController(ITraditionService traditionService)
    {
        _traditionService = traditionService;
    }

    public IActionResult Index()
    {
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
