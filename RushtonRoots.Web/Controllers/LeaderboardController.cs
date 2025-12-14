using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly IContributionScoreService _scoreService;

    public LeaderboardController(IContributionScoreService scoreService)
    {
        _scoreService = scoreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int count = 10)
    {
        var leaderboard = await _scoreService.GetLeaderboardAsync(count);
        return Ok(leaderboard);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserScore(string userId)
    {
        var score = await _scoreService.GetByUserIdAsync(userId);
        if (score == null) return NotFound();
        
        return Ok(score);
    }

    [HttpGet("my-score")]
    public async Task<IActionResult> GetMyScore()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var score = await _scoreService.GetByUserIdAsync(userIdClaim);
        if (score == null)
        {
            return NotFound("No score record found. Start contributing!");
        }
        
        return Ok(score);
    }
}
