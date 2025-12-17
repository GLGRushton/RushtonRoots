using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var document = await _documentService.GetByIdAsync(id);
        if (document == null) return NotFound();
        return Ok(document);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents = await _documentService.GetAllAsync();
        return Ok(documents);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var documents = await _documentService.GetByUserIdAsync(userId);
        return Ok(documents);
    }

    [HttpGet("my-documents")]
    public async Task<IActionResult> GetMyDocuments()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var documents = await _documentService.GetByUserIdAsync(userIdClaim);
        return Ok(documents);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var documents = await _documentService.GetByCategoryAsync(category);
        return Ok(documents);
    }

    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPersonId(int personId)
    {
        var documents = await _documentService.GetByPersonIdAsync(personId);
        return Ok(documents);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchDocumentRequest request)
    {
        var documents = await _documentService.SearchAsync(request);
        return Ok(documents);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] CreateDocumentRequest request, IFormFile file)
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
            var document = await _documentService.UploadDocumentAsync(request, file, userIdClaim);
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading document: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentRequest request)
    {
        try
        {
            var document = await _documentService.UpdateDocumentAsync(id, request);
            return Ok(document);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _documentService.DeleteDocumentAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/version")]
    public async Task<IActionResult> UploadVersion(int id, IFormFile file, [FromForm] string? changeNotes)
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
            var version = await _documentService.UploadNewVersionAsync(id, file, userIdClaim, changeNotes);
            return Ok(version);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading version: {ex.Message}");
        }
    }

    [HttpGet("{id}/versions")]
    public async Task<IActionResult> GetVersions(int id)
    {
        var versions = await _documentService.GetVersionsAsync(id);
        return Ok(versions);
    }

    [HttpGet("{id}/preview")]
    public async Task<IActionResult> GetPreview(int id)
    {
        try
        {
            var previewUrl = await _documentService.GetDocumentPreviewUrlAsync(id);
            return Ok(new { url = previewUrl });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error generating preview: {ex.Message}");
        }
    }
}
