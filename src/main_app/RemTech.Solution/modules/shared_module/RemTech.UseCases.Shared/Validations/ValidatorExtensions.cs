using FluentValidation;
using FluentValidation.Results;
using RemTech.Core.Shared.Result;

namespace RemTech.UseCases.Shared.Validations;

public static class ValidatorExtensions
{
    public static bool NotValid(this ValidationResult validationResult) =>
        !validationResult.IsValid;

    public static Status BadStatus(this ValidationResult validationResult)
    {
        List<Error> errors = [];
        foreach (ValidationFailure failure in validationResult.Errors)
            errors.Add(failure.AsError());

        Error[] distinct = errors.DistinctBy(err => err.Code).ToArray();
        if (distinct.Length != 1)
            throw new ApplicationException("Validation error codes all should be same.");

        IEnumerable<string> errorStrings = errors.Select(er => er.ErrorText);
        string resultMessage = string.Join(", ", errorStrings);
        return Status.Failure(new Error(resultMessage, distinct.First().Code));
    }

    public static Status<T> BadStatus<T>(this ValidationResult validationResult)
    {
        Status status = validationResult.BadStatus();
        return Status<T>.Failure(status.Error);
    }

    public static Error AsError(this ValidationFailure failure)
    {
        if (!Enum.TryParse(failure.ErrorCode, out ErrorCodes errorCode))
            throw new ApplicationException($"Unsupported error code: {errorCode}");
        Error error = new Error(failure.ErrorMessage, errorCode);
        return error;
    }

    public static IRuleBuilderOptionsConditions<TProperty, TValidatable> MustBeValid<
        TProperty,
        TValidatable
    >(
        this IRuleBuilderInitial<TProperty, TValidatable> initial,
        Func<TValidatable, Status> statusFactory
    ) => initial.Custom((validatable, context) => validatable.ManageStatus(context, statusFactory));

    public static IRuleBuilderOptionsConditions<
        TProperty,
        IEnumerable<TValidatable>
    > AllMustBeValid<TProperty, TValidatable>(
        this IRuleBuilderInitial<TProperty, IEnumerable<TValidatable>> initial,
        Func<TValidatable, Status> statusFactory
    ) =>
        initial.Custom(
            (enumerable, context) =>
            {
                foreach (TValidatable entry in enumerable)
                    entry.ManageStatus(context, statusFactory);
            }
        );

    private static void ManageStatus<TProperty, TValidatable>(
        this TValidatable validatable,
        ValidationContext<TProperty> context,
        Func<TValidatable, Status> statusFactory
    )
    {
        Status status = statusFactory(validatable);
        if (status.IsFailure)
            context.AddFailure(status.FailureFromStatus());
    }

    private static ValidationFailure FailureFromStatus(this Status status)
    {
        if (status.IsSuccess)
            throw new ApplicationException("Cannot create failure from success status.");

        string message = status.Error.ErrorText;
        string code = status.Error.Code.ToString();
        return new ValidationFailure()
        {
            ErrorCode = code,
            ErrorMessage = message,
            Severity = Severity.Error,
        };
    }
}
