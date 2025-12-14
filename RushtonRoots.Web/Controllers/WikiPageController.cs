using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WikiPageController : ControllerBase
{
    private readonly IWikiPageService _wikiPageService;

    public WikiPageController(IWikiPageService wikiPageService)
    {
        _wikiPageService = wikiPageService;
    }

    /// <summary>
    /// Get all wiki pages
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool publishedOnly = false)
    {
        var pages = await _wikiPageService.GetAllAsync(publishedOnly);
        return Ok(pages);
    }

    /// <summary>
    /// Get wiki page by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var page = await _wikiPageService.GetByIdAsync(id, incrementViewCount: true);
        if (page == null)
        {
            return NotFound();
        }
        return Ok(page);
    }

    /// <summary>
    /// Get wiki page by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var page = await _wikiPageService.GetBySlugAsync(slug, incrementViewCount: true);
        if (page == null)
        {
            return NotFound();
        }
        return Ok(page);
    }

    /// <summary>
    /// Search wiki pages
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WikiSearchRequest request)
    {
        var (pages, totalCount) = await _wikiPageService.SearchAsync(request);
        return Ok(new { Pages = pages, TotalCount = totalCount });
    }

    /// <summary>
    /// Get wiki pages by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var pages = await _wikiPageService.GetByCategoryAsync(categoryId);
        return Ok(pages);
    }

    /// <summary>
    /// Get wiki pages by tag
    /// </summary>
    [HttpGet("tag/{tagId}")]
    public async Task<IActionResult> GetByTag(int tagId)
    {
        var pages = await _wikiPageService.GetByTagAsync(tagId);
        return Ok(pages);
    }

    /// <summary>
    /// Get recent wiki pages
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 10, [FromQuery] bool publishedOnly = true)
    {
        var pages = await _wikiPageService.GetRecentAsync(count, publishedOnly);
        return Ok(pages);
    }

    /// <summary>
    /// Create a new wiki page
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateWikiPageRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var page = await _wikiPageService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = page.Id }, page);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing wiki page
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWikiPageRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var page = await _wikiPageService.UpdateAsync(id, request, userId);
            return Ok(page);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a wiki page
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _wikiPageService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get version history for a wiki page
    /// </summary>
    [HttpGet("{id}/versions")]
    public async Task<IActionResult> GetVersionHistory(int id)
    {
        var versions = await _wikiPageService.GetVersionHistoryAsync(id);
        return Ok(versions);
    }

    /// <summary>
    /// Get a specific version of a wiki page
    /// </summary>
    [HttpGet("{id}/versions/{versionNumber}")]
    public async Task<IActionResult> GetVersion(int id, int versionNumber)
    {
        var version = await _wikiPageService.GetVersionAsync(id, versionNumber);
        if (version == null)
        {
            return NotFound();
        }
        return Ok(version);
    }
}
