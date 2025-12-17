using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ActivityFeedController : ControllerBase
{
    private readonly IActivityFeedService _activityFeedService;

    public ActivityFeedController(IActivityFeedService activityFeedService)
    {
        _activityFeedService = activityFeedService;
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 50)
    {
        var activities = await _activityFeedService.GetRecentActivitiesAsync(count);
        return Ok(activities);
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublic([FromQuery] int count = 50)
    {
        var activities = await _activityFeedService.GetPublicActivitiesAsync(count);
        return Ok(activities);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserActivities(string userId, [FromQuery] int count = 50)
    {
        var activities = await _activityFeedService.GetUserActivitiesAsync(userId, count);
        return Ok(activities);
    }

    [HttpGet("my-activities")]
    public async Task<IActionResult> GetMyActivities([FromQuery] int count = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var activities = await _activityFeedService.GetUserActivitiesAsync(userIdClaim, count);
        return Ok(activities);
    }
}
