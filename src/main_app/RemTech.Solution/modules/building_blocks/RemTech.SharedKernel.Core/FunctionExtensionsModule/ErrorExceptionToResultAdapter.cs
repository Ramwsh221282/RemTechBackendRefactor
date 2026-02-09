using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

/// <summary>
/// Адаптер для преобразования ErrorException в Result.
/// </summary>
/// <param name="ex">Исключение типа ErrorException для адаптации.</param>
public sealed class ErrorExceptionToResultAdapter(ErrorException ex)
{
	/// <summary>
	/// Адаптирует ErrorException в Result.
	/// </summary>
	/// <returns>Преобразованный результат типа Result.</returns>
	/// <exception cref="ApplicationException">Thrown when the error type cannot be resolved.</exception>
	public Result Adapt()
	{
		return ex switch
		{
			ErrorException.ValidationException => Result.Failure(Error.Validation(ex.Error)),
			ErrorException.ConflictException => Result.Failure(Error.Conflict(ex.Error)),
			ErrorException.InternalException => Result.Failure(Error.Application(ex.Error)),
			ErrorException.NotFoundException => Result.Failure(Error.NotFound(ex.Error)),
			_ => throw new ApplicationException($"Unable to resolve error type: {ex.GetType().Name}"),
		};
	}
}
