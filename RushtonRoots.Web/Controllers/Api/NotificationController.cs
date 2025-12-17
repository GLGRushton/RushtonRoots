using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var notification = await _notificationService.GetByIdAsync(id);
        if (notification == null) return NotFound();
        
        // Verify user owns this notification
        if (notification.UserId != userIdClaim)
        {
            return Forbid();
        }
        
        return Ok(notification);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] bool includeRead = false, [FromQuery] int pageSize = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var notifications = await _notificationService.GetUserNotificationsAsync(userIdClaim, includeRead, pageSize);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var count = await _notificationService.GetUnreadCountAsync(userIdClaim);
        return Ok(new { count });
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        try
        {
            await _notificationService.MarkAsReadAsync(id, userIdClaim);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        await _notificationService.MarkAllAsReadAsync(userIdClaim);
        return NoContent();
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
            await _notificationService.DeleteNotificationAsync(id, userIdClaim);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var preferences = await _notificationService.GetUserPreferencesAsync(userIdClaim);
        return Ok(preferences);
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreference([FromBody] UpdateNotificationPreferenceRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        try
        {
            var preference = await _notificationService.UpdatePreferenceAsync(userIdClaim, request);
            return Ok(preference);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
