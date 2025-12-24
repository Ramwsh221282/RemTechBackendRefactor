using FluentValidation;
using FluentValidation.Results;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Scrutor;

namespace RemTech.SharedKernel.Core.Handlers;

public sealed class ValidatingHandler<TCommand, TResult>(
    IEnumerable<IValidator<TCommand>> validators, 
    ICommandHandler<TCommand, TResult> handler)
    : IValidatingCommandHandler<TCommand, TResult> where TCommand : ICommand
{
    public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
    {
        foreach (IValidator<TCommand> validator in validators)
        {
            ValidationResult result = await validator.ValidateAsync(command, cancellation: ct);
            if (HasErrors(result, out List<string> errors))
                return Error.Validation(string.Join(", ", errors));
        }
        
        return await handler.Execute(command, ct: ct);
    }
    
    private bool HasErrors(ValidationResult result, out List<string> errors)
    {
        errors = [];
        if (result.IsValid == false)
        {
            foreach (ValidationFailure failure in result.Errors)
                errors.Add(failure.ErrorMessage);
            return true;
        }
        return false;
    }
}