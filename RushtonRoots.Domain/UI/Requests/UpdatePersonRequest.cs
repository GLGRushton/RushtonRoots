using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for updating an existing person.
/// </summary>
public class UpdatePersonRequest
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int HouseholdId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; }
    
    public DateTime? DateOfDeath { get; set; }
    
    public bool IsDeceased { get; set; }
    
    [StringLength(500)]
    public string? PhotoUrl { get; set; }
}
