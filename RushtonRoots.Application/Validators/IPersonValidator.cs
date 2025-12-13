using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Validators;

/// <summary>
/// Validator interface for Person requests.
/// </summary>
public interface IPersonValidator
{
    Task<ValidationResult> ValidateCreateAsync(CreatePersonRequest request);
    Task<ValidationResult> ValidateUpdateAsync(UpdatePersonRequest request);
}

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
