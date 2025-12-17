using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for creating a new person.
/// </summary>
public class CreatePersonRequest
{
    public int? HouseholdId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? MiddleName { get; set; }
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string? Suffix { get; set; }
    
    [StringLength(50)]
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [StringLength(200)]
    public string? PlaceOfBirth { get; set; }
    
    public DateTime? DateOfDeath { get; set; }
    
    [StringLength(200)]
    public string? PlaceOfDeath { get; set; }
    
    public bool IsDeceased { get; set; }
    
    [StringLength(5000)]
    public string? Biography { get; set; }
    
    [StringLength(200)]
    public string? Occupation { get; set; }
    
    [StringLength(500)]
    public string? Education { get; set; }
    
    [StringLength(2000)]
    public string? Notes { get; set; }
    
    [StringLength(500)]
    public string? PhotoUrl { get; set; }
}
