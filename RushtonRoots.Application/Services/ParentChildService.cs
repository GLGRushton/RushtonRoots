using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service for ParentChild operations.
/// </summary>
public class ParentChildService : IParentChildService
{
    private readonly IParentChildRepository _repository;
    private readonly IPersonRepository _personRepository;

    public ParentChildService(IParentChildRepository repository, IPersonRepository personRepository)
    {
        _repository = repository;
        _personRepository = personRepository;
    }

    public async Task<ParentChildViewModel?> GetByIdAsync(int id)
    {
        var parentChild = await _repository.GetByIdAsync(id);
        return parentChild != null ? MapToViewModel(parentChild) : null;
    }

    public async Task<IEnumerable<ParentChildViewModel>> GetAllAsync()
    {
        var relationships = await _repository.GetAllAsync();
        return relationships.Select(MapToViewModel);
    }

    public async Task<IEnumerable<ParentChildViewModel>> GetByParentIdAsync(int parentId)
    {
        var relationships = await _repository.GetByParentIdAsync(parentId);
        return relationships.Select(MapToViewModel);
    }

    public async Task<IEnumerable<ParentChildViewModel>> GetByChildIdAsync(int childId)
    {
        var relationships = await _repository.GetByChildIdAsync(childId);
        return relationships.Select(MapToViewModel);
    }

    public async Task<ParentChildViewModel> CreateAsync(CreateParentChildRequest request)
    {
        // Validation
        if (request.ParentPersonId == request.ChildPersonId)
        {
            throw new ValidationException("A person cannot be their own parent");
        }

        // Check if both persons exist
        var parent = await _personRepository.GetByIdAsync(request.ParentPersonId);
        if (parent == null)
        {
            throw new NotFoundException($"Parent with ID {request.ParentPersonId} not found");
        }

        var child = await _personRepository.GetByIdAsync(request.ChildPersonId);
        if (child == null)
        {
            throw new NotFoundException($"Child with ID {request.ChildPersonId} not found");
        }

        // Check if relationship already exists
        if (await _repository.RelationshipExistsAsync(request.ParentPersonId, request.ChildPersonId))
        {
            throw new ValidationException("This parent-child relationship already exists");
        }

        // Check for circular relationships
        if (await _repository.HasCircularRelationshipAsync(request.ParentPersonId, request.ChildPersonId))
        {
            throw new ValidationException("This relationship would create a circular family tree");
        }

        var parentChild = new ParentChild
        {
            ParentPersonId = request.ParentPersonId,
            ChildPersonId = request.ChildPersonId,
            RelationshipType = request.RelationshipType
        };

        var created = await _repository.AddAsync(parentChild);
        var result = await _repository.GetByIdAsync(created.Id);
        return MapToViewModel(result!);
    }

    public async Task<ParentChildViewModel> UpdateAsync(UpdateParentChildRequest request)
    {
        var existing = await _repository.GetByIdAsync(request.Id);
        if (existing == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {request.Id} not found");
        }

        // Validation
        if (request.ParentPersonId == request.ChildPersonId)
        {
            throw new ValidationException("A person cannot be their own parent");
        }

        // Check if both persons exist
        var parent = await _personRepository.GetByIdAsync(request.ParentPersonId);
        if (parent == null)
        {
            throw new NotFoundException($"Parent with ID {request.ParentPersonId} not found");
        }

        var child = await _personRepository.GetByIdAsync(request.ChildPersonId);
        if (child == null)
        {
            throw new NotFoundException($"Child with ID {request.ChildPersonId} not found");
        }

        // Check for circular relationships (only if persons are changing)
        if (existing.ParentPersonId != request.ParentPersonId || existing.ChildPersonId != request.ChildPersonId)
        {
            if (await _repository.HasCircularRelationshipAsync(request.ParentPersonId, request.ChildPersonId))
            {
                throw new ValidationException("This relationship would create a circular family tree");
            }
        }

        existing.ParentPersonId = request.ParentPersonId;
        existing.ChildPersonId = request.ChildPersonId;
        existing.RelationshipType = request.RelationshipType;

        await _repository.UpdateAsync(existing);
        var result = await _repository.GetByIdAsync(request.Id);
        return MapToViewModel(result!);
    }

    public async Task DeleteAsync(int id)
    {
        var parentChild = await _repository.GetByIdAsync(id);
        if (parentChild == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {id} not found");
        }

        await _repository.DeleteAsync(id);
    }

    private static ParentChildViewModel MapToViewModel(ParentChild parentChild)
    {
        // Calculate child's age if birth date is available
        int? childAge = null;
        if (parentChild.ChildPerson?.DateOfBirth.HasValue == true)
        {
            var today = DateTime.Today;
            var birthDate = parentChild.ChildPerson.DateOfBirth.Value;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            childAge = age;
        }

        return new ParentChildViewModel
        {
            Id = parentChild.Id,
            ParentPersonId = parentChild.ParentPersonId,
            ChildPersonId = parentChild.ChildPersonId,
            ParentName = parentChild.ParentPerson != null 
                ? $"{parentChild.ParentPerson.FirstName} {parentChild.ParentPerson.LastName}" 
                : "Unknown",
            ChildName = parentChild.ChildPerson != null 
                ? $"{parentChild.ChildPerson.FirstName} {parentChild.ChildPerson.LastName}" 
                : "Unknown",
            ParentPhotoUrl = parentChild.ParentPerson?.PhotoUrl,
            ChildPhotoUrl = parentChild.ChildPerson?.PhotoUrl,
            ChildBirthDate = parentChild.ChildPerson?.DateOfBirth,
            ChildAge = childAge,
            RelationshipType = parentChild.RelationshipType,
            // TODO: Implement verification logic when verification feature is added
            // For now, all relationships are marked as verified by default
            IsVerified = true,
            CreatedDateTime = parentChild.CreatedDateTime,
            UpdatedDateTime = parentChild.UpdatedDateTime
        };
    }
}
