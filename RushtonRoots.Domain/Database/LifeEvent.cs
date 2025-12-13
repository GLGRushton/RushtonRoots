namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a significant event in a person's life.
/// Events include birth, death, marriage, education, career milestones, and other important moments.
/// </summary>
public class LifeEvent : BaseEntity
{
    public int PersonId { get; set; }
    public string EventType { get; set; } = string.Empty; // Birth, Death, Marriage, Education, Career, Military, Immigration, etc.
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? EventDate { get; set; }
    public int? LocationId { get; set; }
    public int? SourceId { get; set; }
    
    // Navigation properties
    public Person? Person { get; set; }
    public Location? Location { get; set; }
    public Source? Source { get; set; }
}
