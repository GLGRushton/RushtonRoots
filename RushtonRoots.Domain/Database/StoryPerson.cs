namespace RushtonRoots.Domain.Database;

/// <summary>
/// Join table for many-to-many relationship between stories and people.
/// Associates stories with the people they are about.
/// </summary>
public class StoryPerson
{
    /// <summary>
    /// ID of the story
    /// </summary>
    public int StoryId { get; set; }

    /// <summary>
    /// Navigation property to story
    /// </summary>
    public Story? Story { get; set; }

    /// <summary>
    /// ID of the person
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Navigation property to person
    /// </summary>
    public Person? Person { get; set; }

    /// <summary>
    /// Optional note about this person's role in the story
    /// </summary>
    public string? RoleInStory { get; set; }
}
