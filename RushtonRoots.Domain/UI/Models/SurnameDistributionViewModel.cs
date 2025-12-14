namespace RushtonRoots.Domain.UI.Models;

/// <summary>
/// Represents surname distribution statistics.
/// </summary>
public class SurnameDistributionViewModel
{
    public string Surname { get; set; } = string.Empty;
    public int Count { get; set; }
    public int LivingCount { get; set; }
    public int DeceasedCount { get; set; }
}
