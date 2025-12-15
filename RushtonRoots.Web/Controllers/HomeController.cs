using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Home";
        return View();
    }

    public IActionResult StyleGuide()
    {
        ViewData["Title"] = "Style Guide";
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
