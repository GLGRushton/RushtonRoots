namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a source of genealogical information.
/// Sources provide evidence for facts, events, and relationships.
/// </summary>
public class Source : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? RepositoryName { get; set; } // Archive, library, website, etc.
    public string? RepositoryUrl { get; set; }
    public string? CallNumber { get; set; }
    public string SourceType { get; set; } = string.Empty; // Document, Book, Website, Interview, etc.
    public string? Notes { get; set; }
    
    // Navigation properties
    public ICollection<Citation> Citations { get; set; } = new List<Citation>();
    public ICollection<LifeEvent> LifeEvents { get; set; } = new List<LifeEvent>();
    public ICollection<BiographicalNote> BiographicalNotes { get; set; } = new List<BiographicalNote>();
}
