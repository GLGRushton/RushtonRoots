using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Recipe entities
/// </summary>
public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(int id, bool includeRelated = false);
    Task<Recipe?> GetBySlugAsync(string slug, bool includeRelated = false);
    Task<IEnumerable<Recipe>> GetAllAsync(bool publishedOnly = false);
    Task<IEnumerable<Recipe>> GetByCategoryAsync(string category, bool publishedOnly = true);
    Task<IEnumerable<Recipe>> GetByCuisineAsync(string cuisine, bool publishedOnly = true);
    Task<IEnumerable<Recipe>> GetFavoritesAsync(bool publishedOnly = true);
    Task<IEnumerable<Recipe>> GetRecentAsync(int count, bool publishedOnly = true);
    Task<IEnumerable<Recipe>> GetByPersonAsync(int personId, bool publishedOnly = true);
    Task<Recipe> AddAsync(Recipe recipe);
    Task<Recipe> UpdateAsync(Recipe recipe);
    Task DeleteAsync(int id);
    Task IncrementViewCountAsync(int id);
    Task UpdateRatingAsync(int id, decimal averageRating, int ratingCount);
}

/// <summary>
/// Repository interface for RecipeRating entities
/// </summary>
public interface IRecipeRatingRepository
{
    Task<RecipeRating?> GetByIdAsync(int id);
    Task<RecipeRating?> GetByRecipeAndUserAsync(int recipeId, string userId);
    Task<IEnumerable<RecipeRating>> GetByRecipeAsync(int recipeId);
    Task<RecipeRating> AddAsync(RecipeRating rating);
    Task<RecipeRating> UpdateAsync(RecipeRating rating);
    Task DeleteAsync(int id);
}
