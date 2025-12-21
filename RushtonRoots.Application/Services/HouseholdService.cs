using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Validators;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service implementation for Household operations.
/// </summary>
public class HouseholdService : IHouseholdService
{
    private readonly IHouseholdRepository _householdRepository;
    private readonly IHouseholdMapper _householdMapper;
    private readonly IHouseholdValidator _householdValidator;

    public HouseholdService(
        IHouseholdRepository householdRepository,
        IHouseholdMapper householdMapper,
        IHouseholdValidator householdValidator)
    {
        _householdRepository = householdRepository;
        _householdMapper = householdMapper;
        _householdValidator = householdValidator;
    }

    public async Task<HouseholdViewModel?> GetByIdAsync(int id)
    {
        var household = await _householdRepository.GetByIdAsync(id);
        if (household == null) return null;

        var memberCount = await _householdRepository.GetMemberCountAsync(id);
        return _householdMapper.MapToViewModel(household, memberCount);
    }

    public async Task<IEnumerable<HouseholdViewModel>> GetAllAsync()
    {
        var households = await _householdRepository.GetAllAsync();
        var viewModels = new List<HouseholdViewModel>();

        foreach (var household in households)
        {
            var memberCount = await _householdRepository.GetMemberCountAsync(household.Id);
            viewModels.Add(_householdMapper.MapToViewModel(household, memberCount));
        }

        return viewModels;
    }

    public async Task<HouseholdViewModel> CreateAsync(CreateHouseholdRequest request)
    {
        var validationResult = await _householdValidator.ValidateCreateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(string.Join(", ", validationResult.Errors));
        }

        var household = _householdMapper.MapToEntity(request);
        var savedHousehold = await _householdRepository.AddAsync(household);

        // Reload to get navigation properties
        var reloadedHousehold = await _householdRepository.GetByIdAsync(savedHousehold.Id);
        var memberCount = await _householdRepository.GetMemberCountAsync(savedHousehold.Id);
        return _householdMapper.MapToViewModel(reloadedHousehold!, memberCount);
    }

    public async Task<HouseholdViewModel> UpdateAsync(UpdateHouseholdRequest request)
    {
        var validationResult = await _householdValidator.ValidateUpdateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(string.Join(", ", validationResult.Errors));
        }

        var household = await _householdRepository.GetByIdAsync(request.Id);
        if (household == null)
        {
            throw new NotFoundException($"Household with ID {request.Id} not found.");
        }

        _householdMapper.MapToEntity(request, household);
        var updatedHousehold = await _householdRepository.UpdateAsync(household);

        // Reload to get navigation properties
        var reloadedHousehold = await _householdRepository.GetByIdAsync(updatedHousehold.Id);
        var memberCount = await _householdRepository.GetMemberCountAsync(updatedHousehold.Id);
        return _householdMapper.MapToViewModel(reloadedHousehold!, memberCount);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _householdRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new NotFoundException($"Household with ID {id} not found.");
        }

        await _householdRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PersonViewModel>> GetMembersAsync(int householdId)
    {
        var exists = await _householdRepository.ExistsAsync(householdId);
        if (!exists)
        {
            throw new NotFoundException($"Household with ID {householdId} not found.");
        }

        var members = await _householdRepository.GetMembersAsync(householdId);
        // We don't have a person mapper here, so we'll return a simple mapping
        // In a real implementation, you'd inject IPersonMapper and use it
        return members.Select(m => new PersonViewModel
        {
            Id = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            MiddleName = m.MiddleName,
            Suffix = m.Suffix,
            Gender = m.Gender,
            DateOfBirth = m.DateOfBirth,
            PlaceOfBirth = m.PlaceOfBirth,
            DateOfDeath = m.DateOfDeath,
            PlaceOfDeath = m.PlaceOfDeath,
            IsDeceased = m.IsDeceased,
            Biography = m.Biography,
            Occupation = m.Occupation,
            Education = m.Education,
            Notes = m.Notes,
            PhotoUrl = m.PhotoUrl,
            HouseholdId = m.HouseholdId,
            CreatedDateTime = m.CreatedDateTime,
            UpdatedDateTime = m.UpdatedDateTime
        });
    }

    public async Task AddMemberAsync(AddHouseholdMemberRequest request)
    {
        // Basic validation
        if (request.HouseholdId <= 0)
        {
            throw new ValidationException("Invalid household ID.");
        }

        if (request.PersonId <= 0)
        {
            throw new ValidationException("Invalid person ID.");
        }

        try
        {
            await _householdRepository.AddMemberAsync(request.HouseholdId, request.PersonId);
        }
        catch (KeyNotFoundException ex)
        {
            throw new NotFoundException(ex.Message);
        }
    }

    public async Task RemoveMemberAsync(int householdId, int personId)
    {
        if (householdId <= 0)
        {
            throw new ValidationException("Invalid household ID.");
        }

        if (personId <= 0)
        {
            throw new ValidationException("Invalid person ID.");
        }

        try
        {
            await _householdRepository.RemoveMemberAsync(householdId, personId);
        }
        catch (KeyNotFoundException ex)
        {
            throw new NotFoundException(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    public async Task<HouseholdViewModel> UpdateSettingsAsync(UpdateHouseholdSettingsRequest request)
    {
        var household = await _householdRepository.GetByIdAsync(request.Id);
        if (household == null)
        {
            throw new NotFoundException($"Household with ID {request.Id} not found.");
        }

        household.IsArchived = request.IsArchived;
        if (request.IsArchived && household.ArchivedDateTime == null)
        {
            household.ArchivedDateTime = DateTime.UtcNow;
        }
        else if (!request.IsArchived)
        {
            household.ArchivedDateTime = null;
        }

        var updatedHousehold = await _householdRepository.UpdateAsync(household);

        // Reload to get navigation properties
        var reloadedHousehold = await _householdRepository.GetByIdAsync(updatedHousehold.Id);
        var memberCount = await _householdRepository.GetMemberCountAsync(updatedHousehold.Id);
        return _householdMapper.MapToViewModel(reloadedHousehold!, memberCount);
    }

    // New methods for Phase 3.1
    public async Task RemoveMemberByUserIdAsync(int householdId, string userId)
    {
        if (householdId <= 0)
        {
            throw new ValidationException("Invalid household ID.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ValidationException("Invalid user ID.");
        }

        // Convert userId to personId
        var personId = await _householdRepository.GetPersonIdFromUserIdAsync(userId);
        if (personId == null)
        {
            throw new NotFoundException($"User with ID {userId} not found or not linked to a person.");
        }

        // Use existing RemoveMemberAsync method
        await RemoveMemberAsync(householdId, personId.Value);
    }

    public async Task UpdateMemberRoleAsync(int householdId, string userId, string role)
    {
        if (householdId <= 0)
        {
            throw new ValidationException("Invalid household ID.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ValidationException("Invalid user ID.");
        }

        if (string.IsNullOrWhiteSpace(role) || (role != "ADMIN" && role != "EDITOR"))
        {
            throw new ValidationException("Role must be either 'ADMIN' or 'EDITOR'.");
        }

        // Verify household exists
        var householdExists = await _householdRepository.ExistsAsync(householdId);
        if (!householdExists)
        {
            throw new NotFoundException($"Household with ID {householdId} not found.");
        }

        // Convert userId to personId
        var personId = await _householdRepository.GetPersonIdFromUserIdAsync(userId);
        if (personId == null)
        {
            throw new NotFoundException($"User with ID {userId} not found or not linked to a person.");
        }

        // Verify person is a member of the household
        var members = await _householdRepository.GetMembersAsync(householdId);
        if (!members.Any(m => m.Id == personId.Value))
        {
            throw new ValidationException($"User is not a member of household {householdId}.");
        }

        // Update the role
        await _householdRepository.UpdateMemberRoleAsync(householdId, personId.Value, role);
    }

    public async Task ResendInviteAsync(int householdId, string userId)
    {
        if (householdId <= 0)
        {
            throw new ValidationException("Invalid household ID.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ValidationException("Invalid user ID.");
        }

        // Verify household exists
        var householdExists = await _householdRepository.ExistsAsync(householdId);
        if (!householdExists)
        {
            throw new NotFoundException($"Household with ID {householdId} not found.");
        }

        // Convert userId to personId
        var personId = await _householdRepository.GetPersonIdFromUserIdAsync(userId);
        if (personId == null)
        {
            throw new NotFoundException($"User with ID {userId} not found or not linked to a person.");
        }

        // Verify person is a member of the household
        var members = await _householdRepository.GetMembersAsync(householdId);
        if (!members.Any(m => m.Id == personId.Value))
        {
            throw new ValidationException($"User is not a member of household {householdId}.");
        }

        // TODO: Implement actual invite sending logic (e.g., send email, create notification)
        // For now, this is a placeholder that validates inputs and membership
        // In a real implementation, this would:
        // 1. Generate a new invite token/link
        // 2. Send an email or notification to the user
        // 3. Update any invite tracking in the database
    }

    // Phase 3.3: Delete Impact Calculation
    public async Task<HouseholdDeleteImpact> GetDeleteImpactAsync(int householdId)
    {
        if (householdId <= 0)
        {
            throw new ValidationException("Invalid household ID.");
        }

        // Verify household exists
        var householdExists = await _householdRepository.ExistsAsync(householdId);
        if (!householdExists)
        {
            throw new NotFoundException($"Household with ID {householdId} not found.");
        }

        return new HouseholdDeleteImpact
        {
            MemberCount = await _householdRepository.GetMemberCountAsync(householdId),
            PhotoCount = await _householdRepository.GetPhotoCountAsync(householdId),
            DocumentCount = await _householdRepository.GetDocumentCountAsync(householdId),
            RelationshipCount = await _householdRepository.GetRelationshipCountAsync(householdId),
            EventCount = await _householdRepository.GetEventCountAsync(householdId)
        };
    }
}
