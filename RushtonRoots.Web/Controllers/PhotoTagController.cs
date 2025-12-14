using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PhotoTagController : ControllerBase
{
    private readonly IPhotoTagService _tagService;

    public PhotoTagController(IPhotoTagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tag = await _tagService.GetByIdAsync(id);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpGet("photo/{photoId}")]
    public async Task<IActionResult> GetByPhotoId(int photoId)
    {
        var tags = await _tagService.GetByPhotoIdAsync(photoId);
        return Ok(tags);
    }

    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPersonId(int personId)
    {
        var tags = await _tagService.GetByPersonIdAsync(personId);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhotoTagRequest request)
    {
        var tag = await _tagService.CreateTagAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tagService.DeleteTagAsync(id);
        return NoContent();
    }
}
