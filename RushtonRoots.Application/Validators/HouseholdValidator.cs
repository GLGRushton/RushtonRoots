using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Validators;

/// <summary>
/// Validator implementation for Household requests.
/// </summary>
public class HouseholdValidator : IHouseholdValidator
{
    private readonly IPersonRepository _personRepository;

    public HouseholdValidator(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<ValidationResult> ValidateCreateAsync(CreateHouseholdRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.HouseholdName))
        {
            result.IsValid = false;
            result.Errors.Add("Household name is required.");
        }

        if (request.AnchorPersonId <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Valid anchor person ID is required.");
        }
        else if (!await _personRepository.ExistsAsync(request.AnchorPersonId))
        {
            result.IsValid = false;
            result.Errors.Add("Anchor person does not exist.");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(UpdateHouseholdRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        if (request.Id <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Valid household ID is required.");
        }

        if (string.IsNullOrWhiteSpace(request.HouseholdName))
        {
            result.IsValid = false;
            result.Errors.Add("Household name is required.");
        }

        if (request.AnchorPersonId <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Valid anchor person ID is required.");
        }
        else if (!await _personRepository.ExistsAsync(request.AnchorPersonId))
        {
            result.IsValid = false;
            result.Errors.Add("Anchor person does not exist.");
        }

        return result;
    }
}
