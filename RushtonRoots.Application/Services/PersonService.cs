using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Validators;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service implementation for Person operations.
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IPersonMapper _personMapper;
    private readonly IPersonValidator _personValidator;

    public PersonService(
        IPersonRepository personRepository,
        IPersonMapper personMapper,
        IPersonValidator personValidator)
    {
        _personRepository = personRepository;
        _personMapper = personMapper;
        _personValidator = personValidator;
    }

    public async Task<PersonViewModel?> GetByIdAsync(int id)
    {
        var person = await _personRepository.GetByIdAsync(id);
        return person != null ? _personMapper.MapToViewModel(person) : null;
    }

    public async Task<IEnumerable<PersonViewModel>> GetAllAsync()
    {
        var people = await _personRepository.GetAllAsync();
        return people.Select(p => _personMapper.MapToViewModel(p));
    }

    public async Task<IEnumerable<PersonViewModel>> SearchAsync(SearchPersonRequest request)
    {
        var people = await _personRepository.SearchAsync(request);
        return people.Select(p => _personMapper.MapToViewModel(p));
    }

    public async Task<IEnumerable<PersonViewModel>> GetByHouseholdIdAsync(int householdId)
    {
        var people = await _personRepository.GetByHouseholdIdAsync(householdId);
        return people.Select(p => _personMapper.MapToViewModel(p));
    }

    public async Task<PersonViewModel> CreateAsync(CreatePersonRequest request)
    {
        var validationResult = await _personValidator.ValidateCreateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(string.Join(", ", validationResult.Errors));
        }

        var person = _personMapper.MapToEntity(request);
        var savedPerson = await _personRepository.AddAsync(person);

        // Reload to get navigation properties
        var reloadedPerson = await _personRepository.GetByIdAsync(savedPerson.Id);
        return _personMapper.MapToViewModel(reloadedPerson!);
    }

    public async Task<PersonViewModel> UpdateAsync(UpdatePersonRequest request)
    {
        var validationResult = await _personValidator.ValidateUpdateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(string.Join(", ", validationResult.Errors));
        }

        var person = await _personRepository.GetByIdAsync(request.Id);
        if (person == null)
        {
            throw new NotFoundException($"Person with ID {request.Id} not found.");
        }

        _personMapper.MapToEntity(request, person);
        var updatedPerson = await _personRepository.UpdateAsync(person);

        // Reload to get navigation properties
        var reloadedPerson = await _personRepository.GetByIdAsync(updatedPerson.Id);
        return _personMapper.MapToViewModel(reloadedPerson!);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _personRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new NotFoundException($"Person with ID {id} not found.");
        }

        await _personRepository.DeleteAsync(id);
    }
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
