namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents biographical information, stories, and notes about a person.
/// Can include personal anecdotes, character descriptions, achievements, and memories.
/// </summary>
public class BiographicalNote : BaseEntity
{
    public int PersonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; } // Person who wrote the note
    public int? SourceId { get; set; }
    
    // Navigation properties
    public Person? Person { get; set; }
    public Source? Source { get; set; }
}
