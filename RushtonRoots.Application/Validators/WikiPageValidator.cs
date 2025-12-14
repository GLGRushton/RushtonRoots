using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Validators;

public interface IWikiPageValidator
{
    ValidationResult ValidateCreate(CreateWikiPageRequest request);
    ValidationResult ValidateUpdate(UpdateWikiPageRequest request);
}

public class WikiPageValidator : IWikiPageValidator
{
    public ValidationResult ValidateCreate(CreateWikiPageRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            result.AddError("Title", "Title is required");
        }
        else if (request.Title.Length > 200)
        {
            result.AddError("Title", "Title cannot exceed 200 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            result.AddError("Content", "Content is required");
        }

        if (!string.IsNullOrWhiteSpace(request.Summary) && request.Summary.Length > 500)
        {
            result.AddError("Summary", "Summary cannot exceed 500 characters");
        }

        if (!string.IsNullOrWhiteSpace(request.ChangeDescription) && request.ChangeDescription.Length > 1000)
        {
            result.AddError("ChangeDescription", "Change description cannot exceed 1000 characters");
        }

        return result;
    }

    public ValidationResult ValidateUpdate(UpdateWikiPageRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            result.AddError("Title", "Title is required");
        }
        else if (request.Title.Length > 200)
        {
            result.AddError("Title", "Title cannot exceed 200 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            result.AddError("Content", "Content is required");
        }

        if (!string.IsNullOrWhiteSpace(request.Summary) && request.Summary.Length > 500)
        {
            result.AddError("Summary", "Summary cannot exceed 500 characters");
        }

        if (!string.IsNullOrWhiteSpace(request.ChangeDescription) && request.ChangeDescription.Length > 1000)
        {
            result.AddError("ChangeDescription", "Change description cannot exceed 1000 characters");
        }

        return result;
    }
}

public class ValidationResult
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool IsValid => !_errors.Any();

    public Dictionary<string, List<string>> Errors => _errors;

    public void AddError(string field, string message)
    {
        if (!_errors.ContainsKey(field))
        {
            _errors[field] = new List<string>();
        }
        _errors[field].Add(message);
    }

    public List<string> GetErrors(string field)
    {
        return _errors.ContainsKey(field) ? _errors[field] : new List<string>();
    }
}
