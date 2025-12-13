using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Requests;

/// <summary>
/// Request model for creating a new partnership.
/// </summary>
public class CreatePartnershipRequest
{
    [Required(ErrorMessage = "First person is required")]
    public int PersonAId { get; set; }
    
    [Required(ErrorMessage = "Second person is required")]
    public int PersonBId { get; set; }
    
    [Required(ErrorMessage = "Partnership type is required")]
    [MaxLength(50)]
    public string PartnershipType { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
}
