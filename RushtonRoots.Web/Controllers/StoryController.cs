using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// API controller for Story operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;

    public StoryController(IStoryService storyService)
    {
        _storyService = storyService;
    }

    /// <summary>
    /// Get all stories
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool publishedOnly = false)
    {
        var stories = await _storyService.GetAllAsync(publishedOnly);
        return Ok(stories);
    }

    /// <summary>
    /// Get story by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool incrementViewCount = true)
    {
        var story = await _storyService.GetByIdAsync(id, incrementViewCount);
        if (story == null)
        {
            return NotFound();
        }
        return Ok(story);
    }

    /// <summary>
    /// Get story by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool incrementViewCount = true)
    {
        var story = await _storyService.GetBySlugAsync(slug, incrementViewCount);
        if (story == null)
        {
            return NotFound();
        }
        return Ok(story);
    }

    /// <summary>
    /// Search stories with filters
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchStoryRequest request)
    {
        var result = await _storyService.SearchAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Get stories by category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category, [FromQuery] bool publishedOnly = true)
    {
        var stories = await _storyService.GetByCategoryAsync(category, publishedOnly);
        return Ok(stories);
    }

    /// <summary>
    /// Get stories by person ID
    /// </summary>
    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPersonId(int personId, [FromQuery] bool publishedOnly = true)
    {
        var stories = await _storyService.GetByPersonIdAsync(personId, publishedOnly);
        return Ok(stories);
    }

    /// <summary>
    /// Get stories by collection ID
    /// </summary>
    [HttpGet("collection/{collectionId}")]
    public async Task<IActionResult> GetByCollectionId(int collectionId, [FromQuery] bool publishedOnly = true)
    {
        var stories = await _storyService.GetByCollectionIdAsync(collectionId, publishedOnly);
        return Ok(stories);
    }

    /// <summary>
    /// Get recent stories
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 10, [FromQuery] bool publishedOnly = true)
    {
        var stories = await _storyService.GetRecentAsync(count, publishedOnly);
        return Ok(stories);
    }

    /// <summary>
    /// Get all story categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _storyService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Create a new story (Admin/HouseholdAdmin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateStoryRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var story = await _storyService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = story.Id }, story);
    }

    /// <summary>
    /// Update a story (Admin/HouseholdAdmin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStoryRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var story = await _storyService.UpdateAsync(id, request, userId);
            return Ok(story);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a story (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _storyService.DeleteAsync(id);
        return NoContent();
    }
}
