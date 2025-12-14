using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service implementation for advanced search and discovery operations.
/// </summary>
public class SearchService : ISearchService
{
    private readonly IPersonRepository _personRepository;
    private readonly IParentChildRepository _parentChildRepository;
    private readonly IPartnershipRepository _partnershipRepository;
    private readonly IPersonMapper _personMapper;

    public SearchService(
        IPersonRepository personRepository,
        IParentChildRepository parentChildRepository,
        IPartnershipRepository partnershipRepository,
        IPersonMapper personMapper)
    {
        _personRepository = personRepository;
        _parentChildRepository = parentChildRepository;
        _partnershipRepository = partnershipRepository;
        _personMapper = personMapper;
    }

    public async Task<RelationshipPathViewModel?> FindRelationshipAsync(int personAId, int personBId)
    {
        // Validate both people exist
        var personA = await _personRepository.GetByIdAsync(personAId);
        var personB = await _personRepository.GetByIdAsync(personBId);

        if (personA == null || personB == null)
            return null;

        if (personAId == personBId)
        {
            return new RelationshipPathViewModel
            {
                PersonAId = personAId,
                PersonAName = $"{personA.FirstName} {personA.LastName}",
                PersonBId = personBId,
                PersonBName = $"{personB.FirstName} {personB.LastName}",
                RelationshipDescription = "Same person",
                Degree = 0,
                Steps = new List<RelationshipStepViewModel>()
            };
        }

        // Pre-load all people to avoid excessive database calls during BFS
        var allPeople = await _personRepository.GetAllAsync();
        var personCache = allPeople.ToDictionary(p => p.Id);

        // Use breadth-first search to find shortest relationship path
        var path = await FindShortestPathAsync(personAId, personBId, personCache);
        
        if (path == null)
            return null;

        return path;
    }

    private async Task<RelationshipPathViewModel?> FindShortestPathAsync(
        int startId, 
        int targetId, 
        Dictionary<int, Domain.Database.Person> personCache)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<(int PersonId, List<RelationshipStepViewModel> Path)>();
        queue.Enqueue((startId, new List<RelationshipStepViewModel>()));
        visited.Add(startId);

        while (queue.Count > 0)
        {
            var (currentId, currentPath) = queue.Dequeue();

            // Get all connected people
            var connections = await GetConnectedPeopleAsync(currentId);

            foreach (var (connectedId, relationType) in connections)
            {
                if (visited.Contains(connectedId))
                    continue;

                // Use cached person data instead of database calls
                if (!personCache.TryGetValue(currentId, out var currentPerson) ||
                    !personCache.TryGetValue(connectedId, out var connectedPerson))
                    continue;

                var newPath = new List<RelationshipStepViewModel>(currentPath)
                {
                    new RelationshipStepViewModel
                    {
                        FromPersonId = currentId,
                        FromPersonName = $"{currentPerson.FirstName} {currentPerson.LastName}",
                        ToPersonId = connectedId,
                        ToPersonName = $"{connectedPerson.FirstName} {connectedPerson.LastName}",
                        RelationType = relationType
                    }
                };

                // Found the target
                if (connectedId == targetId)
                {
                    var startPerson = personCache[startId];
                    var targetPerson = personCache[targetId];

                    return new RelationshipPathViewModel
                    {
                        PersonAId = startId,
                        PersonAName = $"{startPerson.FirstName} {startPerson.LastName}",
                        PersonBId = targetId,
                        PersonBName = $"{targetPerson.FirstName} {targetPerson.LastName}",
                        RelationshipDescription = GenerateRelationshipDescription(newPath),
                        Degree = newPath.Count,
                        Steps = newPath
                    };
                }

                visited.Add(connectedId);
                queue.Enqueue((connectedId, newPath));
            }
        }

        return null; // No path found
    }

    private async Task<List<(int PersonId, string RelationType)>> GetConnectedPeopleAsync(int personId)
    {
        var connections = new List<(int PersonId, string RelationType)>();

        // Get parents
        var parentRelationships = await _parentChildRepository.GetByChildIdAsync(personId);
        connections.AddRange(parentRelationships.Select(pc => (pc.ParentPersonId, "parent")));

        // Get children
        var childRelationships = await _parentChildRepository.GetByParentIdAsync(personId);
        connections.AddRange(childRelationships.Select(pc => (pc.ChildPersonId, "child")));

        // Get spouses/partners
        var partnerships = await _partnershipRepository.GetByPersonIdAsync(personId);
        foreach (var partnership in partnerships)
        {
            var partnerId = partnership.PersonAId == personId ? partnership.PersonBId : partnership.PersonAId;
            connections.Add((partnerId, "spouse"));
        }

        return connections;
    }

    private string GenerateRelationshipDescription(List<RelationshipStepViewModel> steps)
    {
        if (steps.Count == 0)
            return "Unknown relationship";

        if (steps.Count == 1)
        {
            return steps[0].RelationType switch
            {
                "parent" => "Parent",
                "child" => "Child",
                "spouse" => "Spouse/Partner",
                _ => "Related"
            };
        }

        if (steps.Count == 2)
        {
            var step1 = steps[0].RelationType;
            var step2 = steps[1].RelationType;

            if (step1 == "parent" && step2 == "child")
                return "Sibling";
            if (step1 == "child" && step2 == "parent")
                return "Sibling";
            if (step1 == "parent" && step2 == "parent")
                return "Grandparent";
            if (step1 == "child" && step2 == "child")
                return "Grandchild";
            if (step1 == "spouse" && step2 == "parent")
                return "Parent-in-law";
            if (step1 == "spouse" && step2 == "child")
                return "Child-in-law";
        }

        // For more complex relationships, just describe the degree
        var parentCount = steps.Count(s => s.RelationType == "parent");
        var childCount = steps.Count(s => s.RelationType == "child");

        if (parentCount > 0 && childCount == 0)
            return $"{GetOrdinal(parentCount)} generation ancestor";
        if (childCount > 0 && parentCount == 0)
            return $"{GetOrdinal(childCount)} generation descendant";

        return $"Related ({steps.Count} degrees of separation)";
    }

    private string GetOrdinal(int number)
    {
        return number switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{number}th"
        };
    }

    public async Task<IEnumerable<SurnameDistributionViewModel>> GetSurnameDistributionAsync()
    {
        var allPeople = await _personRepository.GetAllAsync();
        
        var distribution = allPeople
            .GroupBy(p => p.LastName.ToUpper())
            .Select(g => new SurnameDistributionViewModel
            {
                Surname = g.Key,
                Count = g.Count(),
                LivingCount = g.Count(p => !p.IsDeceased),
                DeceasedCount = g.Count(p => p.IsDeceased)
            })
            .OrderByDescending(d => d.Count)
            .ThenBy(d => d.Surname);

        return distribution;
    }

    public async Task<IEnumerable<PersonViewModel>> GetPeopleByLocationAsync(int locationId)
    {
        var allPeople = await _personRepository.GetAllAsync();
        
        // Filter people who have events at this location
        var peopleWithLocation = allPeople
            .Where(p => p.LifeEvents.Any(e => e.LocationId == locationId))
            .Select(p => _personMapper.MapToViewModel(p))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName);

        return peopleWithLocation;
    }

    public async Task<IEnumerable<PersonViewModel>> GetPeopleByEventTypeAsync(string eventType)
    {
        var allPeople = await _personRepository.GetAllAsync();
        
        var eventTypeLower = eventType.ToLower();
        
        // Filter people who have events of this type
        var peopleWithEventType = allPeople
            .Where(p => p.LifeEvents.Any(e => e.EventType.ToLower() == eventTypeLower))
            .Select(p => _personMapper.MapToViewModel(p))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName);

        return peopleWithEventType;
    }
}
