namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for searching persons with advanced criteria.
/// </summary>
public class SearchPersonRequest
{
    /// <summary>
    /// Basic search term for name matching
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by household
    /// </summary>
    public int? HouseholdId { get; set; }
    
    /// <summary>
    /// Filter by deceased status
    /// </summary>
    public bool? IsDeceased { get; set; }
    
    /// <summary>
    /// Filter by birth date range - start
    /// </summary>
    public DateTime? BirthDateFrom { get; set; }
    
    /// <summary>
    /// Filter by birth date range - end
    /// </summary>
    public DateTime? BirthDateTo { get; set; }
    
    /// <summary>
    /// Filter by death date range - start
    /// </summary>
    public DateTime? DeathDateFrom { get; set; }
    
    /// <summary>
    /// Filter by death date range - end
    /// </summary>
    public DateTime? DeathDateTo { get; set; }
    
    /// <summary>
    /// Filter by location ID (searches life events)
    /// </summary>
    public int? LocationId { get; set; }
    
    /// <summary>
    /// Filter by event type (searches life events)
    /// </summary>
    public string? EventType { get; set; }
    
    /// <summary>
    /// Filter by surname only
    /// </summary>
    public string? Surname { get; set; }
}
