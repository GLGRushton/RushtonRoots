using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Web.Controllers.Api;

/// <summary>
/// API Controller for Family Tree data and visualizations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FamilyTreeController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IParentChildService _parentChildService;
    private readonly IPartnershipService _partnershipService;
    private readonly IFamilyTreeService _familyTreeService;
    private readonly ILogger<FamilyTreeController> _logger;

    public FamilyTreeController(
        IPersonService personService,
        IParentChildService parentChildService,
        IPartnershipService partnershipService,
        IFamilyTreeService familyTreeService,
        ILogger<FamilyTreeController> logger)
    {
        _personService = personService;
        _parentChildService = parentChildService;
        _partnershipService = partnershipService;
        _familyTreeService = familyTreeService;
        _logger = logger;
    }

    /// <summary>
    /// Get mini family tree for a specific person.
    /// Includes parents, grandparents, children, and spouses.
    /// </summary>
    /// <param name="personId">Person ID to focus on</param>
    /// <param name="generations">Number of generations to include (default: 2)</param>
    /// <returns>Family tree node with relationships</returns>
    [HttpGet("mini/{personId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FamilyTreeNodeViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FamilyTreeNodeViewModel>> GetMiniTree(
        int personId,
        [FromQuery] int generations = 2)
    {
        try
        {
            var tree = await _familyTreeService.GetMiniTreeAsync(personId, generations);
            
            if (tree == null)
            {
                _logger.LogWarning("Person with ID {PersonId} not found for family tree", personId);
                return NotFound($"Person with ID {personId} not found");
            }
            
            return Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mini family tree for person {PersonId}", personId);
            return StatusCode(500, "An error occurred while retrieving the family tree");
        }
    }

    /// <summary>
    /// Get mini family tree for the current logged-in user.
    /// If not logged in or user not associated with a person, returns tree for youngest person.
    /// </summary>
    /// <returns>Family tree node for current user or youngest person</returns>
    [HttpGet("mini/current")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FamilyTreeNodeViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FamilyTreeNodeViewModel>> GetCurrentUserMiniTree()
    {
        try
        {
            // Get current user ID if authenticated
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var tree = await _familyTreeService.GetMiniTreeForCurrentUserAsync(userId);
            
            if (tree == null)
            {
                _logger.LogWarning("No person found for family tree (user: {UserId})", userId ?? "anonymous");
                return NotFound("No person found for family tree");
            }
            
            return Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mini family tree for current user");
            return StatusCode(500, "An error occurred while retrieving the family tree");
        }
    }

    /// <summary>
    /// Gets pedigree chart data for a specific person (ancestors).
    /// </summary>
    [HttpGet("pedigree/{personId}")]
    public async Task<IActionResult> GetPedigree(int personId, [FromQuery] int generations = 4)
    {
        try
        {
            var person = await _personService.GetByIdAsync(personId);
            if (person == null)
            {
                return NotFound(new { message = $"Person with ID {personId} not found" });
            }

            var pedigreeData = await BuildPedigreeData(personId, generations);
            return Ok(pedigreeData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pedigree for person {PersonId}", personId);
            return StatusCode(500, new { message = "An error occurred while retrieving pedigree data" });
        }
    }

    /// <summary>
    /// Gets descendant chart data for a specific person.
    /// </summary>
    [HttpGet("descendants/{personId}")]
    public async Task<IActionResult> GetDescendants(int personId, [FromQuery] int generations = 3)
    {
        try
        {
            var person = await _personService.GetByIdAsync(personId);
            if (person == null)
            {
                return NotFound(new { message = $"Person with ID {personId} not found" });
            }

            var descendantData = await BuildDescendantData(personId, generations);
            return Ok(descendantData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting descendants for person {PersonId}", personId);
            return StatusCode(500, new { message = "An error occurred while retrieving descendant data" });
        }
    }

    /// <summary>
    /// Gets all people and relationships for building a complete family tree.
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllFamilyData()
    {
        try
        {
            var people = await _personService.GetAllAsync();
            var parentChildRelationships = await _parentChildService.GetAllAsync();
            var partnerships = await _partnershipService.GetAllAsync();

            return Ok(new
            {
                people,
                parentChildRelationships,
                partnerships
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all family data");
            return StatusCode(500, new { message = "An error occurred while retrieving family data" });
        }
    }

    private async Task<object> BuildPedigreeData(int personId, int generations)
    {
        var pedigree = new Dictionary<string, object>();
        await BuildPedigreeRecursive(personId, generations, 0, pedigree);
        return pedigree;
    }

    private async Task BuildPedigreeRecursive(int personId, int maxGenerations, int currentGeneration, Dictionary<string, object> pedigree)
    {
        if (currentGeneration >= maxGenerations)
        {
            return;
        }

        var person = await _personService.GetByIdAsync(personId);
        if (person == null)
        {
            return;
        }

        pedigree["person"] = person;
        pedigree["generation"] = currentGeneration;

        // Get parents
        var parentRelationships = await _parentChildService.GetByChildIdAsync(personId);
        var parents = new List<Dictionary<string, object>>();

        foreach (var rel in parentRelationships)
        {
            var parentData = new Dictionary<string, object>();
            await BuildPedigreeRecursive(rel.ParentPersonId, maxGenerations, currentGeneration + 1, parentData);
            if (parentData.ContainsKey("person"))
            {
                parents.Add(parentData);
            }
        }

        if (parents.Any())
        {
            pedigree["parents"] = parents;
        }
    }

    private async Task<object> BuildDescendantData(int personId, int generations)
    {
        var descendant = new Dictionary<string, object>();
        await BuildDescendantRecursive(personId, generations, 0, descendant);
        return descendant;
    }

    private async Task BuildDescendantRecursive(int personId, int maxGenerations, int currentGeneration, Dictionary<string, object> descendant)
    {
        if (currentGeneration >= maxGenerations)
        {
            return;
        }

        var person = await _personService.GetByIdAsync(personId);
        if (person == null)
        {
            return;
        }

        descendant["person"] = person;
        descendant["generation"] = currentGeneration;

        // Get partner if any
        var partnerships = person.Partnerships?.ToList() ?? new List<PartnershipViewModel>();
        if (partnerships.Any())
        {
            var partnership = partnerships.First();
            var partnerId = partnership.PersonAId == personId ? partnership.PersonBId : partnership.PersonAId;
            var partner = await _personService.GetByIdAsync(partnerId);
            if (partner != null)
            {
                descendant["partner"] = partner;
            }
        }

        // Get children
        var childRelationships = await _parentChildService.GetByParentIdAsync(personId);
        var children = new List<Dictionary<string, object>>();

        foreach (var rel in childRelationships)
        {
            var childData = new Dictionary<string, object>();
            await BuildDescendantRecursive(rel.ChildPersonId, maxGenerations, currentGeneration + 1, childData);
            if (childData.ContainsKey("person"))
            {
                children.Add(childData);
            }
        }

        if (children.Any())
        {
            descendant["children"] = children;
        }
    }
}
