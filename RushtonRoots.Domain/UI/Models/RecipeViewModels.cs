namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for a recipe
/// </summary>
public class RecipeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Ingredients { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int? PrepTimeMinutes { get; set; }
    public int? CookTimeMinutes { get; set; }
    public int? Servings { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Cuisine { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Notes { get; set; }
    public int? OriginatorPersonId { get; set; }
    public string? OriginatorPersonName { get; set; }
    public string SubmittedByUserId { get; set; } = string.Empty;
    public string? SubmittedByUserName { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFavorite { get; set; }
    public decimal AverageRating { get; set; }
    public int RatingCount { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public List<RecipeRatingViewModel> Ratings { get; set; } = new List<RecipeRatingViewModel>();
    
    // Computed properties
    public int? TotalTimeMinutes => (PrepTimeMinutes ?? 0) + (CookTimeMinutes ?? 0);
}

/// <summary>
/// View model for a recipe rating
/// </summary>
public class RecipeRatingViewModel
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool HasMade { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}

/// <summary>
/// Search result for recipes
/// </summary>
public class RecipeSearchResult
{
    public List<RecipeViewModel> Recipes { get; set; } = new List<RecipeViewModel>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
