using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;

namespace RushtonRoots.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomePageService _homePageService;

    public HomeController(ILogger<HomeController> logger, IHomePageService homePageService)
    {
        _logger = logger;
        _homePageService = homePageService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Home";
        
        var statistics = await _homePageService.GetStatisticsAsync();
        ViewBag.TotalMembers = statistics.TotalMembers;
        ViewBag.OldestAncestor = statistics.OldestAncestor;
        ViewBag.NewestMember = statistics.NewestMember;
        ViewBag.TotalPhotos = statistics.TotalPhotos;
        ViewBag.TotalStories = statistics.TotalStories;
        ViewBag.ActiveHouseholds = statistics.ActiveHouseholds;
        
        ViewBag.RecentAdditions = await _homePageService.GetRecentAdditionsAsync();
        ViewBag.UpcomingBirthdays = await _homePageService.GetUpcomingBirthdaysAsync();
        ViewBag.UpcomingAnniversaries = await _homePageService.GetUpcomingAnniversariesAsync();
        ViewBag.ActivityFeed = await _homePageService.GetActivityFeedAsync();
        
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
