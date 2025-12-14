using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Recipe entities
/// </summary>
public class RecipeRepository : IRecipeRepository
{
    private readonly RushtonRootsDbContext _context;

    public RecipeRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe?> GetByIdAsync(int id, bool includeRelated = false)
    {
        var query = _context.Recipes.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(r => r.SubmittedByUser)
                .Include(r => r.OriginatorPerson)
                .Include(r => r.Ratings)
                    .ThenInclude(rr => rr.User);
        }

        return await query.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Recipe?> GetBySlugAsync(string slug, bool includeRelated = false)
    {
        var query = _context.Recipes.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(r => r.SubmittedByUser)
                .Include(r => r.OriginatorPerson)
                .Include(r => r.Ratings)
                    .ThenInclude(rr => rr.User);
        }

        return await query.FirstOrDefaultAsync(r => r.Slug == slug);
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync(bool publishedOnly = false)
    {
        var query = _context.Recipes
            .Include(r => r.SubmittedByUser)
            .Include(r => r.OriginatorPerson)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(r => r.IsPublished);
        }

        return await query.OrderByDescending(r => r.UpdatedDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByCategoryAsync(string category, bool publishedOnly = true)
    {
        var query = _context.Recipes
            .Include(r => r.SubmittedByUser)
            .Include(r => r.OriginatorPerson)
            .Where(r => r.Category == category);

        if (publishedOnly)
        {
            query = query.Where(r => r.IsPublished);
        }

        return await query.OrderByDescending(r => r.AverageRating).ThenByDescending(r => r.UpdatedDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByCuisineAsync(string cuisine, bool publishedOnly = true)
    {
        var query = _context.Recipes
            .Include(r => r.SubmittedByUser)
            .Include(r => r.OriginatorPerson)
            .Where(r => r.Cuisine == cuisine);

        if (publishedOnly)
        {
            query = query.Where(r => r.IsPublished);
        }

        return await query.OrderByDescending(r => r.AverageRating).ThenByDescending(r => r.UpdatedDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetFavoritesAsync(bool publishedOnly = true)
    {
        var query = _context.Recipes
            .Include(r => r.SubmittedByUser)
            .Include(r => r.OriginatorPerson)
            .Where(r => r.IsFavorite);

        if (publishedOnly)
        {
            query = query.Where(r => r.IsPublished);
        }

        return await query.OrderByDescending(r => r.AverageRating).ThenBy(r => r.Name).ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var query = _context.Recipes
            .Include(r => r.SubmittedByUser)
            .Include(r => r.OriginatorPerson)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(r => r.IsPublished);
        }

        return await query.OrderByDescending(r => r.CreatedDateTime).Take(count).ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByPersonAsync(int personId, bool publishedOnly = true)
    {
        var query = _context.Recipes
            .Include(r => r.SubmittedByUser)
            .Include(r => r.OriginatorPerson)
            .Where(r => r.OriginatorPersonId == personId);

        if (publishedOnly)
        {
            query = query.Where(r => r.IsPublished);
        }

        return await query.OrderBy(r => r.Name).ToListAsync();
    }

    public async Task<Recipe> AddAsync(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recipe> UpdateAsync(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task DeleteAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe != null)
        {
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
        }
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe != null)
        {
            recipe.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateRatingAsync(int id, decimal averageRating, int ratingCount)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe != null)
        {
            recipe.AverageRating = averageRating;
            recipe.RatingCount = ratingCount;
            await _context.SaveChangesAsync();
        }
    }
}

/// <summary>
/// Repository implementation for RecipeRating entities
/// </summary>
public class RecipeRatingRepository : IRecipeRatingRepository
{
    private readonly RushtonRootsDbContext _context;

    public RecipeRatingRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<RecipeRating?> GetByIdAsync(int id)
    {
        return await _context.RecipeRatings
            .Include(rr => rr.User)
            .Include(rr => rr.Recipe)
            .FirstOrDefaultAsync(rr => rr.Id == id);
    }

    public async Task<RecipeRating?> GetByRecipeAndUserAsync(int recipeId, string userId)
    {
        return await _context.RecipeRatings
            .Include(rr => rr.User)
            .FirstOrDefaultAsync(rr => rr.RecipeId == recipeId && rr.UserId == userId);
    }

    public async Task<IEnumerable<RecipeRating>> GetByRecipeAsync(int recipeId)
    {
        return await _context.RecipeRatings
            .Include(rr => rr.User)
            .Where(rr => rr.RecipeId == recipeId)
            .OrderByDescending(rr => rr.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<RecipeRating> AddAsync(RecipeRating rating)
    {
        _context.RecipeRatings.Add(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task<RecipeRating> UpdateAsync(RecipeRating rating)
    {
        _context.RecipeRatings.Update(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task DeleteAsync(int id)
    {
        var rating = await _context.RecipeRatings.FindAsync(id);
        if (rating != null)
        {
            _context.RecipeRatings.Remove(rating);
            await _context.SaveChangesAsync();
        }
    }
}
