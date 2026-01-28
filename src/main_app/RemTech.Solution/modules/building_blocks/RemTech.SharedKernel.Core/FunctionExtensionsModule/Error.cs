namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

/// <summary>
/// Базовый класс ошибки.
/// </summary>
/// <param name="message">Сообщение об ошибке.</param>
public abstract class Error(string message)
{
	/// <summary>
	/// Сообщение об ошибке.
	/// </summary>
	public string Message => message;

	public static implicit operator Result(Error error) => Result.Failure(error);

	public static implicit operator string(Error error) => error.Message;

	/// <summary>
	/// Ошибка валидации.
	/// </summary>
	/// <param name="message">Сообщение об ошибке валидации.</param>
	/// <returns>Объект ошибки валидации.</returns>
	public static Error Validation(string message) => new ValidationError(message);

	/// <summary>
	/// Ошибка валидации из коллекции сообщений.
	/// </summary>
	/// <param name="errors">Коллекция сообщений об ошибках валидации.</param>
	/// <returns>Объект ошибки валидации.</returns>
	public static Error Validation(IEnumerable<string> errors)
	{
		string singleMessage = string.Join(Environment.NewLine, errors);
		return Validation(singleMessage);
	}

	/// <summary>
	/// Ошибка значения не установлено.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <returns>Объект ошибки валидации.</returns>
	public static Error NotSet(string valueName) => new ValidationError($"{valueName} значение не установлено.");

	/// <summary>
	/// Ошибка превышения максимальной длины значения.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <param name="maxLength">Максимальная длина значения.</param>
	/// <returns>Объект ошибки валидации.</returns>
	public static Error GreaterThan(string valueName, int maxLength) =>
		new ValidationError($"{valueName} значение превышает длину {maxLength} символов.");

	/// <summary>
	/// Ошибка некорректного формата значения.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <returns>Объект ошибки валидации.</returns>
	public static Error InvalidFormat(string valueName) => new ValidationError($"{valueName} некорректный формат.");

	/// <summary>
	/// Ошибка валидации из результата.
	/// </summary>
	/// <param name="result">Результат с ошибкой валидации.</param>
	/// <returns>Объект ошибки валидации.</returns>
	public static Error Validation(Result result) => new ValidationError(result.Error.Message);

	/// <summary>
	/// Ошибка приложения.
	/// </summary>
	/// <param name="message">Сообщение об ошибке приложения.</param>
	/// <returns>Объект ошибки приложения.</returns>
	public static Error Application(string message) => new ApplicationError(message);

	/// <summary>
	/// Ошибка не найдена.
	/// </summary>
	/// <param name="message">Сообщение об ошибке не найдено.</param>
	/// <returns>Объект ошибки не найдено.</returns>
	public static Error NotFound(string message) => new NotFoundError(message);

	/// <summary>
	/// Ошибка неавторизованности.
	/// </summary>
	/// <param name="message">Сообщение об ошибке неавторизованности.</param>
	/// <returns>Объект ошибки неавторизованности.</returns>
	public static Error Unauthorized(string message) => new UnauthorizedError(message);

	/// <summary>
	/// Ошибка запрещено.
	/// </summary>
	/// <param name="message">Сообщение об ошибке запрещено.</param>
	/// <returns>Объект ошибки запрещено.</returns>
	public static Error Forbidden(string message) => new ForbiddenError(message);

	/// <summary>
	/// Ошибка конфликта.
	/// </summary>
	/// <param name="message">Сообщение об ошибке конфликта.</param>
	/// <returns>Объект ошибки конфликта.</returns>
	public static Error Conflict(string message) => new ConflictError(message);

	/// <summary>
	/// Нет ошибки.
	/// </summary>
	/// <returns>Объект отсутствия ошибки.</returns>
	public static Error NoError() => new NoneError();

	/// <summary>
	/// Ошибка запрещено.
	/// </summary>
	public sealed class ForbiddenError : Error
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ForbiddenError"/>.
		/// </summary>
		/// <param name="message">Сообщение об ошибке запрещено.</param>
		internal ForbiddenError(string message)
			: base(message) { }
	}

	/// <summary>
	/// Ошибка неавторизованности.
	/// </summary>
	public sealed class UnauthorizedError : Error
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="UnauthorizedError"/>.
		/// </summary>
		/// <param name="message">Сообщение об ошибке неавторизованности.</param>
		internal UnauthorizedError(string message)
			: base(message) { }
	}

	/// <summary>
	/// Ошибка валидации.
	/// </summary>
	public sealed class ValidationError : Error
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ValidationError"/>.
		/// </summary>
		/// <param name="message">Сообщение об ошибке валидации.</param>
		internal ValidationError(string message)
			: base(message) { }
	}

	/// <summary>
	/// Ошибка приложения.
	/// </summary>
	public sealed class ApplicationError : Error
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ApplicationError"/>.
		/// </summary>
		/// <param name="message">Сообщение об ошибке приложения.</param>
		internal ApplicationError(string message)
			: base(message) { }
	}

	/// <summary>
	/// Ошибка не найдена.
	/// </summary>
	public sealed class NotFoundError : Error
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NotFoundError"/>.
		/// </summary>
		/// <param name="message">Сообщение об ошибке не найдено.</param>
		internal NotFoundError(string message)
			: base(message) { }
	}

	/// <summary>
	/// Ошибка конфликта.
	/// </summary>
	public sealed class ConflictError : Error
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ConflictError"/>.
		/// </summary>
		/// <param name="message">Сообщение об ошибке конфликта.</param>
		internal ConflictError(string message)
			: base(message) { }
	}

	/// <summary>
	/// Нет ошибки.
	/// </summary>
	public sealed class NoneError : Error
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NoneError"/>.
		/// </summary>
		internal NoneError()
			: base(string.Empty) { }
	}
}
