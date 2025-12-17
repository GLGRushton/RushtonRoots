namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for displaying person information.
/// </summary>
public class PersonViewModel
{
    public int Id { get; set; }
    public int? HouseholdId { get; set; }
    public string HouseholdName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public string FullName => $"{FirstName} {(string.IsNullOrWhiteSpace(MiddleName) ? "" : MiddleName + " ")}{LastName}{(string.IsNullOrWhiteSpace(Suffix) ? "" : " " + Suffix)}".Trim();
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public string? PlaceOfDeath { get; set; }
    public bool IsDeceased { get; set; }
    public string? Biography { get; set; }
    public string? Occupation { get; set; }
    public string? Education { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    
    // Relationships
    public IEnumerable<PartnershipViewModel> Partnerships { get; set; } = new List<PartnershipViewModel>();
    public IEnumerable<ParentChildViewModel> ParentRelationships { get; set; } = new List<ParentChildViewModel>();
    public IEnumerable<ParentChildViewModel> ChildRelationships { get; set; } = new List<ParentChildViewModel>();
}
