using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service for Partnership operations.
/// </summary>
public class PartnershipService : IPartnershipService
{
    private readonly IPartnershipRepository _repository;
    private readonly IPersonRepository _personRepository;

    public PartnershipService(IPartnershipRepository repository, IPersonRepository personRepository)
    {
        _repository = repository;
        _personRepository = personRepository;
    }

    public async Task<PartnershipViewModel?> GetByIdAsync(int id)
    {
        var partnership = await _repository.GetByIdAsync(id);
        return partnership != null ? MapToViewModel(partnership) : null;
    }

    public async Task<IEnumerable<PartnershipViewModel>> GetAllAsync()
    {
        var partnerships = await _repository.GetAllAsync();
        return partnerships.Select(MapToViewModel);
    }

    public async Task<IEnumerable<PartnershipViewModel>> GetByPersonIdAsync(int personId)
    {
        var partnerships = await _repository.GetByPersonIdAsync(personId);
        return partnerships.Select(MapToViewModel);
    }

    public async Task<PartnershipViewModel> CreateAsync(CreatePartnershipRequest request)
    {
        // Validation
        if (request.PersonAId == request.PersonBId)
        {
            throw new ValidationException("A person cannot be partnered with themselves");
        }

        // Check if both persons exist
        var personA = await _personRepository.GetByIdAsync(request.PersonAId);
        if (personA == null)
        {
            throw new NotFoundException($"Person with ID {request.PersonAId} not found");
        }

        var personB = await _personRepository.GetByIdAsync(request.PersonBId);
        if (personB == null)
        {
            throw new NotFoundException($"Person with ID {request.PersonBId} not found");
        }

        // Check if partnership already exists
        if (await _repository.PartnershipExistsAsync(request.PersonAId, request.PersonBId))
        {
            throw new ValidationException("A partnership already exists between these two people");
        }

        // Validate dates
        if (request.StartDate.HasValue && request.EndDate.HasValue && request.EndDate < request.StartDate)
        {
            throw new ValidationException("End date cannot be before start date");
        }

        var partnership = new Partnership
        {
            PersonAId = request.PersonAId,
            PersonBId = request.PersonBId,
            PartnershipType = request.PartnershipType,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var created = await _repository.AddAsync(partnership);
        var result = await _repository.GetByIdAsync(created.Id);
        return MapToViewModel(result!);
    }

    public async Task<PartnershipViewModel> UpdateAsync(UpdatePartnershipRequest request)
    {
        var existing = await _repository.GetByIdAsync(request.Id);
        if (existing == null)
        {
            throw new NotFoundException($"Partnership with ID {request.Id} not found");
        }

        // Validation
        if (request.PersonAId == request.PersonBId)
        {
            throw new ValidationException("A person cannot be partnered with themselves");
        }

        // Check if both persons exist
        var personA = await _personRepository.GetByIdAsync(request.PersonAId);
        if (personA == null)
        {
            throw new NotFoundException($"Person with ID {request.PersonAId} not found");
        }

        var personB = await _personRepository.GetByIdAsync(request.PersonBId);
        if (personB == null)
        {
            throw new NotFoundException($"Person with ID {request.PersonBId} not found");
        }

        // Validate dates
        if (request.StartDate.HasValue && request.EndDate.HasValue && request.EndDate < request.StartDate)
        {
            throw new ValidationException("End date cannot be before start date");
        }

        existing.PersonAId = request.PersonAId;
        existing.PersonBId = request.PersonBId;
        existing.PartnershipType = request.PartnershipType;
        existing.StartDate = request.StartDate;
        existing.EndDate = request.EndDate;

        await _repository.UpdateAsync(existing);
        var result = await _repository.GetByIdAsync(request.Id);
        return MapToViewModel(result!);
    }

    public async Task DeleteAsync(int id)
    {
        var partnership = await _repository.GetByIdAsync(id);
        if (partnership == null)
        {
            throw new NotFoundException($"Partnership with ID {id} not found");
        }

        await _repository.DeleteAsync(id);
    }

    private static PartnershipViewModel MapToViewModel(Partnership partnership)
    {
        return new PartnershipViewModel
        {
            Id = partnership.Id,
            PersonAId = partnership.PersonAId,
            PersonBId = partnership.PersonBId,
            PersonAName = partnership.PersonA != null 
                ? $"{partnership.PersonA.FirstName} {partnership.PersonA.LastName}" 
                : "Unknown",
            PersonBName = partnership.PersonB != null 
                ? $"{partnership.PersonB.FirstName} {partnership.PersonB.LastName}" 
                : "Unknown",
            PartnershipType = partnership.PartnershipType,
            StartDate = partnership.StartDate,
            EndDate = partnership.EndDate,
            CreatedDateTime = partnership.CreatedDateTime,
            UpdatedDateTime = partnership.UpdatedDateTime
        };
    }
}
