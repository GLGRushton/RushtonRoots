using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PhotoController : ControllerBase
{
    private readonly IPersonPhotoService _photoService;

    public PhotoController(IPersonPhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var photo = await _photoService.GetByIdAsync(id);
        if (photo == null) return NotFound();
        return Ok(photo);
    }

    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPersonId(int personId)
    {
        var photos = await _photoService.GetByPersonIdAsync(personId);
        return Ok(photos);
    }

    [HttpGet("person/{personId}/primary")]
    public async Task<IActionResult> GetPrimaryPhoto(int personId)
    {
        var photo = await _photoService.GetPrimaryPhotoAsync(personId);
        if (photo == null) return NotFound();
        return Ok(photo);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadPhotoRequest request, IFormFile file)
    {
        try
        {
            var photo = await _photoService.UploadPhotoAsync(request, file);
            return CreatedAtAction(nameof(GetById), new { id = photo.Id }, photo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePersonPhotoRequest request)
    {
        try
        {
            var photo = await _photoService.UpdatePhotoAsync(id, request);
            return Ok(photo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _photoService.DeletePhotoAsync(id);
        return NoContent();
    }

    [HttpGet("timeline")]
    public async Task<IActionResult> GetPhotoTimeline([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var photos = await _photoService.GetPhotosByDateRangeAsync(startDate, endDate);
        return Ok(photos);
    }

    [HttpGet("album/{albumId}")]
    public async Task<IActionResult> GetByAlbumId(int albumId)
    {
        var photos = await _photoService.GetPhotosByAlbumIdAsync(albumId);
        return Ok(photos);
    }
}
