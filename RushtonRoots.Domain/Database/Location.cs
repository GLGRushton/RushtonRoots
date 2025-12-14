namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a physical location or place associated with people and events.
/// Includes geocoding coordinates for mapping visualization.
/// </summary>
public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Navigation properties
    public ICollection<LifeEvent> LifeEvents { get; set; } = new List<LifeEvent>();
}
