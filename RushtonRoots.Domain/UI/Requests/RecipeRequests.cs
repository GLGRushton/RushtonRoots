namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request to create a new recipe
/// </summary>
public class CreateRecipeRequest
{
    public string Name { get; set; } = string.Empty;
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
    public bool IsPublished { get; set; }
    public bool IsFavorite { get; set; }
}

/// <summary>
/// Request to update an existing recipe
/// </summary>
public class UpdateRecipeRequest
{
    public string Name { get; set; } = string.Empty;
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
    public bool IsPublished { get; set; }
    public bool IsFavorite { get; set; }
}

/// <summary>
/// Request to search recipes
/// </summary>
public class SearchRecipeRequest
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? Cuisine { get; set; }
    public int? OriginatorPersonId { get; set; }
    public bool? IsFavorite { get; set; }
    public bool? IsPublished { get; set; }
    public int? MaxPrepTime { get; set; }
    public int? MaxCookTime { get; set; }
    public decimal? MinRating { get; set; }
    public string SortBy { get; set; } = "Name"; // Name, Rating, ViewCount, CreatedDateTime, UpdatedDateTime
    public bool SortDescending { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Request to create or update a recipe rating
/// </summary>
public class CreateRecipeRatingRequest
{
    public int RecipeId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public bool HasMade { get; set; }
}

/// <summary>
/// Request to update an existing recipe rating
/// </summary>
public class UpdateRecipeRatingRequest
{
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public bool HasMade { get; set; }
}
