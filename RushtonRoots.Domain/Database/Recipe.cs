namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a family recipe that can be shared across generations.
/// Recipes preserve family culinary traditions and favorite dishes.
/// </summary>
public class Recipe : BaseEntity
{
    /// <summary>
    /// Name of the recipe
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the recipe
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Brief description of the recipe
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// List of ingredients (stored as JSON or delimited string)
    /// </summary>
    public string Ingredients { get; set; } = string.Empty;

    /// <summary>
    /// Step-by-step cooking instructions
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Preparation time in minutes
    /// </summary>
    public int? PrepTimeMinutes { get; set; }

    /// <summary>
    /// Cooking time in minutes
    /// </summary>
    public int? CookTimeMinutes { get; set; }

    /// <summary>
    /// Number of servings this recipe makes
    /// </summary>
    public int? Servings { get; set; }

    /// <summary>
    /// Category (e.g., Appetizer, Main Course, Dessert, Beverage)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Cuisine type (e.g., Italian, Mexican, American)
    /// </summary>
    public string? Cuisine { get; set; }

    /// <summary>
    /// URL to recipe photo
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Special notes or tips for making the recipe
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Person who originally created or contributed this recipe (if known)
    /// </summary>
    public int? OriginatorPersonId { get; set; }

    /// <summary>
    /// Navigation property to person who originated the recipe
    /// </summary>
    public Person? OriginatorPerson { get; set; }

    /// <summary>
    /// User who submitted the recipe to the system
    /// </summary>
    public string SubmittedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to submitter
    /// </summary>
    public ApplicationUser? SubmittedByUser { get; set; }

    /// <summary>
    /// Whether the recipe is published (visible to family members)
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Whether the recipe is a family favorite
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Average rating of the recipe (calculated from ratings)
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Total number of ratings
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    /// Number of times this recipe has been viewed
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Collection of ratings for this recipe
    /// </summary>
    public ICollection<RecipeRating> Ratings { get; set; } = new List<RecipeRating>();
}
