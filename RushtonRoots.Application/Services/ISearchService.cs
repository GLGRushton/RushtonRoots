using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for advanced search and discovery operations.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Calculates the relationship path between two people (Find My Relative).
    /// </summary>
    Task<RelationshipPathViewModel?> FindRelationshipAsync(int personAId, int personBId);
    
    /// <summary>
    /// Gets surname distribution statistics.
    /// </summary>
    Task<IEnumerable<SurnameDistributionViewModel>> GetSurnameDistributionAsync();
    
    /// <summary>
    /// Finds all people associated with a specific location.
    /// </summary>
    Task<IEnumerable<PersonViewModel>> GetPeopleByLocationAsync(int locationId);
    
    /// <summary>
    /// Finds all people with a specific event type.
    /// </summary>
    Task<IEnumerable<PersonViewModel>> GetPeopleByEventTypeAsync(string eventType);
}
