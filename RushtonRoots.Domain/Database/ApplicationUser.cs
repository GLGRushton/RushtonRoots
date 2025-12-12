using Microsoft.AspNetCore.Identity;

namespace RushtonRoots.Domain.Database;

/// <summary>
/// Extends IdentityUser to link to a Person entity.
/// User authentication is handled by .NET Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public int? PersonId { get; set; }
    
    // Navigation property
    public Person? Person { get; set; }
}
