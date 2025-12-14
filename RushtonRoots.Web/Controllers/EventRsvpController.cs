using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventRsvpController : ControllerBase
{
    private readonly IEventRsvpService _eventRsvpService;

    public EventRsvpController(IEventRsvpService eventRsvpService)
    {
        _eventRsvpService = eventRsvpService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rsvp = await _eventRsvpService.GetByIdAsync(id);
        if (rsvp == null) return NotFound();
        
        return Ok(rsvp);
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var rsvps = await _eventRsvpService.GetByEventIdAsync(eventId);
        return Ok(rsvps);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var rsvps = await _eventRsvpService.GetByUserIdAsync(userId);
        return Ok(rsvps);
    }

    [HttpGet("my-rsvps")]
    public async Task<IActionResult> GetMyRsvps()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var rsvps = await _eventRsvpService.GetByUserIdAsync(userIdClaim);
        return Ok(rsvps);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventRsvpRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        try
        {
            var rsvp = await _eventRsvpService.CreateRsvpAsync(request, userIdClaim);
            return CreatedAtAction(nameof(GetById), new { id = rsvp.Id }, rsvp);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventRsvpRequest request)
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
            var updatedRsvp = await _eventRsvpService.UpdateRsvpAsync(id, request, userIdClaim);
            return Ok(updatedRsvp);
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
            await _eventRsvpService.DeleteRsvpAsync(id, userIdClaim);
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
