namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

/// <summary>
/// Класс, представляющий тип Unit.
/// </summary>
public sealed record Unit
{
	private Unit() { }

	/// <summary>
	/// Единственный экземпляр типа Unit.
	/// </summary>
	public static Unit Value { get; } = new();

	/// <summary>
	/// Создает экземпляр <see cref="Unit{T}"/> с заданным значением.
	/// </summary>
	/// <typeparam name="T">Тип значения.</typeparam>
	/// <param name="value">Значение для создания экземпляра Unit.</param>
	/// <returns>Экземпляр Unit с заданным значением.</returns>
	public static Unit<T> UnitOf<T>(T value)
	{
		return new(value);
	}
}

/// <summary>
/// Класс, представляющий тип Unit с значением.
/// </summary>
/// <typeparam name="T">Тип значения.</typeparam>
public sealed record Unit<T>
{
	private readonly T _value;

	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Unit{T}"/>.
	/// </summary>
	/// <param name="value">Значение для создания экземпляра Unit.</param>
	internal Unit(T value)
	{
		_value = value;
	}

	/// <summary>
	/// Выполняет асинхронную операцию с использованием значения Unit.
	/// </summary>
	/// <param name="operation">Асинхронная операция, использующая значение Unit.</param>
	/// <returns>Результат выполнения операции.</returns>
	public async Task<Result<Unit>> Executed(Func<T, Task> operation)
	{
		await operation(_value);
		return Unit.Value;
	}
}

/// <summary>
/// Расширения для работы с типом Unit.
/// </summary>
public static class UnitModule
{
	extension(Unit)
	{
		public static async Task<Result<Unit>> Executed(Func<Task> operation)
		{
			await operation();
			return Unit.Value;
		}

		public static Result<Unit> ValidationUnit(IEnumerable<string> errors)
		{
			return errors.Any() ? Error.Validation(errors) : Unit.Value;
		}
	}
}
