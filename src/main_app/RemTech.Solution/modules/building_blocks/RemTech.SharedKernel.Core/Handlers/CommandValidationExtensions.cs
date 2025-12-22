using FluentValidation;
using FluentValidation.Results;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

public static class CommandValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, TProperty> MustBeValid<T, TProperty>(
        this IRuleBuilderInitial<T, TProperty> builder,
        Func<TProperty, Result> validation
        )
    {
        return builder.Custom(((validate, context) =>
                {
                    Result result = validation(validate);
                    if (result.IsFailure)
                    {
                        string error = result.Error.Message;
                        context.AddFailure(new ValidationFailure() { ErrorMessage = error });
                    }
                }));
    }
}