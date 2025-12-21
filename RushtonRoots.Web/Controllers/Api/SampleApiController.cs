using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// Sample API controller for testing purposes.
/// This endpoint is available without authentication for health check and testing.
/// Should be disabled in production if not needed.
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class SampleApiController : ControllerBase
{
    private readonly ILogger<SampleApiController> _logger;

    public SampleApiController(ILogger<SampleApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello from the API!", timestamp = DateTime.UtcNow });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { id, message = $"Item {id}", timestamp = DateTime.UtcNow });
    }
}
