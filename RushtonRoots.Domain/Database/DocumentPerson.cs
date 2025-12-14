namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents an association between a document and a person.
/// Documents can be linked to multiple people (e.g., a marriage certificate for two people).
/// </summary>
public class DocumentPerson : BaseEntity
{
    public int DocumentId { get; set; }
    public int PersonId { get; set; }
    public string? Notes { get; set; } // Additional context about the association
    
    // Navigation properties
    public Document? Document { get; set; }
    public Person? Person { get; set; }
}
