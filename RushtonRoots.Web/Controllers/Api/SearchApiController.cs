using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// API Controller for advanced search and discovery operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SearchApiController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly IPersonService _personService;

    public SearchApiController(ISearchService searchService, IPersonService personService)
    {
        _searchService = searchService;
        _personService = personService;
    }

    /// <summary>
    /// Advanced person search with multiple criteria.
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>List of matching people</returns>
    [HttpGet("person")]
    public async Task<IActionResult> SearchPerson([FromQuery] SearchPersonRequest request)
    {
        var results = await _personService.SearchAsync(request);
        return Ok(results);
    }

    /// <summary>
    /// Find the relationship path between two people.
    /// </summary>
    /// <param name="personAId">First person ID</param>
    /// <param name="personBId">Second person ID</param>
    /// <returns>Relationship path information</returns>
    [HttpGet("relationship")]
    public async Task<IActionResult> FindRelationship([FromQuery] int personAId, [FromQuery] int personBId)
    {
        var relationship = await _searchService.FindRelationshipAsync(personAId, personBId);
        
        if (relationship == null)
            return NotFound(new { message = "No relationship found between the specified people." });
        
        return Ok(relationship);
    }

    /// <summary>
    /// Get surname distribution statistics.
    /// </summary>
    /// <returns>List of surnames with counts</returns>
    [HttpGet("surname-distribution")]
    public async Task<IActionResult> GetSurnameDistribution()
    {
        var distribution = await _searchService.GetSurnameDistributionAsync();
        return Ok(distribution);
    }

    /// <summary>
    /// Find all people from a specific location.
    /// </summary>
    /// <param name="locationId">Location ID</param>
    /// <returns>List of people associated with the location</returns>
    [HttpGet("by-location/{locationId}")]
    public async Task<IActionResult> GetPeopleByLocation(int locationId)
    {
        var people = await _searchService.GetPeopleByLocationAsync(locationId);
        return Ok(people);
    }

    /// <summary>
    /// Find all people with a specific event type.
    /// </summary>
    /// <param name="eventType">Event type (e.g., "Birth", "Marriage", "Education")</param>
    /// <returns>List of people with the specified event type</returns>
    [HttpGet("by-event-type")]
    public async Task<IActionResult> GetPeopleByEventType([FromQuery] string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            return BadRequest(new { message = "Event type is required." });
        
        var people = await _searchService.GetPeopleByEventTypeAsync(eventType);
        return Ok(people);
    }
}
