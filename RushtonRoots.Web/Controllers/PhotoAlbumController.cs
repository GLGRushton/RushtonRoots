using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PhotoAlbumController : ControllerBase
{
    private readonly IPhotoAlbumService _albumService;

    public PhotoAlbumController(IPhotoAlbumService albumService)
    {
        _albumService = albumService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var album = await _albumService.GetByIdAsync(id);
        if (album == null) return NotFound();
        return Ok(album);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var albums = await _albumService.GetAllAsync();
        return Ok(albums);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var albums = await _albumService.GetByUserIdAsync(userId);
        return Ok(albums);
    }

    [HttpGet("my-albums")]
    public async Task<IActionResult> GetMyAlbums()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var albums = await _albumService.GetByUserIdAsync(userIdClaim);
        return Ok(albums);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhotoAlbumRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var album = await _albumService.CreateAlbumAsync(request, userIdClaim);
        return CreatedAtAction(nameof(GetById), new { id = album.Id }, album);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePhotoAlbumRequest request)
    {
        try
        {
            var album = await _albumService.UpdateAlbumAsync(id, request);
            return Ok(album);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _albumService.DeleteAlbumAsync(id);
        return NoContent();
    }
}
