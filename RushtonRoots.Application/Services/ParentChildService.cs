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
    private readonly IParentChildMapper _mapper;

    public ParentChildService(
        IParentChildRepository repository, 
        IPersonRepository personRepository,
        IParentChildMapper mapper)
    {
        _repository = repository;
        _personRepository = personRepository;
        _mapper = mapper;
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
}
