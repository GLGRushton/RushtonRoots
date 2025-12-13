using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Models;

public class UserProfileViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PersonName { get; set; }
    
    public int? PersonId { get; set; }
}
