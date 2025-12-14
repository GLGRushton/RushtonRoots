namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a rating and optional comment on a family recipe.
/// </summary>
public class RecipeRating : BaseEntity
{
    /// <summary>
    /// ID of the recipe being rated
    /// </summary>
    public int RecipeId { get; set; }

    /// <summary>
    /// Navigation property to the recipe
    /// </summary>
    public Recipe? Recipe { get; set; }

    /// <summary>
    /// User who provided the rating
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Rating value (1-5 stars)
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment about the recipe
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Whether the user has made this recipe
    /// </summary>
    public bool HasMade { get; set; }
}
