namespace RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

/// <summary>
/// Базовый класс для ошибок приложения.
/// </summary>
/// <param name="error">Сообщение об ошибке.</param>
public abstract class ErrorException(string error) : Exception(error)
{
	/// <summary>
	/// Сообщение об ошибке.
	/// </summary>
	public string Error { get; } = error;

	/// <summary>
	/// Создает исключение конфликта.
	/// </summary>
	/// <param name="message">Сообщение об ошибке.</param>
	/// <returns>Исключение конфликта.</returns>
	public static ConflictException Conflict(string message)
	{
		return new(message);
	}

	/// <summary>
	/// Создает исключение не найдено.
	/// </summary>
	/// <param name="message">Сообщение об ошибке.</param>
	/// <returns>Исключение не найдено.</returns>
	public static NotFoundException NotFound(string message)
	{
		return new(message);
	}

	/// <summary>
	/// Создает исключение валидации.
	/// </summary>
	/// <param name="message">Сообщение об ошибке.</param>
	/// <returns>Исключение валидации.</returns>
	public static ValidationException Validation(string message)
	{
		return new(message);
	}

	/// <summary>
	/// Создает исключение валидации для незаполненного значения.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <returns>Исключение валидации для незаполненного значения.</returns>
	public static ValidationException ValueNotSet(string valueName)
	{
		string message = $"{valueName}. Значение не заполнено.";
		return Validation(message);
	}

	/// <summary>
	/// Создает исключение валидации для превышающего длину значения.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <param name="length">Максимально допустимая длина.</param>
	/// <returns>Исключение валидации для превышающего длину значения.</returns>
	public static ValidationException ValueExcess(string valueName, int length)
	{
		string message = $"{valueName}. Превышает длину {length}.";
		return Validation(message);
	}

	/// <summary>
	/// Создает исключение валидации для некорректного формата значения.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <returns>Исключение валидации для некорректного формата значения.</returns>
	public static ValidationException ValueInvalidFormat(string valueName)
	{
		string message = $"{valueName}. Некорректный формат.";
		return Validation(message);
	}

	/// <summary>
	/// Создает исключение валидации для невалидного значения.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <returns>Исключение валидации для невалидного значения.</returns>
	public static ValidationException ValueInvalid(string valueName)
	{
		string message = $"{valueName}. Невалидное значение.";
		return Validation(message);
	}

	/// <summary>
	/// Создает исключение валидации для невалидного значения с указанием самого значения.
	/// </summary>
	/// <param name="valueName">Имя значения.</param>
	/// <param name="value">Значение.</param>
	/// <returns>Исключение валидации для невалидного значения с указанием самого значения.</returns>
	public static ValidationException ValueInvalid(string valueName, string value)
	{
		string message = $"{valueName}. Невалидное значение {value}.";
		return Validation(message);
	}

	/// <summary>
	/// Создает внутреннее исключение.
	/// </summary>
	/// <param name="message">Сообщение об ошибке.</param>
	/// <returns>Внутреннее исключение.</returns>
	public static InternalException Internal(string message)
	{
		return new(message);
	}

	/// <summary>
	/// Исключение конфликта.
	/// </summary>
	/// <param name="error">Сообщение об ошибке.</param>
	public class ConflictException(string error) : ErrorException(error);

	/// <summary>
	/// Исключение не найдено.
	/// </summary>
	/// <param name="error">Сообщение об ошибке.</param>
	public class NotFoundException(string error) : ErrorException(error);

	/// <summary>
	/// Исключение валидации.
	/// </summary>
	/// <param name="error">Сообщение об ошибке.</param>
	public class ValidationException(string error) : ErrorException(error);

	/// <summary>
	/// Внутреннее исключение.
	/// </summary>
	/// <param name="error">Сообщение об ошибке.</param>
	public class InternalException(string error) : ErrorException(error);
}
