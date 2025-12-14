using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FamilyTaskController : ControllerBase
{
    private readonly IFamilyTaskService _familyTaskService;

    public FamilyTaskController(IFamilyTaskService familyTaskService)
    {
        _familyTaskService = familyTaskService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _familyTaskService.GetByIdAsync(id);
        if (task == null) return NotFound();
        
        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _familyTaskService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("household/{householdId}")]
    public async Task<IActionResult> GetByHousehold(int householdId)
    {
        var tasks = await _familyTaskService.GetByHouseholdIdAsync(householdId);
        return Ok(tasks);
    }

    [HttpGet("assigned-to-me")]
    public async Task<IActionResult> GetAssignedToMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var tasks = await _familyTaskService.GetByAssignedUserIdAsync(userIdClaim);
        return Ok(tasks);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var tasks = await _familyTaskService.GetByStatusAsync(status);
        return Ok(tasks);
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var tasks = await _familyTaskService.GetByEventIdAsync(eventId);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFamilyTaskRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var task = await _familyTaskService.CreateTaskAsync(request, userIdClaim);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFamilyTaskRequest request)
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
            var updatedTask = await _familyTaskService.UpdateTaskAsync(id, request, userIdClaim);
            return Ok(updatedTask);
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
            await _familyTaskService.DeleteTaskAsync(id, userIdClaim);
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
