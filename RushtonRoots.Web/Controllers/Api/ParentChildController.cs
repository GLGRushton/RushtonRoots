using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// API controller for ParentChild relationship operations.
/// Provides RESTful endpoints for creating, reading, updating, and deleting parent-child relationships.
/// </summary>
[ApiController]
[Route("api/parentchild")]
[Authorize]
public class ParentChildController : ControllerBase
{
    private readonly IParentChildService _parentChildService;
    private readonly ILogger<ParentChildController> _logger;

    public ParentChildController(IParentChildService parentChildService, ILogger<ParentChildController> logger)
    {
        _parentChildService = parentChildService;
        _logger = logger;
    }

    /// <summary>
    /// Get all parent-child relationships with optional pagination.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <returns>List of parent-child relationships</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ParentChildViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ParentChildViewModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var relationships = await _parentChildService.GetAllAsync();
            
            // Apply pagination
            var paginatedRelationships = relationships
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            
            return Ok(paginatedRelationships);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all parent-child relationships");
            return StatusCode(500, "An error occurred while retrieving parent-child relationships");
        }
    }

    /// <summary>
    /// Get a specific parent-child relationship by ID.
    /// </summary>
    /// <param name="id">Relationship ID</param>
    /// <returns>Relationship details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ParentChildViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParentChildViewModel>> GetById(int id)
    {
        try
        {
            var relationship = await _parentChildService.GetByIdAsync(id);
            
            if (relationship == null)
            {
                _logger.LogWarning("Parent-child relationship with ID {RelationshipId} not found", id);
                return NotFound($"Parent-child relationship with ID {id} not found");
            }
            
            return Ok(relationship);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parent-child relationship with ID {RelationshipId}", id);
            return StatusCode(500, "An error occurred while retrieving the relationship");
        }
    }

    /// <summary>
    /// Get all parent-child relationships for a specific person (as parent or child).
    /// </summary>
    /// <param name="personId">Person ID</param>
    /// <returns>List of relationships for the person</returns>
    [HttpGet("person/{personId}")]
    [ProducesResponseType(typeof(IEnumerable<ParentChildViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ParentChildViewModel>>> GetByPersonId(int personId)
    {
        try
        {
            // Get relationships where person is parent
            var asParent = await _parentChildService.GetByParentIdAsync(personId);
            // Get relationships where person is child
            var asChild = await _parentChildService.GetByChildIdAsync(personId);
            
            // Combine both lists
            var allRelationships = asParent.Concat(asChild).Distinct();
            
            return Ok(allRelationships);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving relationships for person with ID {PersonId}", personId);
            return StatusCode(500, "An error occurred while retrieving relationships");
        }
    }

    /// <summary>
    /// Get all parents of a specific child.
    /// </summary>
    /// <param name="childId">Child person ID</param>
    /// <returns>List of parent relationships</returns>
    [HttpGet("parents/{childId}")]
    [ProducesResponseType(typeof(IEnumerable<ParentChildViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ParentChildViewModel>>> GetParents(int childId)
    {
        try
        {
            var parents = await _parentChildService.GetByChildIdAsync(childId);
            return Ok(parents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parents for child with ID {ChildId}", childId);
            return StatusCode(500, "An error occurred while retrieving parents");
        }
    }

    /// <summary>
    /// Get all children of a specific parent.
    /// </summary>
    /// <param name="parentId">Parent person ID</param>
    /// <returns>List of child relationships</returns>
    [HttpGet("children/{parentId}")]
    [ProducesResponseType(typeof(IEnumerable<ParentChildViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ParentChildViewModel>>> GetChildren(int parentId)
    {
        try
        {
            var children = await _parentChildService.GetByParentIdAsync(parentId);
            return Ok(children);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving children for parent with ID {ParentId}", parentId);
            return StatusCode(500, "An error occurred while retrieving children");
        }
    }

    /// <summary>
    /// Create a new parent-child relationship.
    /// </summary>
    /// <param name="request">Relationship creation request</param>
    /// <returns>Created relationship</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(typeof(ParentChildViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ParentChildViewModel>> Create([FromBody] CreateParentChildRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate relationship integrity
            if (request.ParentPersonId == request.ChildPersonId)
            {
                return BadRequest("A person cannot be their own parent");
            }

            var relationship = await _parentChildService.CreateAsync(request);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = relationship.Id },
                relationship);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid parent-child relationship creation request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parent-child relationship");
            return StatusCode(500, "An error occurred while creating the relationship");
        }
    }

    /// <summary>
    /// Update an existing parent-child relationship.
    /// </summary>
    /// <param name="id">Relationship ID</param>
    /// <param name="request">Relationship update request</param>
    /// <returns>Updated relationship</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(typeof(ParentChildViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParentChildViewModel>> Update(int id, [FromBody] UpdateParentChildRequest request)
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
            if (request.ParentPersonId == request.ChildPersonId)
            {
                return BadRequest("A person cannot be their own parent");
            }

            var relationship = await _parentChildService.UpdateAsync(request);
            
            return Ok(relationship);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parent-child relationship with ID {RelationshipId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid parent-child relationship update request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parent-child relationship with ID {RelationshipId}", id);
            return StatusCode(500, "An error occurred while updating the relationship");
        }
    }

    /// <summary>
    /// Delete a parent-child relationship (soft delete).
    /// </summary>
    /// <param name="id">Relationship ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _parentChildService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parent-child relationship with ID {RelationshipId} not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parent-child relationship with ID {RelationshipId}", id);
            return StatusCode(500, "An error occurred while deleting the relationship");
        }
    }

    // Phase 4.2: Evidence & Family Context endpoints

    /// <summary>
    /// Get evidence (sources/citations) for a specific parent-child relationship.
    /// </summary>
    /// <param name="id">Relationship ID</param>
    /// <returns>List of sources that support this relationship</returns>
    [HttpGet("{id}/evidence")]
    [ProducesResponseType(typeof(IEnumerable<SourceViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SourceViewModel>>> GetEvidence(int id)
    {
        try
        {
            var sources = await _parentChildService.GetEvidenceAsync(id);
            return Ok(sources);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parent-child relationship with ID {RelationshipId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving evidence for parent-child relationship with ID {RelationshipId}", id);
            return StatusCode(500, "An error occurred while retrieving evidence");
        }
    }

    /// <summary>
    /// Get related life events for both the parent and child in the relationship.
    /// </summary>
    /// <param name="id">Relationship ID</param>
    /// <returns>List of life events for parent and child</returns>
    [HttpGet("{id}/events")]
    [ProducesResponseType(typeof(IEnumerable<LifeEventViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LifeEventViewModel>>> GetRelatedEvents(int id)
    {
        try
        {
            var events = await _parentChildService.GetRelatedEventsAsync(id);
            return Ok(events);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parent-child relationship with ID {RelationshipId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving related events for parent-child relationship with ID {RelationshipId}", id);
            return StatusCode(500, "An error occurred while retrieving related events");
        }
    }

    /// <summary>
    /// Get grandparents (parents of the parent) in this relationship.
    /// </summary>
    /// <param name="id">Relationship ID</param>
    /// <returns>List of grandparents</returns>
    [HttpGet("{id}/grandparents")]
    [ProducesResponseType(typeof(IEnumerable<PersonViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonViewModel>>> GetGrandparents(int id)
    {
        try
        {
            var grandparents = await _parentChildService.GetGrandparentsAsync(id);
            return Ok(grandparents);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parent-child relationship with ID {RelationshipId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grandparents for parent-child relationship with ID {RelationshipId}", id);
            return StatusCode(500, "An error occurred while retrieving grandparents");
        }
    }

    /// <summary>
    /// Get siblings (other children of the same parent) in this relationship.
    /// </summary>
    /// <param name="id">Relationship ID</param>
    /// <returns>List of siblings</returns>
    [HttpGet("{id}/siblings")]
    [ProducesResponseType(typeof(IEnumerable<PersonViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonViewModel>>> GetSiblings(int id)
    {
        try
        {
            var siblings = await _parentChildService.GetSiblingsAsync(id);
            return Ok(siblings);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parent-child relationship with ID {RelationshipId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving siblings for parent-child relationship with ID {RelationshipId}", id);
            return StatusCode(500, "An error occurred while retrieving siblings");
        }
    }
}
