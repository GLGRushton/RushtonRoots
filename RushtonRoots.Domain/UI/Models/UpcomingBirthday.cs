namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// Represents an upcoming birthday event
/// </summary>
public class UpcomingBirthday
{
    public int PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime NextBirthday { get; set; }
    public int Age { get; set; }
    public int DaysUntilBirthday { get; set; }
}
