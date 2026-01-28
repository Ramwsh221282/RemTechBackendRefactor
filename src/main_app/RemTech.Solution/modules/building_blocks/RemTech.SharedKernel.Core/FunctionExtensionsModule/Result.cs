namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

/// <summary>
/// Результат операции, представляющий успех или ошибку.
/// </summary>
public class Result
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Result"/> с ошибкой.
	/// </summary>
	/// <param name="error">Ошибка, связанная с результатом.</param>
	internal Result(Error error)
	{
		Error = error;
		IsSuccess = false;
	}

	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Result"/> для успешного результата.
	/// </summary>
	internal Result()
	{
		IsSuccess = true;
	}

	/// <summary>
	/// Ошибка, связанная с результатом.
	/// </summary>
	public Error Error { get; } = Error.NoError();

	/// <summary>
	/// Указывает, является ли результат успешным.
	/// </summary>
	public bool IsSuccess { get; }

	/// <summary>
	/// Указывает, является ли результат неуспешным.
	/// </summary>
	public bool IsFailure => !IsSuccess;

	/// <summary>
	/// Импlicitное преобразование ошибки в результат.
	/// </summary>
	/// <param name="error">Ошибка для преобразования в результат.</param>
	public static implicit operator Result(Error error) => Failure(error);

	/// <summary>
	/// Создает успешный результат.
	/// </summary>
	/// <returns>Успешный результат.</returns>
	public static Result Success() => new();

	/// <summary>
	/// Создает успешный результат с заданным значением.
	/// </summary>
	/// <typeparam name="T">Тип значения результата.</typeparam>
	/// <param name="value">Значение успешного результата.</param>
	/// <returns>Успешный результат с заданным значением.</returns>
	public static Result<T> Success<T>(T value) => new(value);

	/// <summary>
	/// Создает неуспешный результат с заданной ошибкой.
	/// </summary>
	/// <typeparam name="T">Тип значения результата.</typeparam>
	/// <param name="error">Ошибка, связанная с результатом.</param>
	/// <returns>Неуспешный результат с заданной ошибкой.</returns>
	public static Result<T> Failure<T>(Error error) => new(error);

	/// <summary>
	/// Создает неуспешный результат с заданной ошибкой.
	/// </summary>
	/// <param name="error">Ошибка, связанная с результатом.</param>
	/// <returns>Неуспешный результат с заданной ошибкой.</returns>
	public static Result Failure(Error error) => new(error);

	/// <summary>
	/// Преобразует результат в значение указанного типа при успешном результате.
	/// </summary>
	/// <typeparam name="T">Тип значения результата.</typeparam>
	/// <param name="onSuccess">Функция для преобразования успешного результата.</param>
	/// <returns>Результат с преобразованным значением или ошибкой.</returns>
	public Result<T> Map<T>(Func<T> onSuccess) => IsSuccess ? Success(onSuccess()) : Failure<T>(Error);
}

/// <summary>
/// Результат операции с возвращаемым значением определенного типа.
/// </summary>
/// <typeparam name="T">Тип значения результата.</typeparam>
public class Result<T> : Result
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Result{T}"/> с успешным значением.
	/// </summary>
	/// <param name="value">Значение успешного результата.</param>
	internal Result(T value)
	{
		Value = value;
	}

	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Result{T}"/> с ошибкой.
	/// </summary>
	/// <param name="error">Ошибка, связанная с результатом.</param>
	internal Result(Error error)
		: base(error)
	{
		Value = default!;
	}

	/// <summary>
	/// Значение результата типа <typeparamref name="T"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">Попытка получить значение неуспешного результата.</exception>
	public T Value =>
		!IsSuccess
			? throw new InvalidOperationException($"Нельзя получить доступ к неуспешному {nameof(Result)}")
			: field;

	/// <summary>
	/// Импlicitное преобразование значения в результат.
	/// </summary>
	/// <param name="value">Значение для преобразования в результат.</param>
	public static implicit operator Result<T>(T value) => new(value);

	/// <summary>
	/// Импlicitное преобразование ошибки в результат.
	/// </summary>
	/// <param name="error">Ошибка для преобразования в результат.</param>
	public static implicit operator Result<T>(Error error) => new(error);

	/// <summary>
	/// Импlicitное преобразование результата в значение типа <typeparamref name="T"/>.
	/// </summary>
	/// <param name="result">Результат для преобразования в значение типа <typeparamref name="T"/>.</param>
	public static implicit operator T(Result<T> result) => result.Value;
}
