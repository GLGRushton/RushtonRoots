namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// View model for admin dashboard system statistics
/// </summary>
public class AdminStatistics
{
    /// <summary>
    /// Total number of registered users in the system
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Total number of households
    /// </summary>
    public int TotalHouseholds { get; set; }

    /// <summary>
    /// Total number of persons in the family tree
    /// </summary>
    public int TotalPersons { get; set; }

    /// <summary>
    /// Total number of media items (photos, documents, etc.)
    /// </summary>
    public int MediaItems { get; set; }
}
