using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// API controller for Recipe operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    /// <summary>
    /// Get all recipes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool publishedOnly = false)
    {
        var recipes = await _recipeService.GetAllAsync(publishedOnly);
        return Ok(recipes);
    }

    /// <summary>
    /// Get recipe by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool incrementViewCount = true)
    {
        var recipe = await _recipeService.GetByIdAsync(id, incrementViewCount);
        if (recipe == null)
        {
            return NotFound();
        }
        return Ok(recipe);
    }

    /// <summary>
    /// Get recipe by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool incrementViewCount = true)
    {
        var recipe = await _recipeService.GetBySlugAsync(slug, incrementViewCount);
        if (recipe == null)
        {
            return NotFound();
        }
        return Ok(recipe);
    }

    /// <summary>
    /// Search recipes with filters
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRecipeRequest request)
    {
        var result = await _recipeService.SearchAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Get recipes by category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category, [FromQuery] bool publishedOnly = true)
    {
        var recipes = await _recipeService.GetByCategoryAsync(category, publishedOnly);
        return Ok(recipes);
    }

    /// <summary>
    /// Get recipes by cuisine
    /// </summary>
    [HttpGet("cuisine/{cuisine}")]
    public async Task<IActionResult> GetByCuisine(string cuisine, [FromQuery] bool publishedOnly = true)
    {
        var recipes = await _recipeService.GetByCuisineAsync(cuisine, publishedOnly);
        return Ok(recipes);
    }

    /// <summary>
    /// Get favorite recipes
    /// </summary>
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites([FromQuery] bool publishedOnly = true)
    {
        var recipes = await _recipeService.GetFavoritesAsync(publishedOnly);
        return Ok(recipes);
    }

    /// <summary>
    /// Get recent recipes
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 10, [FromQuery] bool publishedOnly = true)
    {
        var recipes = await _recipeService.GetRecentAsync(count, publishedOnly);
        return Ok(recipes);
    }

    /// <summary>
    /// Get recipes by person (originator)
    /// </summary>
    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPerson(int personId, [FromQuery] bool publishedOnly = true)
    {
        var recipes = await _recipeService.GetByPersonAsync(personId, publishedOnly);
        return Ok(recipes);
    }

    /// <summary>
    /// Get all recipe categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _recipeService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get all recipe cuisines
    /// </summary>
    [HttpGet("cuisines")]
    public async Task<IActionResult> GetCuisines()
    {
        var cuisines = await _recipeService.GetCuisinesAsync();
        return Ok(cuisines);
    }

    /// <summary>
    /// Create a new recipe
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateRecipeRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var recipe = await _recipeService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
    }

    /// <summary>
    /// Update an existing recipe
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecipeRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var recipe = await _recipeService.UpdateAsync(id, request, userId);
            return Ok(recipe);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a recipe
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _recipeService.DeleteAsync(id);
        return NoContent();
    }
}

/// <summary>
/// API controller for RecipeRating operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecipeRatingController : ControllerBase
{
    private readonly IRecipeRatingService _ratingService;

    public RecipeRatingController(IRecipeRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    /// <summary>
    /// Get rating by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rating = await _ratingService.GetByIdAsync(id);
        if (rating == null)
        {
            return NotFound();
        }
        return Ok(rating);
    }

    /// <summary>
    /// Get user's rating for a recipe
    /// </summary>
    [HttpGet("recipe/{recipeId}/user")]
    public async Task<IActionResult> GetByRecipeAndUser(int recipeId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var rating = await _ratingService.GetByRecipeAndUserAsync(recipeId, userId);
        if (rating == null)
        {
            return NotFound();
        }
        return Ok(rating);
    }

    /// <summary>
    /// Get all ratings for a recipe
    /// </summary>
    [HttpGet("recipe/{recipeId}")]
    public async Task<IActionResult> GetByRecipe(int recipeId)
    {
        var ratings = await _ratingService.GetByRecipeAsync(recipeId);
        return Ok(ratings);
    }

    /// <summary>
    /// Create a new rating
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecipeRatingRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var rating = await _ratingService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing rating
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecipeRatingRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            var rating = await _ratingService.UpdateAsync(id, request, userId);
            return Ok(rating);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Delete a rating
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _ratingService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
