using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var media = await _mediaService.GetByIdAsync(id);
        if (media == null) return NotFound();
        return Ok(media);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var mediaList = await _mediaService.GetAllAsync();
        return Ok(mediaList);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var mediaList = await _mediaService.GetByUserIdAsync(userId);
        return Ok(mediaList);
    }

    [HttpGet("my-media")]
    public async Task<IActionResult> GetMyMedia()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var mediaList = await _mediaService.GetByUserIdAsync(userIdClaim);
        return Ok(mediaList);
    }

    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPersonId(int personId)
    {
        var mediaList = await _mediaService.GetByPersonIdAsync(personId);
        return Ok(mediaList);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchMediaRequest request)
    {
        var mediaList = await _mediaService.SearchAsync(request);
        return Ok(mediaList);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] CreateMediaRequest request, IFormFile file)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            var media = await _mediaService.UploadMediaAsync(request, file, userIdClaim);
            return Ok(media);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading media: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMediaRequest request)
    {
        try
        {
            var media = await _mediaService.UpdateMediaAsync(id, request);
            return Ok(media);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating media: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _mediaService.DeleteMediaAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting media: {ex.Message}");
        }
    }

    [HttpGet("{id}/stream")]
    public async Task<IActionResult> GetStreamUrl(int id)
    {
        try
        {
            var url = await _mediaService.GetMediaStreamUrlAsync(id);
            return Ok(new { url });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error getting stream URL: {ex.Message}");
        }
    }

    // Timeline marker endpoints
    [HttpPost("{id}/markers")]
    public async Task<IActionResult> AddTimelineMarker(int id, [FromBody] CreateMediaTimelineMarkerRequest request)
    {
        try
        {
            var marker = await _mediaService.AddTimelineMarkerAsync(id, request);
            return Ok(marker);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error adding timeline marker: {ex.Message}");
        }
    }

    [HttpGet("{id}/markers")]
    public async Task<IActionResult> GetTimelineMarkers(int id)
    {
        var markers = await _mediaService.GetTimelineMarkersAsync(id);
        return Ok(markers);
    }

    [HttpDelete("markers/{markerId}")]
    public async Task<IActionResult> DeleteTimelineMarker(int markerId)
    {
        try
        {
            await _mediaService.DeleteTimelineMarkerAsync(markerId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting timeline marker: {ex.Message}");
        }
    }
}
