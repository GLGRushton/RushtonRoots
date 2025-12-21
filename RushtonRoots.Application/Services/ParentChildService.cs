using RushtonRoots.Application.Mappers;
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
    private readonly ILifeEventRepository _lifeEventRepository;
    private readonly IParentChildMapper _mapper;
    private readonly ISourceMapper _sourceMapper;
    private readonly ILifeEventMapper _lifeEventMapper;
    private readonly IPersonMapper _personMapper;

    public ParentChildService(
        IParentChildRepository repository, 
        IPersonRepository personRepository,
        ILifeEventRepository lifeEventRepository,
        IParentChildMapper mapper,
        ISourceMapper sourceMapper,
        ILifeEventMapper lifeEventMapper,
        IPersonMapper personMapper)
    {
        _repository = repository;
        _personRepository = personRepository;
        _lifeEventRepository = lifeEventRepository;
        _mapper = mapper;
        _sourceMapper = sourceMapper;
        _lifeEventMapper = lifeEventMapper;
        _personMapper = personMapper;
    }

    public async Task<ParentChildViewModel?> GetByIdAsync(int id)
    {
        var parentChild = await _repository.GetByIdAsync(id);
        return parentChild != null ? _mapper.MapToViewModel(parentChild) : null;
    }

    public async Task<IEnumerable<ParentChildViewModel>> GetAllAsync()
    {
        var relationships = await _repository.GetAllAsync();
        return relationships.Select(r => _mapper.MapToViewModel(r));
    }

    public async Task<IEnumerable<ParentChildViewModel>> GetByParentIdAsync(int parentId)
    {
        var relationships = await _repository.GetByParentIdAsync(parentId);
        return relationships.Select(r => _mapper.MapToViewModel(r));
    }

    public async Task<IEnumerable<ParentChildViewModel>> GetByChildIdAsync(int childId)
    {
        var relationships = await _repository.GetByChildIdAsync(childId);
        return relationships.Select(r => _mapper.MapToViewModel(r));
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

        var parentChild = _mapper.MapToEntity(request);

        var created = await _repository.AddAsync(parentChild);
        var result = await _repository.GetByIdAsync(created.Id);
        return _mapper.MapToViewModel(result!);
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

        _mapper.UpdateEntity(existing, request);

        await _repository.UpdateAsync(existing);
        var result = await _repository.GetByIdAsync(request.Id);
        return _mapper.MapToViewModel(result!);
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

    // Phase 4.2: Evidence & Family Context methods

    public async Task<IEnumerable<SourceViewModel>> GetEvidenceAsync(int relationshipId)
    {
        // Validate relationship exists
        var relationship = await _repository.GetByIdAsync(relationshipId);
        if (relationship == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {relationshipId} not found");
        }

        // Get sources linked to this relationship
        var sources = await _repository.GetSourcesAsync(relationshipId);
        return _sourceMapper.MapToViewModels(sources);
    }

    public async Task<IEnumerable<LifeEventViewModel>> GetRelatedEventsAsync(int relationshipId)
    {
        // Validate relationship exists
        var relationship = await _repository.GetByIdAsync(relationshipId);
        if (relationship == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {relationshipId} not found");
        }

        // Get life events for both parent and child
        var parentEvents = await _lifeEventRepository.GetByPersonIdAsync(relationship.ParentPersonId);
        var childEvents = await _lifeEventRepository.GetByPersonIdAsync(relationship.ChildPersonId);

        // Combine and map to view models
        var allEvents = parentEvents.Concat(childEvents).ToList();
        return _lifeEventMapper.MapToViewModels(allEvents);
    }

    public async Task<IEnumerable<PersonViewModel>> GetGrandparentsAsync(int relationshipId)
    {
        // Validate relationship exists
        var relationship = await _repository.GetByIdAsync(relationshipId);
        if (relationship == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {relationshipId} not found");
        }

        // Get grandparents
        var grandparents = await _repository.GetGrandparentsAsync(relationshipId);
        return grandparents.Select(p => _personMapper.MapToViewModel(p));
    }

    public async Task<IEnumerable<PersonViewModel>> GetSiblingsAsync(int relationshipId)
    {
        // Validate relationship exists
        var relationship = await _repository.GetByIdAsync(relationshipId);
        if (relationship == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {relationshipId} not found");
        }

        // Get siblings
        var siblings = await _repository.GetSiblingsAsync(relationshipId);
        return siblings.Select(p => _personMapper.MapToViewModel(p));
    }

    // Phase 4.3: Verification System methods

    public async Task<ParentChildViewModel> VerifyRelationshipAsync(int relationshipId, string verifiedBy)
    {
        // Validate relationship exists
        var relationship = await _repository.GetByIdAsync(relationshipId);
        if (relationship == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {relationshipId} not found");
        }

        // Validate verifiedBy parameter
        if (string.IsNullOrWhiteSpace(verifiedBy))
        {
            throw new ValidationException("VerifiedBy is required");
        }

        // Update verification fields
        relationship.IsVerified = true;
        relationship.VerifiedDate = DateTime.UtcNow;
        relationship.VerifiedBy = verifiedBy;

        // Save changes
        await _repository.UpdateAsync(relationship);

        // Return updated view model
        var result = await _repository.GetByIdAsync(relationshipId);
        return _mapper.MapToViewModel(result!);
    }

    public async Task<ParentChildViewModel> UpdateNotesAsync(int relationshipId, string notes)
    {
        // Validate relationship exists
        var relationship = await _repository.GetByIdAsync(relationshipId);
        if (relationship == null)
        {
            throw new NotFoundException($"Parent-child relationship with ID {relationshipId} not found");
        }

        // Validate notes parameter
        if (notes == null)
        {
            throw new ValidationException("Notes cannot be null");
        }

        if (notes.Length > 2000)
        {
            throw new ValidationException("Notes cannot exceed 2000 characters");
        }

        // Update notes
        relationship.Notes = notes;

        // Save changes
        await _repository.UpdateAsync(relationship);

        // Return updated view model
        var result = await _repository.GetByIdAsync(relationshipId);
        return _mapper.MapToViewModel(result!);
    }
}
