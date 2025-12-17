using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// API controller for Household entity operations.
/// Provides RESTful endpoints for creating, reading, updating, and deleting households.
/// </summary>
[ApiController]
[Route("api/household")]
[Authorize]
public class HouseholdController : ControllerBase
{
    private readonly IHouseholdService _householdService;
    private readonly ILogger<HouseholdController> _logger;

    public HouseholdController(IHouseholdService householdService, ILogger<HouseholdController> logger)
    {
        _householdService = householdService;
        _logger = logger;
    }

    /// <summary>
    /// Get all households with optional pagination.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <returns>List of households</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HouseholdViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HouseholdViewModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var households = await _householdService.GetAllAsync();
            
            // Apply pagination
            var paginatedHouseholds = households
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            
            return Ok(paginatedHouseholds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all households");
            return StatusCode(500, "An error occurred while retrieving households");
        }
    }

    /// <summary>
    /// Get a specific household by ID.
    /// </summary>
    /// <param name="id">Household ID</param>
    /// <returns>Household details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HouseholdViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdViewModel>> GetById(int id)
    {
        try
        {
            var household = await _householdService.GetByIdAsync(id);
            
            if (household == null)
            {
                _logger.LogWarning("Household with ID {HouseholdId} not found", id);
                return NotFound($"Household with ID {id} not found");
            }
            
            return Ok(household);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving household with ID {HouseholdId}", id);
            return StatusCode(500, "An error occurred while retrieving the household");
        }
    }

    /// <summary>
    /// Get all members of a specific household.
    /// </summary>
    /// <param name="id">Household ID</param>
    /// <returns>List of household members</returns>
    [HttpGet("{id}/members")]
    [ProducesResponseType(typeof(IEnumerable<PersonViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonViewModel>>> GetMembers(int id)
    {
        try
        {
            var members = await _householdService.GetMembersAsync(id);
            return Ok(members);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Household with ID {HouseholdId} not found", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving members for household with ID {HouseholdId}", id);
            return StatusCode(500, "An error occurred while retrieving household members");
        }
    }

    /// <summary>
    /// Create a new household.
    /// </summary>
    /// <param name="request">Household creation request</param>
    /// <returns>Created household</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(HouseholdViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HouseholdViewModel>> Create([FromBody] CreateHouseholdRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var household = await _householdService.CreateAsync(request);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = household.Id },
                household);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating household");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating household");
            return StatusCode(500, "An error occurred while creating the household");
        }
    }

    /// <summary>
    /// Update an existing household.
    /// </summary>
    /// <param name="id">Household ID</param>
    /// <param name="request">Household update request</param>
    /// <returns>Updated household</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(typeof(HouseholdViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdViewModel>> Update(int id, [FromBody] UpdateHouseholdRequest request)
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

            var household = await _householdService.UpdateAsync(request);
            
            return Ok(household);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Household with ID {HouseholdId} not found for update", id);
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating household");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating household with ID {HouseholdId}", id);
            return StatusCode(500, "An error occurred while updating the household");
        }
    }

    /// <summary>
    /// Delete a household.
    /// </summary>
    /// <param name="id">Household ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _householdService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Household with ID {HouseholdId} not found for deletion", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting household with ID {HouseholdId}", id);
            return StatusCode(500, "An error occurred while deleting the household");
        }
    }

    /// <summary>
    /// Add a member to a household.
    /// </summary>
    /// <param name="id">Household ID</param>
    /// <param name="request">Add member request</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/members")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMember(int id, [FromBody] AddHouseholdMemberRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.HouseholdId)
            {
                return BadRequest("Household ID in URL does not match ID in request body");
            }

            await _householdService.AddMemberAsync(request);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Error adding member to household {HouseholdId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error adding member to household");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to household {HouseholdId}", id);
            return StatusCode(500, "An error occurred while adding member to household");
        }
    }

    /// <summary>
    /// Remove a member from a household.
    /// </summary>
    /// <param name="id">Household ID</param>
    /// <param name="personId">Person ID to remove</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}/members/{personId}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember(int id, int personId)
    {
        try
        {
            await _householdService.RemoveMemberAsync(id, personId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Error removing member from household {HouseholdId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error removing member from household");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member {PersonId} from household {HouseholdId}", personId, id);
            return StatusCode(500, "An error occurred while removing member from household");
        }
    }

    /// <summary>
    /// Update household settings (e.g., archive status).
    /// </summary>
    /// <param name="id">Household ID</param>
    /// <param name="request">Settings update request</param>
    /// <returns>Updated household</returns>
    [HttpPut("{id}/settings")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(typeof(HouseholdViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdViewModel>> UpdateSettings(int id, [FromBody] UpdateHouseholdSettingsRequest request)
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

            var household = await _householdService.UpdateSettingsAsync(request);
            return Ok(household);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Household with ID {HouseholdId} not found for settings update", id);
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating household settings");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating settings for household with ID {HouseholdId}", id);
            return StatusCode(500, "An error occurred while updating household settings");
        }
    }
}
