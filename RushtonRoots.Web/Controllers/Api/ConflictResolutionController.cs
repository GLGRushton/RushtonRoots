using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConflictResolutionController : ControllerBase
{
    private readonly IConflictResolutionService _conflictService;

    public ConflictResolutionController(IConflictResolutionService conflictService)
    {
        _conflictService = conflictService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var conflict = await _conflictService.GetByIdAsync(id);
        if (conflict == null) return NotFound();
        
        return Ok(conflict);
    }

    [HttpGet("open")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> GetOpenConflicts()
    {
        var conflicts = await _conflictService.GetOpenConflictsAsync();
        return Ok(conflicts);
    }

    [HttpPost("resolve")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Resolve([FromBody] ResolveConflictRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        try
        {
            var conflict = await _conflictService.ResolveConflictAsync(request, userIdClaim);
            return Ok(conflict);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
