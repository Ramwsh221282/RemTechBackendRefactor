using FluentValidation;
using FluentValidation.Results;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

public sealed class ValidatingHandler<TCommand, TResult>(
	IEnumerable<IValidator<TCommand>> validators,
	ICommandHandler<TCommand, TResult> handler
) : IValidatingCommandHandler<TCommand, TResult>
	where TCommand : ICommand
{
	private IEnumerable<IValidator<TCommand>> Validators { get; } = validators;
	private ICommandHandler<TCommand, TResult> Handler { get; } = handler;

	public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
	{
		foreach (IValidator<TCommand> validator in Validators)
		{
			ValidationResult result = await validator.ValidateAsync(command, cancellation: ct);
			if (ValidatingHandler<TCommand, TResult>.HasErrors(result, out List<string> errors))
				return Error.Validation(string.Join(", ", errors));
		}

		return await Handler.Execute(command, ct: ct);
	}

	private static bool HasErrors(ValidationResult result, out List<string> errors)
	{
		errors = [];
		if (!result.IsValid)
		{
			foreach (ValidationFailure failure in result.Errors)
				errors.Add(failure.ErrorMessage);
			return true;
		}
		return false;
	}
}
