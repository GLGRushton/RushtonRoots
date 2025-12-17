using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// MVC controller for the Media Gallery - provides photo and video gallery views
/// </summary>
[Authorize]
public class MediaGalleryController : Controller
{
    private readonly ILogger<MediaGalleryController> _logger;

    public MediaGalleryController(ILogger<MediaGalleryController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the media gallery with optional filtering
    /// </summary>
    /// <param name="type">Optional filter by media type (video or photo)</param>
    /// <param name="page">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 24)</param>
    /// <returns>Gallery view</returns>
    public IActionResult Index(string? type = null, int page = 1, int pageSize = 24)
    {
        ViewData["Title"] = "Media Gallery";
        ViewData["MediaType"] = type;
        ViewData["Page"] = page;
        ViewData["PageSize"] = pageSize;
        
        return View();
    }

    /// <summary>
    /// Display the upload page for uploading new media
    /// </summary>
    /// <returns>Upload view</returns>
    public IActionResult Upload()
    {
        ViewData["Title"] = "Upload Media";
        return View();
    }
}
