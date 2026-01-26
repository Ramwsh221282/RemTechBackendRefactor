namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public abstract class Error(string message)
{
	public string Message => message;

	public static implicit operator Result(Error error)
	{
		return Result.Failure(error);
	}

	public static implicit operator string(Error error)
	{
		return error.Message;
	}

	public static Error Validation(string message) => new ValidationError(message);

	public static Error Validation(IEnumerable<string> errors)
	{
		string singleMessage = string.Join(Environment.NewLine, errors);
		return Validation(singleMessage);
	}

	public static Error NotSet(string valueName) => new ValidationError($"{valueName} значение не установлено.");

	public static Error GreaterThan(string valueName, int maxLength) =>
		new ValidationError($"{valueName} значение превышает длину {maxLength} символов.");

	public static Error InvalidFormat(string valueName) => new ValidationError($"{valueName} некорректный формат.");

	public static Error Validation(Result result) => new ValidationError(result.Error.Message);

	public static Error Application(string message) => new ApplicationError(message);

	public static Error NotFound(string message) => new NotFoundError(message);

	public static Error Unauthorized(string message) => new UnauthorizedError(message);

	public static Error Forbidden(string message) => new ForbiddenError(message);

	public static Error Conflict(string message) => new ConflictError(message);

	public static Error NoError() => new NoneError();

	public sealed class ForbiddenError : Error
	{
		internal ForbiddenError(string message)
			: base(message) { }
	}

	public sealed class UnauthorizedError : Error
	{
		internal UnauthorizedError(string message)
			: base(message) { }
	}

	public sealed class ValidationError : Error
	{
		internal ValidationError(string message)
			: base(message) { }
	}

	public sealed class ApplicationError : Error
	{
		internal ApplicationError(string message)
			: base(message) { }
	}

	public sealed class NotFoundError : Error
	{
		internal NotFoundError(string message)
			: base(message) { }
	}

	public sealed class ConflictError : Error
	{
		internal ConflictError(string message)
			: base(message) { }
	}

	public sealed class NoneError : Error
	{
		internal NoneError()
			: base(string.Empty) { }
	}
}
