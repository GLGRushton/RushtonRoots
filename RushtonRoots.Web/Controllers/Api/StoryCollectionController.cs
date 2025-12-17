using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// API controller for StoryCollection operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoryCollectionController : ControllerBase
{
    private readonly IStoryCollectionService _collectionService;

    public StoryCollectionController(IStoryCollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    /// <summary>
    /// Get all story collections
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool publishedOnly = false)
    {
        var collections = await _collectionService.GetAllAsync(publishedOnly);
        return Ok(collections);
    }

    /// <summary>
    /// Get story collection by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool includeStories = false)
    {
        var collection = await _collectionService.GetByIdAsync(id, includeStories);
        if (collection == null)
        {
            return NotFound();
        }
        return Ok(collection);
    }

    /// <summary>
    /// Get story collection by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool includeStories = false)
    {
        var collection = await _collectionService.GetBySlugAsync(slug, includeStories);
        if (collection == null)
        {
            return NotFound();
        }
        return Ok(collection);
    }

    /// <summary>
    /// Create a new story collection (Admin/HouseholdAdmin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateStoryCollectionRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var collection = await _collectionService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = collection.Id }, collection);
    }

    /// <summary>
    /// Update a story collection (Admin/HouseholdAdmin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStoryCollectionRequest request)
    {
        try
        {
            var collection = await _collectionService.UpdateAsync(id, request);
            return Ok(collection);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a story collection (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _collectionService.DeleteAsync(id);
        return NoContent();
    }
}
