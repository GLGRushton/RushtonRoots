using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// API Controller for Family Tree data and visualizations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FamilyTreeController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IParentChildService _parentChildService;
    private readonly IPartnershipService _partnershipService;
    private readonly ILogger<FamilyTreeController> _logger;

    public FamilyTreeController(
        IPersonService personService,
        IParentChildService parentChildService,
        IPartnershipService partnershipService,
        ILogger<FamilyTreeController> logger)
    {
        _personService = personService;
        _parentChildService = parentChildService;
        _partnershipService = partnershipService;
        _logger = logger;
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
