using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using System.Text.RegularExpressions;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for Recipe operations
/// </summary>
public interface IRecipeService
{
    Task<RecipeViewModel?> GetByIdAsync(int id, bool incrementViewCount = false);
    Task<RecipeViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false);
    Task<IEnumerable<RecipeViewModel>> GetAllAsync(bool publishedOnly = false);
    Task<IEnumerable<RecipeViewModel>> GetByCategoryAsync(string category, bool publishedOnly = true);
    Task<IEnumerable<RecipeViewModel>> GetByCuisineAsync(string cuisine, bool publishedOnly = true);
    Task<IEnumerable<RecipeViewModel>> GetFavoritesAsync(bool publishedOnly = true);
    Task<IEnumerable<RecipeViewModel>> GetRecentAsync(int count, bool publishedOnly = true);
    Task<IEnumerable<RecipeViewModel>> GetByPersonAsync(int personId, bool publishedOnly = true);
    Task<RecipeSearchResult> SearchAsync(SearchRecipeRequest request);
    Task<RecipeViewModel> CreateAsync(CreateRecipeRequest request, string userId);
    Task<RecipeViewModel> UpdateAsync(int id, UpdateRecipeRequest request, string userId);
    Task DeleteAsync(int id);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<string>> GetCuisinesAsync();
}

/// <summary>
/// Service interface for RecipeRating operations
/// </summary>
public interface IRecipeRatingService
{
    Task<RecipeRatingViewModel?> GetByIdAsync(int id);
    Task<RecipeRatingViewModel?> GetByRecipeAndUserAsync(int recipeId, string userId);
    Task<IEnumerable<RecipeRatingViewModel>> GetByRecipeAsync(int recipeId);
    Task<RecipeRatingViewModel> CreateAsync(CreateRecipeRatingRequest request, string userId);
    Task<RecipeRatingViewModel> UpdateAsync(int id, UpdateRecipeRatingRequest request, string userId);
    Task DeleteAsync(int id);
}

/// <summary>
/// Service implementation for Recipe operations
/// </summary>
public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly RushtonRoots.Infrastructure.Database.RushtonRootsDbContext _context;

    public RecipeService(IRecipeRepository recipeRepository, RushtonRoots.Infrastructure.Database.RushtonRootsDbContext context)
    {
        _recipeRepository = recipeRepository;
        _context = context;
    }

    public async Task<RecipeViewModel?> GetByIdAsync(int id, bool incrementViewCount = false)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id, includeRelated: true);
        if (recipe == null) return null;

        if (incrementViewCount)
        {
            await _recipeRepository.IncrementViewCountAsync(id);
            recipe.ViewCount++;
        }

        return MapToViewModel(recipe);
    }

    public async Task<RecipeViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false)
    {
        var recipe = await _recipeRepository.GetBySlugAsync(slug, includeRelated: true);
        if (recipe == null) return null;

        if (incrementViewCount)
        {
            await _recipeRepository.IncrementViewCountAsync(recipe.Id);
            recipe.ViewCount++;
        }

        return MapToViewModel(recipe);
    }

    public async Task<IEnumerable<RecipeViewModel>> GetAllAsync(bool publishedOnly = false)
    {
        var recipes = await _recipeRepository.GetAllAsync(publishedOnly);
        return recipes.Select(MapToViewModel);
    }

    public async Task<IEnumerable<RecipeViewModel>> GetByCategoryAsync(string category, bool publishedOnly = true)
    {
        var recipes = await _recipeRepository.GetByCategoryAsync(category, publishedOnly);
        return recipes.Select(MapToViewModel);
    }

    public async Task<IEnumerable<RecipeViewModel>> GetByCuisineAsync(string cuisine, bool publishedOnly = true)
    {
        var recipes = await _recipeRepository.GetByCuisineAsync(cuisine, publishedOnly);
        return recipes.Select(MapToViewModel);
    }

    public async Task<IEnumerable<RecipeViewModel>> GetFavoritesAsync(bool publishedOnly = true)
    {
        var recipes = await _recipeRepository.GetFavoritesAsync(publishedOnly);
        return recipes.Select(MapToViewModel);
    }

    public async Task<IEnumerable<RecipeViewModel>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var recipes = await _recipeRepository.GetRecentAsync(count, publishedOnly);
        return recipes.Select(MapToViewModel);
    }

    public async Task<IEnumerable<RecipeViewModel>> GetByPersonAsync(int personId, bool publishedOnly = true)
    {
        var recipes = await _recipeRepository.GetByPersonAsync(personId, publishedOnly);
        return recipes.Select(MapToViewModel);
    }

    public async Task<RecipeSearchResult> SearchAsync(SearchRecipeRequest request)
    {
        var query = _context.Recipes
            .Include(r => r.SubmittedByUser)
            .Include(r => r.OriginatorPerson)
            .AsQueryable();

        // Apply filters
        if (request.IsPublished.HasValue)
        {
            query = query.Where(r => r.IsPublished == request.IsPublished.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(searchTerm) ||
                r.Ingredients.ToLower().Contains(searchTerm) ||
                r.Instructions.ToLower().Contains(searchTerm) ||
                (r.Description != null && r.Description.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(r => r.Category == request.Category);
        }

        if (!string.IsNullOrWhiteSpace(request.Cuisine))
        {
            query = query.Where(r => r.Cuisine == request.Cuisine);
        }

        if (request.OriginatorPersonId.HasValue)
        {
            query = query.Where(r => r.OriginatorPersonId == request.OriginatorPersonId.Value);
        }

        if (request.IsFavorite.HasValue)
        {
            query = query.Where(r => r.IsFavorite == request.IsFavorite.Value);
        }

        if (request.MaxPrepTime.HasValue)
        {
            query = query.Where(r => r.PrepTimeMinutes <= request.MaxPrepTime.Value);
        }

        if (request.MaxCookTime.HasValue)
        {
            query = query.Where(r => r.CookTimeMinutes <= request.MaxCookTime.Value);
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(r => r.AverageRating >= request.MinRating.Value);
        }

        // Get total count before sorting and paging
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = request.SortBy.ToLower() switch
        {
            "rating" => request.SortDescending
                ? query.OrderByDescending(r => r.AverageRating)
                : query.OrderBy(r => r.AverageRating),
            "viewcount" => request.SortDescending
                ? query.OrderByDescending(r => r.ViewCount)
                : query.OrderBy(r => r.ViewCount),
            "createddatetime" => request.SortDescending
                ? query.OrderByDescending(r => r.CreatedDateTime)
                : query.OrderBy(r => r.CreatedDateTime),
            "updateddatetime" => request.SortDescending
                ? query.OrderByDescending(r => r.UpdatedDateTime)
                : query.OrderBy(r => r.UpdatedDateTime),
            _ => request.SortDescending
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name)
        };

        // Apply paging
        var recipes = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new RecipeSearchResult
        {
            Recipes = recipes.Select(MapToViewModel).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<RecipeViewModel> CreateAsync(CreateRecipeRequest request, string userId)
    {
        var recipe = new Recipe
        {
            Name = request.Name,
            Slug = await GenerateUniqueSlugAsync(request.Name),
            Description = request.Description,
            Ingredients = request.Ingredients,
            Instructions = request.Instructions,
            PrepTimeMinutes = request.PrepTimeMinutes,
            CookTimeMinutes = request.CookTimeMinutes,
            Servings = request.Servings,
            Category = request.Category,
            Cuisine = request.Cuisine,
            PhotoUrl = request.PhotoUrl,
            Notes = request.Notes,
            OriginatorPersonId = request.OriginatorPersonId,
            SubmittedByUserId = userId,
            IsPublished = request.IsPublished,
            IsFavorite = request.IsFavorite,
            AverageRating = 0,
            RatingCount = 0,
            ViewCount = 0
        };

        await _recipeRepository.AddAsync(recipe);

        // Reload with related data
        recipe = await _recipeRepository.GetByIdAsync(recipe.Id, includeRelated: true);
        return MapToViewModel(recipe!);
    }

    public async Task<RecipeViewModel> UpdateAsync(int id, UpdateRecipeRequest request, string userId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
            throw new InvalidOperationException($"Recipe with ID {id} not found");

        recipe.Name = request.Name;
        recipe.Slug = await GenerateUniqueSlugAsync(request.Name, id);
        recipe.Description = request.Description;
        recipe.Ingredients = request.Ingredients;
        recipe.Instructions = request.Instructions;
        recipe.PrepTimeMinutes = request.PrepTimeMinutes;
        recipe.CookTimeMinutes = request.CookTimeMinutes;
        recipe.Servings = request.Servings;
        recipe.Category = request.Category;
        recipe.Cuisine = request.Cuisine;
        recipe.PhotoUrl = request.PhotoUrl;
        recipe.Notes = request.Notes;
        recipe.OriginatorPersonId = request.OriginatorPersonId;
        recipe.IsPublished = request.IsPublished;
        recipe.IsFavorite = request.IsFavorite;

        await _recipeRepository.UpdateAsync(recipe);

        // Reload with related data
        recipe = await _recipeRepository.GetByIdAsync(recipe.Id, includeRelated: true);
        return MapToViewModel(recipe!);
    }

    public async Task DeleteAsync(int id)
    {
        await _recipeRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await _context.Recipes
            .Where(r => r.IsPublished)
            .Select(r => r.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetCuisinesAsync()
    {
        return await _context.Recipes
            .Where(r => r.IsPublished && r.Cuisine != null)
            .Select(r => r.Cuisine!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null)
    {
        var baseSlug = Regex.Replace(name.ToLower(), @"[^a-z0-9\s-]", "");
        baseSlug = Regex.Replace(baseSlug, @"\s+", "-").Trim('-');

        var slug = baseSlug;
        var counter = 1;

        while (true)
        {
            var existing = await _context.Recipes
                .Where(r => r.Slug == slug && (!excludeId.HasValue || r.Id != excludeId.Value))
                .AnyAsync();

            if (!existing)
                break;

            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    private RecipeViewModel MapToViewModel(Recipe recipe)
    {
        return new RecipeViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Slug = recipe.Slug,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Category = recipe.Category,
            Cuisine = recipe.Cuisine,
            PhotoUrl = recipe.PhotoUrl,
            Notes = recipe.Notes,
            OriginatorPersonId = recipe.OriginatorPersonId,
            OriginatorPersonName = recipe.OriginatorPerson != null
                ? $"{recipe.OriginatorPerson.FirstName} {recipe.OriginatorPerson.LastName}"
                : null,
            SubmittedByUserId = recipe.SubmittedByUserId,
            SubmittedByUserName = recipe.SubmittedByUser?.UserName,
            IsPublished = recipe.IsPublished,
            IsFavorite = recipe.IsFavorite,
            AverageRating = recipe.AverageRating,
            RatingCount = recipe.RatingCount,
            ViewCount = recipe.ViewCount,
            CreatedDateTime = recipe.CreatedDateTime,
            UpdatedDateTime = recipe.UpdatedDateTime,
            Ratings = recipe.Ratings?.Select(MapRatingToViewModel).ToList() ?? new List<RecipeRatingViewModel>()
        };
    }

    private RecipeRatingViewModel MapRatingToViewModel(RecipeRating rating)
    {
        return new RecipeRatingViewModel
        {
            Id = rating.Id,
            RecipeId = rating.RecipeId,
            UserId = rating.UserId,
            UserName = rating.User?.UserName,
            Rating = rating.Rating,
            Comment = rating.Comment,
            HasMade = rating.HasMade,
            CreatedDateTime = rating.CreatedDateTime,
            UpdatedDateTime = rating.UpdatedDateTime
        };
    }
}

/// <summary>
/// Service implementation for RecipeRating operations
/// </summary>
public class RecipeRatingService : IRecipeRatingService
{
    private readonly IRecipeRatingRepository _ratingRepository;
    private readonly IRecipeRepository _recipeRepository;

    public RecipeRatingService(IRecipeRatingRepository ratingRepository, IRecipeRepository recipeRepository)
    {
        _ratingRepository = ratingRepository;
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeRatingViewModel?> GetByIdAsync(int id)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        return rating != null ? MapToViewModel(rating) : null;
    }

    public async Task<RecipeRatingViewModel?> GetByRecipeAndUserAsync(int recipeId, string userId)
    {
        var rating = await _ratingRepository.GetByRecipeAndUserAsync(recipeId, userId);
        return rating != null ? MapToViewModel(rating) : null;
    }

    public async Task<IEnumerable<RecipeRatingViewModel>> GetByRecipeAsync(int recipeId)
    {
        var ratings = await _ratingRepository.GetByRecipeAsync(recipeId);
        return ratings.Select(MapToViewModel);
    }

    public async Task<RecipeRatingViewModel> CreateAsync(CreateRecipeRatingRequest request, string userId)
    {
        // Check if user already rated this recipe
        var existing = await _ratingRepository.GetByRecipeAndUserAsync(request.RecipeId, userId);
        if (existing != null)
            throw new InvalidOperationException("User has already rated this recipe. Use update instead.");

        var rating = new RecipeRating
        {
            RecipeId = request.RecipeId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            HasMade = request.HasMade
        };

        await _ratingRepository.AddAsync(rating);

        // Update recipe average rating
        await UpdateRecipeRatingAsync(request.RecipeId);

        return MapToViewModel((await _ratingRepository.GetByIdAsync(rating.Id))!);
    }

    public async Task<RecipeRatingViewModel> UpdateAsync(int id, UpdateRecipeRatingRequest request, string userId)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
            throw new InvalidOperationException($"Rating with ID {id} not found");

        if (rating.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own ratings");

        rating.Rating = request.Rating;
        rating.Comment = request.Comment;
        rating.HasMade = request.HasMade;

        await _ratingRepository.UpdateAsync(rating);

        // Update recipe average rating
        await UpdateRecipeRatingAsync(rating.RecipeId);

        return MapToViewModel((await _ratingRepository.GetByIdAsync(id))!);
    }

    public async Task DeleteAsync(int id)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
            throw new InvalidOperationException($"Rating with ID {id} not found");

        var recipeId = rating.RecipeId;

        await _ratingRepository.DeleteAsync(id);

        // Update recipe average rating
        await UpdateRecipeRatingAsync(recipeId);
    }

    private async Task UpdateRecipeRatingAsync(int recipeId)
    {
        var ratings = await _ratingRepository.GetByRecipeAsync(recipeId);
        var ratingsList = ratings.ToList();

        if (ratingsList.Any())
        {
            var average = ratingsList.Average(r => r.Rating);
            await _recipeRepository.UpdateRatingAsync(recipeId, (decimal)average, ratingsList.Count);
        }
        else
        {
            await _recipeRepository.UpdateRatingAsync(recipeId, 0, 0);
        }
    }

    private RecipeRatingViewModel MapToViewModel(RecipeRating rating)
    {
        return new RecipeRatingViewModel
        {
            Id = rating.Id,
            RecipeId = rating.RecipeId,
            UserId = rating.UserId,
            UserName = rating.User?.UserName,
            Rating = rating.Rating,
            Comment = rating.Comment,
            HasMade = rating.HasMade,
            CreatedDateTime = rating.CreatedDateTime,
            UpdatedDateTime = rating.UpdatedDateTime
        };
    }
}
