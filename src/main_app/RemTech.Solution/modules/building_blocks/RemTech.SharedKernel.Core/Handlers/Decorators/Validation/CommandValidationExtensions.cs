using FluentValidation;
using FluentValidation.Results;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

public static class CommandValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, TProperty> MustBeValid<T, TProperty>(
        this IRuleBuilderInitial<T, TProperty> builder,
        Func<TProperty, Result> validation
    )
    {
        return builder.Custom(
            (validate, context) =>
            {
                Result result = validation(validate);
                if (result.IsFailure)
                {
                    string error = result.Error.Message;
                    context.AddFailure(new ValidationFailure() { ErrorMessage = error });
                }
            }
        );
    }

    public static IRuleBuilderOptionsConditions<T, IEnumerable<TProperty>> EachMustFollow<
        T,
        TProperty
    >(
        this IRuleBuilderInitial<T, IEnumerable<TProperty>> builder,
        Func<TProperty, Result>[] validations
    )
    {
        return builder.Custom(
            (validate, context) =>
            {
                var failures = new List<ValidationFailure>();
                foreach (TProperty item in validate)
                {
                    foreach (Func<TProperty, Result> validation in validations)
                    {
                        Result result = validation(item);
                        if (result.IsFailure)
                        {
                            string error = result.Error.Message;
                            failures.Add(new ValidationFailure() { ErrorMessage = error });
                        }
                    }
                }
                if (failures.Count > 0)
                {
                    foreach (ValidationFailure failure in failures)
                        context.AddFailure(failure);
                }
            }
        );
    }

    public static IRuleBuilderOptionsConditions<T, IEnumerable<TProperty>> AllMustBeValid<
        T,
        TProperty
    >(
        this IRuleBuilderInitial<T, IEnumerable<TProperty>> builder,
        Func<TProperty, Result> validation
    )
    {
        return builder.Custom(
            (validate, context) =>
            {
                var failures = new List<ValidationFailure>();
                foreach (TProperty item in validate)
                {
                    Result result = validation(item);
                    if (result.IsFailure)
                    {
                        string error = result.Error.Message;
                        failures.Add(new ValidationFailure() { ErrorMessage = error });
                    }
                }
                if (failures.Count > 0)
                {
                    foreach (ValidationFailure failure in failures)
                        context.AddFailure(failure);
                }
            }
        );
    }
}
