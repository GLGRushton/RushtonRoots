using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for static informational pages - provides public content pages
/// </summary>
public class InfoController : Controller
{
    private readonly ILogger<InfoController> _logger;

    public InfoController(ILogger<InfoController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the About RushtonRoots page
    /// </summary>
    /// <returns>About page view</returns>
    public IActionResult About()
    {
        ViewData["Title"] = "About RushtonRoots";
        ViewData["Description"] = "Learn about RushtonRoots - A comprehensive family tree and genealogy application designed to preserve your family history and connect with your heritage.";
        ViewData["OgTitle"] = "About RushtonRoots";
        ViewData["OgDescription"] = "Learn about RushtonRoots - A comprehensive family tree and genealogy application designed to preserve your family history and connect with your heritage.";
        ViewData["OgType"] = "website";
        
        return View();
    }

    /// <summary>
    /// Display the Contact information page
    /// </summary>
    /// <returns>Contact page view</returns>
    public IActionResult Contact()
    {
        ViewData["Title"] = "Contact Us";
        ViewData["Description"] = "Get in touch with the RushtonRoots team. We're here to help with questions, feedback, and support.";
        ViewData["OgTitle"] = "Contact RushtonRoots";
        ViewData["OgDescription"] = "Get in touch with the RushtonRoots team. We're here to help with questions, feedback, and support.";
        ViewData["OgType"] = "website";
        
        return View();
    }

    /// <summary>
    /// Display the Mission statement page
    /// </summary>
    /// <returns>Mission page view</returns>
    public IActionResult Mission()
    {
        ViewData["Title"] = "Our Mission";
        ViewData["Description"] = "The mission of RushtonRoots is to help families preserve their history, connect with their heritage, and strengthen family bonds across generations.";
        ViewData["OgTitle"] = "RushtonRoots Mission";
        ViewData["OgDescription"] = "The mission of RushtonRoots is to help families preserve their history, connect with their heritage, and strengthen family bonds across generations.";
        ViewData["OgType"] = "website";
        
        return View();
    }

    /// <summary>
    /// Display the Privacy Policy page
    /// </summary>
    /// <returns>Privacy policy view</returns>
    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy";
        ViewData["Description"] = "Read our privacy policy to understand how RushtonRoots collects, uses, and protects your personal information and family data.";
        ViewData["OgTitle"] = "RushtonRoots Privacy Policy";
        ViewData["OgDescription"] = "Read our privacy policy to understand how RushtonRoots collects, uses, and protects your personal information and family data.";
        ViewData["OgType"] = "website";
        
        return View();
    }

    /// <summary>
    /// Display the Terms of Service page
    /// </summary>
    /// <returns>Terms of service view</returns>
    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms of Service";
        ViewData["Description"] = "Read the RushtonRoots terms of service to understand your rights and responsibilities when using our family tree and genealogy platform.";
        ViewData["OgTitle"] = "RushtonRoots Terms of Service";
        ViewData["OgDescription"] = "Read the RushtonRoots terms of service to understand your rights and responsibilities when using our family tree and genealogy platform.";
        ViewData["OgType"] = "website";
        
        return View();
    }

    /// <summary>
    /// Display the Family Story page
    /// </summary>
    /// <returns>Family story view</returns>
    public IActionResult Story()
    {
        ViewData["Title"] = "Our Family Story";
        ViewData["Description"] = "Discover the rich history and heritage of the Rushton family, the inspiration behind RushtonRoots genealogy platform.";
        ViewData["OgTitle"] = "The Rushton Family Story";
        ViewData["OgDescription"] = "Discover the rich history and heritage of the Rushton family, the inspiration behind RushtonRoots genealogy platform.";
        ViewData["OgType"] = "article";
        ViewData["ArticleSection"] = "Family History";
        
        return View();
    }
}
