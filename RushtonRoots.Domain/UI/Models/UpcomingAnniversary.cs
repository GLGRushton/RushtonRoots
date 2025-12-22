namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// Represents an upcoming partnership anniversary
/// </summary>
public class UpcomingAnniversary
{
    public int PartnershipId { get; set; }
    public string PersonAName { get; set; } = string.Empty;
    public string PersonBName { get; set; } = string.Empty;
    public string? PersonAPhotoUrl { get; set; }
    public string? PersonBPhotoUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextAnniversary { get; set; }
    public int YearsMarried { get; set; }
    public int DaysUntilAnniversary { get; set; }
}
