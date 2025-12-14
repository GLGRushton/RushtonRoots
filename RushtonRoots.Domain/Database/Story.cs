namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a family story or memory that can be associated with one or more people.
/// Stories preserve family history, traditions, and experiences for future generations.
/// </summary>
public class Story : BaseEntity
{
    /// <summary>
    /// Title of the story
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the story
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Main content of the story
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional summary or excerpt
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Category of the story (e.g., Childhood, WarStories, Recipes, Traditions, etc.)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Optional date when the events in the story occurred
    /// </summary>
    public DateTime? StoryDate { get; set; }

    /// <summary>
    /// Optional location where the story took place
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// User who submitted/created the story
    /// </summary>
    public string SubmittedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to submitter
    /// </summary>
    public ApplicationUser? SubmittedByUser { get; set; }

    /// <summary>
    /// Whether the story is published (visible to family members)
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Number of times this story has been viewed
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Whether this story allows collaborative editing
    /// </summary>
    public bool AllowCollaboration { get; set; }

    /// <summary>
    /// ID of the collection this story belongs to (if any)
    /// </summary>
    public int? CollectionId { get; set; }

    /// <summary>
    /// Navigation property to story collection
    /// </summary>
    public StoryCollection? Collection { get; set; }

    /// <summary>
    /// Many-to-many navigation to people associated with this story
    /// </summary>
    public ICollection<StoryPerson> StoryPeople { get; set; } = new List<StoryPerson>();
}
