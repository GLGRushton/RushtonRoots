using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Validators;

public interface IWikiPageValidator
{
    Task<ValidationResult> ValidateCreateAsync(CreateWikiPageRequest request);
    Task<ValidationResult> ValidateUpdateAsync(UpdateWikiPageRequest request);
}

public class WikiPageValidator : IWikiPageValidator
{
    public Task<ValidationResult> ValidateCreateAsync(CreateWikiPageRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            result.IsValid = false;
            result.Errors.Add("Title is required");
        }
        else if (request.Title.Length > 200)
        {
            result.IsValid = false;
            result.Errors.Add("Title cannot exceed 200 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            result.IsValid = false;
            result.Errors.Add("Content is required");
        }

        if (!string.IsNullOrWhiteSpace(request.Summary) && request.Summary.Length > 500)
        {
            result.IsValid = false;
            result.Errors.Add("Summary cannot exceed 500 characters");
        }

        if (!string.IsNullOrWhiteSpace(request.ChangeDescription) && request.ChangeDescription.Length > 1000)
        {
            result.IsValid = false;
            result.Errors.Add("Change description cannot exceed 1000 characters");
        }

        return Task.FromResult(result);
    }

    public Task<ValidationResult> ValidateUpdateAsync(UpdateWikiPageRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            result.IsValid = false;
            result.Errors.Add("Title is required");
        }
        else if (request.Title.Length > 200)
        {
            result.IsValid = false;
            result.Errors.Add("Title cannot exceed 200 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            result.IsValid = false;
            result.Errors.Add("Content is required");
        }

        if (!string.IsNullOrWhiteSpace(request.Summary) && request.Summary.Length > 500)
        {
            result.IsValid = false;
            result.Errors.Add("Summary cannot exceed 500 characters");
        }

        if (!string.IsNullOrWhiteSpace(request.ChangeDescription) && request.ChangeDescription.Length > 1000)
        {
            result.IsValid = false;
            result.Errors.Add("Change description cannot exceed 1000 characters");
        }

        return Task.FromResult(result);
    }
}
