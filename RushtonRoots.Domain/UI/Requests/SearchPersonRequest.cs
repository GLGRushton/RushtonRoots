namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for searching persons.
/// </summary>
public class SearchPersonRequest
{
    public string? SearchTerm { get; set; }
    public int? HouseholdId { get; set; }
    public bool? IsDeceased { get; set; }
}
