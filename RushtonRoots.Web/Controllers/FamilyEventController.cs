using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FamilyEventController : ControllerBase
{
    private readonly IFamilyEventService _familyEventService;

    public FamilyEventController(IFamilyEventService familyEventService)
    {
        _familyEventService = familyEventService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var familyEvent = await _familyEventService.GetByIdAsync(id);
        if (familyEvent == null) return NotFound();
        
        return Ok(familyEvent);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _familyEventService.GetAllAsync();
        return Ok(events);
    }

    [HttpGet("household/{householdId}")]
    public async Task<IActionResult> GetByHousehold(int householdId)
    {
        var events = await _familyEventService.GetByHouseholdIdAsync(householdId);
        return Ok(events);
    }

    [HttpGet("daterange")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var events = await _familyEventService.GetByDateRangeAsync(startDate, endDate);
        return Ok(events);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] int count = 10)
    {
        var events = await _familyEventService.GetUpcomingEventsAsync(count);
        return Ok(events);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFamilyEventRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var familyEvent = await _familyEventService.CreateEventAsync(request, userIdClaim);
        return CreatedAtAction(nameof(GetById), new { id = familyEvent.Id }, familyEvent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFamilyEventRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        if (id != request.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var updatedEvent = await _familyEventService.UpdateEventAsync(id, request, userIdClaim);
            return Ok(updatedEvent);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        try
        {
            await _familyEventService.DeleteEventAsync(id, userIdClaim);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
