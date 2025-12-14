using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContributionController : ControllerBase
{
    private readonly IContributionService _contributionService;

    public ContributionController(IContributionService contributionService)
    {
        _contributionService = contributionService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contribution = await _contributionService.GetByIdAsync(id);
        if (contribution == null) return NotFound();
        
        return Ok(contribution);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var contributions = await _contributionService.GetByStatusAsync(status);
        return Ok(contributions);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var contributions = await _contributionService.GetPendingContributionsAsync();
        return Ok(contributions);
    }

    [HttpGet("my-contributions")]
    public async Task<IActionResult> GetMyContributions()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var contributions = await _contributionService.GetByContributorAsync(userIdClaim);
        return Ok(contributions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContributionRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var contribution = await _contributionService.CreateAsync(request, userIdClaim);
        return CreatedAtAction(nameof(GetById), new { id = contribution.Id }, contribution);
    }

    [HttpPost("review")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Review([FromBody] ReviewContributionRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        try
        {
            var contribution = await _contributionService.ReviewAsync(request, userIdClaim);
            return Ok(contribution);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("apply/{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Apply(int id)
    {
        var success = await _contributionService.ApplyContributionAsync(id);
        if (!success)
        {
            return BadRequest("Contribution cannot be applied");
        }

        return Ok(new { message = "Contribution applied successfully" });
    }
}
