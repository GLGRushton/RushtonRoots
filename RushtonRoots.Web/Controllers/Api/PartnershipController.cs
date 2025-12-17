using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// API controller for Partnership entity operations.
/// Provides RESTful endpoints for creating, reading, updating, and deleting partnerships.
/// </summary>
[ApiController]
[Route("api/partnership")]
[Authorize]
public class PartnershipController : ControllerBase
{
    private readonly IPartnershipService _partnershipService;
    private readonly ILogger<PartnershipController> _logger;

    public PartnershipController(IPartnershipService partnershipService, ILogger<PartnershipController> logger)
    {
        _partnershipService = partnershipService;
        _logger = logger;
    }

    /// <summary>
    /// Get all partnerships with optional pagination.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <returns>List of partnerships</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PartnershipViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PartnershipViewModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var partnerships = await _partnershipService.GetAllAsync();
            
            // Apply pagination
            var paginatedPartnerships = partnerships
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            
            return Ok(paginatedPartnerships);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all partnerships");
            return StatusCode(500, "An error occurred while retrieving partnerships");
        }
    }

    /// <summary>
    /// Get a specific partnership by ID.
    /// </summary>
    /// <param name="id">Partnership ID</param>
    /// <returns>Partnership details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PartnershipViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartnershipViewModel>> GetById(int id)
    {
        try
        {
            var partnership = await _partnershipService.GetByIdAsync(id);
            
            if (partnership == null)
            {
                _logger.LogWarning("Partnership with ID {PartnershipId} not found", id);
                return NotFound($"Partnership with ID {id} not found");
            }
            
            return Ok(partnership);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving partnership with ID {PartnershipId}", id);
            return StatusCode(500, "An error occurred while retrieving the partnership");
        }
    }

    /// <summary>
    /// Get all partnerships for a specific person.
    /// </summary>
    /// <param name="personId">Person ID</param>
    /// <returns>List of partnerships for the person</returns>
    [HttpGet("person/{personId}")]
    [ProducesResponseType(typeof(IEnumerable<PartnershipViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PartnershipViewModel>>> GetByPersonId(int personId)
    {
        try
        {
            var partnerships = await _partnershipService.GetByPersonIdAsync(personId);
            return Ok(partnerships);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving partnerships for person with ID {PersonId}", personId);
            return StatusCode(500, "An error occurred while retrieving partnerships");
        }
    }

    /// <summary>
    /// Create a new partnership.
    /// </summary>
    /// <param name="request">Partnership creation request</param>
    /// <returns>Created partnership</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(typeof(PartnershipViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PartnershipViewModel>> Create([FromBody] CreatePartnershipRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate relationship integrity
            if (request.PersonAId == request.PersonBId)
            {
                return BadRequest("A person cannot be partnered with themselves");
            }

            var partnership = await _partnershipService.CreateAsync(request);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = partnership.Id },
                partnership);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid partnership creation request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating partnership");
            return StatusCode(500, "An error occurred while creating the partnership");
        }
    }

    /// <summary>
    /// Update an existing partnership.
    /// </summary>
    /// <param name="id">Partnership ID</param>
    /// <param name="request">Partnership update request</param>
    /// <returns>Updated partnership</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(typeof(PartnershipViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartnershipViewModel>> Update(int id, [FromBody] UpdatePartnershipRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest("ID in URL does not match ID in request body");
            }

            // Validate relationship integrity
            if (request.PersonAId == request.PersonBId)
            {
                return BadRequest("A person cannot be partnered with themselves");
            }

            var partnership = await _partnershipService.UpdateAsync(request);
            
            return Ok(partnership);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Partnership with ID {PartnershipId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid partnership update request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating partnership with ID {PartnershipId}", id);
            return StatusCode(500, "An error occurred while updating the partnership");
        }
    }

    /// <summary>
    /// Delete a partnership (soft delete).
    /// </summary>
    /// <param name="id">Partnership ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _partnershipService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Partnership with ID {PartnershipId} not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting partnership with ID {PartnershipId}", id);
            return StatusCode(500, "An error occurred while deleting the partnership");
        }
    }
}
