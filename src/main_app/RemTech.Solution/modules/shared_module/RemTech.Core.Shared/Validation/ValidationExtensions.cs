using FluentValidation;
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

    public static IRuleBuilderOptionsConditions<TProperty, TToValidate> MustBeValid<
        TProperty,
        TToValidate
    >(this IRuleBuilderInitial<TProperty, TToValidate> builder, Func<TToValidate, Status> statusFn)
    {
        return builder.Custom(
            (
                (validate, context) =>
                {
                    Status status = statusFn(validate);
                    if (status.IsFailure)
                        context.AddFailure(status.FailureFromStatus());
                }
            )
        );
    }

    public static IRuleBuilderOptionsConditions<TProperty, TToValidate> MustBeValid<
        TProperty,
        TToValidate,
        TStatus
    >(
        this IRuleBuilderInitial<TProperty, TToValidate> builder,
        Func<TToValidate, Status<TStatus>> statusFn
    )
    {
        return builder.Custom(
            (
                (validate, context) =>
                {
                    Status<TStatus> status = statusFn(validate);
                    Status notGeneric = new Status(status.Error);
                    if (notGeneric.IsFailure)
                        context.AddFailure(notGeneric.FailureFromStatus());
                }
            )
        );
    }

    private static ValidationFailure FailureFromStatus(this Status status) =>
        new() { ErrorCode = nameof(ErrorCodes.Validation), ErrorMessage = status.Error.ErrorText };
}
