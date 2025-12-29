using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for family tree operations.
/// </summary>
public interface IFamilyTreeService
{
    /// <summary>
    /// Get mini family tree for a specific person.
    /// Includes parents, grandparents, children, and spouses.
    /// </summary>
    /// <param name="personId">Focus person ID</param>
    /// <param name="generations">Number of generations to include (default: 2)</param>
    /// <returns>Family tree node with the person as root</returns>
    Task<FamilyTreeNodeViewModel?> GetMiniTreeAsync(int personId, int generations = 2);
    
    /// <summary>
    /// Get mini family tree for the current logged-in user.
    /// Falls back to youngest person if user not found or not associated with a person.
    /// </summary>
    /// <param name="userId">Current user ID (null if not logged in)</param>
    /// <returns>Family tree node</returns>
    Task<FamilyTreeNodeViewModel?> GetMiniTreeForCurrentUserAsync(string? userId);
    
    /// <summary>
    /// Get the youngest person in the database.
    /// </summary>
    /// <returns>Person ID of the youngest person, or null if no persons exist</returns>
    Task<int?> GetYoungestPersonIdAsync();
}
