using System.ComponentModel.DataAnnotations;

namespace RushtonRoots.Domain.UI.Models;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
