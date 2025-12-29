using Microsoft.AspNetCore.Identity;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service implementation for family tree operations.
/// </summary>
public class FamilyTreeService : IFamilyTreeService
{
    private readonly IPersonRepository _personRepository;
    private readonly IParentChildRepository _parentChildRepository;
    private readonly IPartnershipRepository _partnershipRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public FamilyTreeService(
        IPersonRepository personRepository,
        IParentChildRepository parentChildRepository,
        IPartnershipRepository partnershipRepository,
        UserManager<ApplicationUser> userManager)
    {
        _personRepository = personRepository;
        _parentChildRepository = parentChildRepository;
        _partnershipRepository = partnershipRepository;
        _userManager = userManager;
    }

    public async Task<FamilyTreeNodeViewModel?> GetMiniTreeAsync(int personId, int generations = 2)
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            return null;
        }

        var focusNode = await BuildTreeNodeAsync(person, 0);
        
        // Load parents (generation -1)
        if (generations >= 1)
        {
            await LoadParentsAsync(focusNode, person.Id);
            
            // Load grandparents (generation -2)
            if (generations >= 2 && focusNode.Parents != null)
            {
                foreach (var parent in focusNode.Parents)
                {
                    await LoadParentsAsync(parent, parent.Id);
                }
            }
        }
        
        // Load children (generation 1)
        await LoadChildrenAsync(focusNode, person.Id);
        
        // Load spouses (generation 0)
        await LoadSpousesAsync(focusNode, person.Id);

        return focusNode;
    }

    public async Task<FamilyTreeNodeViewModel?> GetMiniTreeForCurrentUserAsync(string? userId)
    {
        int? personId = null;

        // Try to get person ID from current user
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user?.PersonId != null)
            {
                personId = user.PersonId;
            }
        }

        // If user not found or not associated with a person, get youngest person
        if (!personId.HasValue)
        {
            personId = await GetYoungestPersonIdAsync();
        }

        if (!personId.HasValue)
        {
            return null;
        }

        return await GetMiniTreeAsync(personId.Value);
    }

    public async Task<int?> GetYoungestPersonIdAsync()
    {
        var youngestPerson = await _personRepository.GetYoungestPersonAsync();
        return youngestPerson?.Id;
    }

    private async Task<FamilyTreeNodeViewModel> BuildTreeNodeAsync(Person person, int generation)
    {
        return new FamilyTreeNodeViewModel
        {
            Id = person.Id,
            Name = $"{person.FirstName} {person.LastName}".Trim(),
            PhotoUrl = person.PhotoUrl,
            BirthDate = person.DateOfBirth,
            DeathDate = person.DateOfDeath,
            Generation = generation,
            Parents = new List<FamilyTreeNodeViewModel>(),
            Children = new List<FamilyTreeNodeViewModel>(),
            Spouses = new List<FamilyTreeNodeViewModel>()
        };
    }

    private async Task LoadParentsAsync(FamilyTreeNodeViewModel node, int childId)
    {
        var parentRelationships = await _parentChildRepository.GetByChildIdAsync(childId);
        
        foreach (var relationship in parentRelationships.Where(r => !r.IsDeleted))
        {
            var parent = await _personRepository.GetByIdAsync(relationship.ParentPersonId);
            if (parent != null && !parent.IsDeleted)
            {
                var parentNode = await BuildTreeNodeAsync(parent, node.Generation - 1);
                node.Parents!.Add(parentNode);
            }
        }
    }

    private async Task LoadChildrenAsync(FamilyTreeNodeViewModel node, int parentId)
    {
        var childRelationships = await _parentChildRepository.GetByParentIdAsync(parentId);
        
        foreach (var relationship in childRelationships.Where(r => !r.IsDeleted))
        {
            var child = await _personRepository.GetByIdAsync(relationship.ChildPersonId);
            if (child != null && !child.IsDeleted)
            {
                var childNode = await BuildTreeNodeAsync(child, node.Generation + 1);
                node.Children!.Add(childNode);
            }
        }
    }

    private async Task LoadSpousesAsync(FamilyTreeNodeViewModel node, int personId)
    {
        var partnerships = await _partnershipRepository.GetByPersonIdAsync(personId);
        
        foreach (var partnership in partnerships.Where(p => !p.IsDeleted))
        {
            // Get the other person in the partnership
            var spouseId = partnership.PersonAId == personId 
                ? partnership.PersonBId 
                : partnership.PersonAId;
            
            var spouse = await _personRepository.GetByIdAsync(spouseId);
            if (spouse != null && !spouse.IsDeleted)
            {
                var spouseNode = await BuildTreeNodeAsync(spouse, node.Generation);
                node.Spouses!.Add(spouseNode);
            }
        }
    }
}
