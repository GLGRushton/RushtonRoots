namespace RushtonRoots.Domain.Database;

/// <summary>
/// Links specific facts/fields to their supporting citations
/// </summary>
public class FactCitation : BaseEntity
{
    public string EntityType { get; set; } = string.Empty; // Person, Partnership, LifeEvent, etc.
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty; // Which field this citation supports
    public int CitationId { get; set; }
    public string ConfidenceLevel { get; set; } = "Medium"; // Low, Medium, High, Proven
    public string? Notes { get; set; } // Additional notes about how this citation supports the fact
    public string AddedByUserId { get; set; } = string.Empty;

    // Navigation properties
    public Citation? Citation { get; set; }
    public ApplicationUser? AddedBy { get; set; }
}
