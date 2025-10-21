using FluentValidation.Results;
using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Validation;

public static class ValidationExtensions
{
    public static Error ValidationError(this ValidationResult validation)
    {
        if (validation.IsValid)
            throw new ApplicationException("Validation result was valid.");

        IEnumerable<string> errors = validation.Errors.Select(er => er.ErrorMessage);
        string message = string.Join(" ,", errors);
        return Error.Validation(message);
    }
}
