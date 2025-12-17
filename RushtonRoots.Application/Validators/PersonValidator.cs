using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Validators;

/// <summary>
/// Validator implementation for Person requests.
/// </summary>
public class PersonValidator : IPersonValidator
{
    private readonly IHouseholdRepository _householdRepository;

    public PersonValidator(IHouseholdRepository householdRepository)
    {
        _householdRepository = householdRepository;
    }

    public async Task<ValidationResult> ValidateCreateAsync(CreatePersonRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            result.IsValid = false;
            result.Errors.Add("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            result.IsValid = false;
            result.Errors.Add("Last name is required.");
        }

        if (request.HouseholdId.HasValue && request.HouseholdId.Value > 0)
        {
            if (!await _householdRepository.ExistsAsync(request.HouseholdId.Value))
            {
                result.IsValid = false;
                result.Errors.Add("Household does not exist.");
            }
        }

        if (request.DateOfBirth.HasValue && request.DateOfDeath.HasValue)
        {
            if (request.DateOfDeath < request.DateOfBirth)
            {
                result.IsValid = false;
                result.Errors.Add("Date of death cannot be before date of birth.");
            }
        }

        if (request.IsDeceased && !request.DateOfDeath.HasValue)
        {
            result.IsValid = false;
            result.Errors.Add("Date of death is required for deceased persons.");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(UpdatePersonRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        if (request.Id <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Valid person ID is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            result.IsValid = false;
            result.Errors.Add("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            result.IsValid = false;
            result.Errors.Add("Last name is required.");
        }

        if (request.HouseholdId.HasValue && request.HouseholdId.Value > 0)
        {
            if (!await _householdRepository.ExistsAsync(request.HouseholdId.Value))
            {
                result.IsValid = false;
                result.Errors.Add("Household does not exist.");
            }
        }

        if (request.DateOfBirth.HasValue && request.DateOfDeath.HasValue)
        {
            if (request.DateOfDeath < request.DateOfBirth)
            {
                result.IsValid = false;
                result.Errors.Add("Date of death cannot be before date of birth.");
            }
        }

        if (request.IsDeceased && !request.DateOfDeath.HasValue)
        {
            result.IsValid = false;
            result.Errors.Add("Date of death is required for deceased persons.");
        }

        return result;
    }
}
