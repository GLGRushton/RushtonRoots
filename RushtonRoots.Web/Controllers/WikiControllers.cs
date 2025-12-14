using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WikiCategoryController : ControllerBase
{
    private readonly IWikiCategoryService _wikiCategoryService;

    public WikiCategoryController(IWikiCategoryService wikiCategoryService)
    {
        _wikiCategoryService = wikiCategoryService;
    }

    /// <summary>
    /// Get all wiki categories
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _wikiCategoryService.GetAllAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get root categories (no parent)
    /// </summary>
    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories()
    {
        var categories = await _wikiCategoryService.GetRootCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get wiki category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _wikiCategoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Get wiki category by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var category = await _wikiCategoryService.GetBySlugAsync(slug);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new wiki category
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateWikiCategoryRequest request)
    {
        var category = await _wikiCategoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update an existing wiki category
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWikiCategoryRequest request)
    {
        try
        {
            var category = await _wikiCategoryService.UpdateAsync(id, request);
            return Ok(category);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a wiki category
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _wikiCategoryService.DeleteAsync(id);
        return NoContent();
    }
}

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WikiTagController : ControllerBase
{
    private readonly IWikiTagService _wikiTagService;

    public WikiTagController(IWikiTagService wikiTagService)
    {
        _wikiTagService = wikiTagService;
    }

    /// <summary>
    /// Get all wiki tags
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _wikiTagService.GetAllAsync();
        return Ok(tags);
    }

    /// <summary>
    /// Get popular wiki tags
    /// </summary>
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopular([FromQuery] int count = 20)
    {
        var tags = await _wikiTagService.GetPopularTagsAsync(count);
        return Ok(tags);
    }

    /// <summary>
    /// Get wiki tag by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tag = await _wikiTagService.GetByIdAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        return Ok(tag);
    }

    /// <summary>
    /// Get wiki tag by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var tag = await _wikiTagService.GetBySlugAsync(slug);
        if (tag == null)
        {
            return NotFound();
        }
        return Ok(tag);
    }

    /// <summary>
    /// Create a new wiki tag
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateWikiTagRequest request)
    {
        var tag = await _wikiTagService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
    }

    /// <summary>
    /// Delete a wiki tag
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _wikiTagService.DeleteAsync(id);
        return NoContent();
    }
}

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WikiTemplateController : ControllerBase
{
    private readonly IWikiTemplateService _wikiTemplateService;

    public WikiTemplateController(IWikiTemplateService wikiTemplateService)
    {
        _wikiTemplateService = wikiTemplateService;
    }

    /// <summary>
    /// Get all wiki templates
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
    {
        var templates = await _wikiTemplateService.GetAllAsync(activeOnly);
        return Ok(templates);
    }

    /// <summary>
    /// Get templates by type
    /// </summary>
    [HttpGet("type/{templateType}")]
    public async Task<IActionResult> GetByType(string templateType)
    {
        var templates = await _wikiTemplateService.GetByTypeAsync(templateType);
        return Ok(templates);
    }

    /// <summary>
    /// Get wiki template by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var template = await _wikiTemplateService.GetByIdAsync(id);
        if (template == null)
        {
            return NotFound();
        }
        return Ok(template);
    }

    /// <summary>
    /// Create a new wiki template
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateWikiTemplateRequest request)
    {
        var template = await _wikiTemplateService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
    }

    /// <summary>
    /// Update an existing wiki template
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWikiTemplateRequest request)
    {
        try
        {
            var template = await _wikiTemplateService.UpdateAsync(id, request);
            return Ok(template);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a wiki template
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _wikiTemplateService.DeleteAsync(id);
        return NoContent();
    }
}
