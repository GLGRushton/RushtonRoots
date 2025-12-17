using Microsoft.AspNetCore.Mvc;

namespace RushtonRoots.Web.Controllers.Api;

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
