using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Validators;

/// <summary>
/// Validator interface for Household requests.
/// </summary>
public interface IHouseholdValidator
{
    Task<ValidationResult> ValidateCreateAsync(CreateHouseholdRequest request);
    Task<ValidationResult> ValidateUpdateAsync(UpdateHouseholdRequest request);
}
