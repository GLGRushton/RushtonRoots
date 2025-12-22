using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// API controller for Tradition operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TraditionController : ControllerBase
{
    private readonly ITraditionService _traditionService;

    public TraditionController(ITraditionService traditionService)
    {
        _traditionService = traditionService;
    }

    /// <summary>
    /// Get all traditions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool publishedOnly = false)
    {
        var traditions = await _traditionService.GetAllAsync(publishedOnly);
        return Ok(traditions);
    }

    /// <summary>
    /// Get tradition by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool incrementViewCount = true)
    {
        var tradition = await _traditionService.GetByIdAsync(id, incrementViewCount);
        if (tradition == null)
        {
            return NotFound();
        }
        return Ok(tradition);
    }

    /// <summary>
    /// Get tradition by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool incrementViewCount = true)
    {
        var tradition = await _traditionService.GetBySlugAsync(slug, incrementViewCount);
        if (tradition == null)
        {
            return NotFound();
        }
        return Ok(tradition);
    }

    /// <summary>
    /// Search traditions with filters
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchTraditionRequest request)
    {
        var result = await _traditionService.SearchAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Get traditions by category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category, [FromQuery] bool publishedOnly = true)
    {
        var traditions = await _traditionService.GetByCategoryAsync(category, publishedOnly);
        return Ok(traditions);
    }

    /// <summary>
    /// Get traditions by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status, [FromQuery] bool publishedOnly = true)
    {
        var traditions = await _traditionService.GetByStatusAsync(status, publishedOnly);
        return Ok(traditions);
    }

    /// <summary>
    /// Get traditions by person (who started them)
    /// </summary>
    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPerson(int personId, [FromQuery] bool publishedOnly = true)
    {
        var traditions = await _traditionService.GetByPersonAsync(personId, publishedOnly);
        return Ok(traditions);
    }

    /// <summary>
    /// Get recent traditions
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 10, [FromQuery] bool publishedOnly = true)
    {
        var traditions = await _traditionService.GetRecentAsync(count, publishedOnly);
        return Ok(traditions);
    }

    /// <summary>
    /// Get all tradition categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _traditionService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Create a new tradition
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateTraditionRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var tradition = await _traditionService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = tradition.Id }, tradition);
    }

    /// <summary>
    /// Update an existing tradition
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTraditionRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var tradition = await _traditionService.UpdateAsync(id, request, userId);
            return Ok(tradition);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a tradition
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _traditionService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get recipes related to a specific tradition
    /// </summary>
    /// <param name="id">The ID of the tradition</param>
    /// <returns>List of related recipes</returns>
    [HttpGet("{id}/recipes")]
    public async Task<IActionResult> GetRelatedRecipes(int id)
    {
        try
        {
            var recipes = await _traditionService.GetRelatedRecipesAsync(id);
            return Ok(recipes);
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Tradition with ID {id} not found" });
        }
    }

    /// <summary>
    /// Get stories related to a specific tradition
    /// </summary>
    /// <param name="id">The ID of the tradition</param>
    /// <returns>List of related stories</returns>
    [HttpGet("{id}/stories")]
    public async Task<IActionResult> GetRelatedStories(int id)
    {
        try
        {
            var stories = await _traditionService.GetRelatedStoriesAsync(id);
            return Ok(stories);
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Tradition with ID {id} not found" });
        }
    }

    /// <summary>
    /// Get past occurrences of a tradition
    /// </summary>
    /// <param name="id">The ID of the tradition</param>
    /// <param name="count">Number of past occurrences to return (default: 5)</param>
    /// <returns>List of past tradition occurrences</returns>
    [HttpGet("{id}/occurrences/past")]
    public async Task<IActionResult> GetPastOccurrences(int id, [FromQuery] int count = 5)
    {
        try
        {
            var occurrences = await _traditionService.GetPastOccurrencesAsync(id, count);
            return Ok(occurrences);
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Tradition with ID {id} not found" });
        }
    }

    /// <summary>
    /// Get the next occurrence of a tradition
    /// </summary>
    /// <param name="id">The ID of the tradition</param>
    /// <returns>Next tradition occurrence or null if no future occurrence is scheduled</returns>
    [HttpGet("{id}/occurrences/next")]
    public async Task<IActionResult> GetNextOccurrence(int id)
    {
        try
        {
            var occurrence = await _traditionService.GetNextOccurrenceAsync(id);
            if (occurrence == null)
            {
                return Ok(new { message = "No future occurrence scheduled for this tradition" });
            }
            return Ok(occurrence);
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Tradition with ID {id} not found" });
        }
    }
}

/// <summary>
/// API controller for TraditionTimeline operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TraditionTimelineController : ControllerBase
{
    private readonly ITraditionTimelineService _timelineService;

    public TraditionTimelineController(ITraditionTimelineService timelineService)
    {
        _timelineService = timelineService;
    }

    /// <summary>
    /// Get timeline entry by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var timeline = await _timelineService.GetByIdAsync(id);
        if (timeline == null)
        {
            return NotFound();
        }
        return Ok(timeline);
    }

    /// <summary>
    /// Get all timeline entries for a tradition
    /// </summary>
    [HttpGet("tradition/{traditionId}")]
    public async Task<IActionResult> GetByTradition(int traditionId)
    {
        var timelines = await _timelineService.GetByTraditionAsync(traditionId);
        return Ok(timelines);
    }

    /// <summary>
    /// Create a new timeline entry
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateTraditionTimelineRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var timeline = await _timelineService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = timeline.Id }, timeline);
    }

    /// <summary>
    /// Update an existing timeline entry
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTraditionTimelineRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var timeline = await _timelineService.UpdateAsync(id, request, userId);
            return Ok(timeline);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a timeline entry
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _timelineService.DeleteAsync(id);
        return NoContent();
    }
}
