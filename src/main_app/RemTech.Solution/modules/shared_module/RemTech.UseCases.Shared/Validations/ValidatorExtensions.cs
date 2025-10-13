using FluentValidation;
using FluentValidation.Results;
using RemTech.Result.Pattern;

namespace RemTech.UseCases.Shared.Validations;

public static class ValidatorExtensions
{
    public static bool NotValid(this ValidationResult validationResult) =>
        !validationResult.IsValid;

    public static Result.Pattern.Result Failure(this ValidationResult validationResult)
    {
        List<Error> errors = [];
        foreach (ValidationFailure failure in validationResult.Errors)
            errors.Add(failure.AsError());

        Error[] distinct = errors.DistinctBy(err => err.Code).ToArray();
        if (distinct.Length != 1)
            throw new ApplicationException("Validation error codes all should be same.");

        IEnumerable<string> errorStrings = errors.Select(er => er.ErrorText);
        string resultMessage = string.Join(", ", errorStrings);
        return Result.Pattern.Result.Failure(new Error(resultMessage, distinct.First().Code));
    }

    public static Result<T> Failure<T>(this ValidationResult validationResult)
    {
        Result.Pattern.Result result = validationResult.Failure();
        return Result<T>.Failure(result.Error);
    }

    public static Error AsError(this ValidationFailure failure)
    {
        if (!Enum.TryParse(failure.ErrorCode, out ErrorCodes errorCode))
            throw new ApplicationException($"Unsupported error code: {errorCode}");
        Error error = new Error(failure.ErrorMessage, errorCode);
        return error;
    }

    public static void MustBeValid<TProperty, TValidatable>(
        this IRuleBuilderInitial<TProperty, TValidatable> initial,
        Func<TValidatable, Result.Pattern.Result> statusFactory
    ) => initial.Custom((validatable, context) => validatable.ManageStatus(context, statusFactory));

    public static void AllMustBeValid<TProperty, TValidatable>(
        this IRuleBuilderInitial<TProperty, IEnumerable<TValidatable>> initial,
        Func<TValidatable, Result.Pattern.Result> statusFactory
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
        Func<TValidatable, Result.Pattern.Result> statusFactory
    )
    {
        Result.Pattern.Result result = statusFactory(validatable);
        if (result.IsFailure)
            context.AddFailure(result.FailureFromStatus());
    }

    private static ValidationFailure FailureFromStatus(this Result.Pattern.Result result)
    {
        if (result.IsSuccess)
            throw new ApplicationException("Cannot create failure from success status.");

        string message = result.Error.ErrorText;
        string code = result.Error.Code.ToString();
        return new ValidationFailure()
        {
            ErrorCode = code,
            ErrorMessage = message,
            Severity = Severity.Error,
        };
    }
}
