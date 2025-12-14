using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LifeEventController : ControllerBase
{
    private readonly ILifeEventService _lifeEventService;

    public LifeEventController(ILifeEventService lifeEventService)
    {
        _lifeEventService = lifeEventService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var lifeEvent = await _lifeEventService.GetByIdAsync(id);
        if (lifeEvent == null)
        {
            return NotFound();
        }
        return Ok(lifeEvent);
    }

    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPersonId(int personId)
    {
        var lifeEvents = await _lifeEventService.GetByPersonIdAsync(personId);
        return Ok(lifeEvents);
    }

    [HttpGet("person/{personId}/timeline")]
    public async Task<IActionResult> GetPersonTimeline(int personId)
    {
        try
        {
            var timeline = await _lifeEventService.GetPersonTimelineAsync(personId);
            return Ok(timeline);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLifeEventRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var lifeEvent = await _lifeEventService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = lifeEvent.Id }, lifeEvent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLifeEventRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var lifeEvent = await _lifeEventService.UpdateAsync(request);
            return Ok(lifeEvent);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _lifeEventService.DeleteAsync(id);
        return NoContent();
    }
}
