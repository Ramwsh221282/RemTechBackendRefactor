using System.Reflection;

namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

/// <summary>
/// Класс, представляющий необязательное значение.
/// </summary>
public class Optional
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Optional"/>.
	/// </summary>
	/// <param name="hasValue">Флаг, указывающий, содержит ли объект значение.</param>
	protected Optional(bool hasValue)
	{
		HasValue = hasValue;
	}

	/// <summary>
	/// Указывает, содержит ли объект значение.
	/// </summary>
	public bool HasValue { get; }

	/// <summary>
	/// Указывает, не содержит ли объект значение.
	/// </summary>
	public bool NoValue => !HasValue;

	/// <summary>
	/// Создает экземпляр <see cref="Optional{T}"/> с заданным значением.
	/// </summary>
	/// <typeparam name="T">Тип значения.</typeparam>
	/// <param name="value">Значение.</param>
	/// <returns>Экземпляр <see cref="Optional{T}"/> с заданным значением.</returns>
	public static Optional<T> Some<T>(T value)
	{
		return new(value);
	}

	/// <summary>
	/// Создает экземпляр <see cref="Optional{T}"/> из nullable значения.
	/// </summary>
	/// <typeparam name="T">Тип значения.</typeparam>
	/// <param name="value">Nullable значение.</param>
	/// <returns>Экземпляр <see cref="Optional{T}"/> с заданным значением или пустой экземпляр, если значение null.</returns>
	public static Optional<T> FromNullable<T>(T? value)
	{
		return value is null ? None<T>() : Some(value);
	}

	/// <summary>
	/// Создает экземпляр <see cref="Optional{DateTime}"/> из nullable значения DateTime.
	/// </summary>
	/// <param name="value">Nullable значение DateTime.</param>
	/// <returns>Экземпляр <see cref="Optional{DateTime}"/> с заданным значением или пустой экземпляр, если значение null.</returns>
	public static Optional<DateTime> FromNullable(DateTime? value)
	{
		return value.HasValue ? Some(value.Value) : None<DateTime>();
	}

	/// <summary>
	/// Создает пустой экземпляр <see cref="Optional{T}"/>.
	/// </summary>
	/// <typeparam name="T">Тип значения.</typeparam>
	/// <returns>Пустой экземпляр <see cref="Optional{T}"/>.</returns>
	public static Optional<T> None<T>()
	{
		return new();
	}

	/// <summary>
	/// Проверяет, существуют ли все значения в коллекции <see cref="Optional"/>.
	/// </summary>
	/// <param name="optionals">Коллекция объектов <see cref="Optional"/>.</param>
	/// <returns>True, если все объекты содержат значения; в противном случае - false.</returns>
	public static bool AllValuesExist(IEnumerable<Optional> optionals)
	{
		return optionals.All(optional => optional.HasValue);
	}

	/// <summary>
	/// Проверяет, существуют ли все значения в объекте, содержащем поля типа <see cref="Optional{T}"/>.
	/// </summary>
	/// <typeparam name="T">Тип объекта.</typeparam>
	/// <param name="object">Объект, содержащий поля типа <see cref="Optional{T}"/>.</param>
	/// <returns>True, если все поля содержат значения; в противном случае - false.</returns>
	public static bool AllValuesExist<T>(T @object)
		where T : notnull
	{
		IEnumerable<FieldInfo> fields = @object
			.GetType()
			.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.Where(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(Optional<>));

		IEnumerable<Optional> optionals = fields.Select(f => (Optional)f.GetValue(@object)!);
		return AllValuesExist(optionals);
	}

	/// <summary>
	/// Преобразует объект <see cref="Optional"/> в значение указанного типа.
	/// </summary>
	/// <typeparam name="U">Тип возвращаемого значения.</typeparam>
	/// <param name="onHasValue">Функция, вызываемая при наличии значения.</param>
	/// <param name="onNoneValue">Функция, вызываемая при отсутствии значения.</param>
	/// <returns>Значение типа <typeparamref name="U"/>, возвращаемое одной из функций.</returns>
	public U Map<U>(Func<U> onHasValue, Func<U> onNoneValue)
	{
		return HasValue ? onHasValue() : onNoneValue();
	}

	/// <summary>
	/// Преобразует объект <see cref="Optional"/> в значение указанного типа.
	/// </summary>
	/// <typeparam name="U">Тип возвращаемого значения.</typeparam>
	/// <param name="onHasValue">Значение, возвращаемое при наличии значения.</param>
	/// <param name="onNoneValue">Значение, возвращаемое при отсутствии значения.</param>
	/// <returns>Значение типа <typeparamref name="U"/>, возвращаемое одним из параметров.</returns>
	public U Map<U>(U onHasValue, U onNoneValue)
	{
		return HasValue ? onHasValue : onNoneValue;
	}
}

/// <summary>
/// Класс, представляющий необязательное значение определенного типа.
/// </summary>
/// <typeparam name="T">Тип значения.</typeparam>
public sealed class Optional<T> : Optional
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Optional{T}"/>.
	/// </summary>
	/// <param name="value">Значение.</param>
	internal Optional(T value)
		: base(true)
	{
		Value = value;
	}

	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="Optional{T}"/> без значения.
	/// </summary>
	internal Optional()
		: base(false)
	{
		Value = default!;
	}

	/// <summary>
	/// Значение типа <typeparamref name="T"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">Попытка получить значение, когда оно отсутствует.</exception>
	public T Value =>
		HasValue
			? field
			: throw new InvalidOperationException($"Нельзя получить доступ к пустому значению {nameof(Optional<>)}");

	public static implicit operator Optional<T>(T value)
	{
		return Some(value);
	}

	/// <summary>
	/// Создает экземпляр <see cref="Optional{T}"/> с заданным значением.
	/// </summary>
	/// <param name="value">Значение для создания экземпляра.</param>
	/// <returns>Экземпляр <see cref="Optional{T}"/> с заданным значением.</returns>
	public static Optional<T> Some(T value)
	{
		return new(value);
	}

	/// <summary>
	/// Создает пустой экземпляр <see cref="Optional{T}"/>.
	/// </summary>
	/// <returns>Пустой экземпляр <see cref="Optional{T}"/>.</returns>
	public static Optional<T> None()
	{
		return new();
	}

	/// <summary>
	/// Выполняет указанное действие, если значение присутствует.
	/// </summary>
	/// <param name="action">Действие, выполняемое при наличии значения.</param>
	public void ExecuteOnValue(Action action)
	{
		if (HasValue)
		{
			action();
		}
	}

	/// <summary>
	/// Выполняет указанное действие с значением, если оно присутствует.
	/// </summary>
	/// <param name="action">Действие, выполняемое с значением при его наличии.</param>
	public void ExecuteOnValue(Action<T> action)
	{
		if (HasValue)
		{
			action(Value);
		}
	}

	/// <summary>
	/// Выполняет указанное асинхронное действие, если значение присутствует.
	/// </summary>
	/// <param name="action">Асинхронное действие, выполняемое при наличии значения.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task ExecuteOnValueAsync(Func<Task> action)
	{
		if (HasValue)
		{
			await action();
		}
	}

	/// <summary>
	/// Выполняет указанное асинхронное действие с значением, если оно присутствует.
	/// </summary>
	/// <param name="action">Асинхронное действие, выполняемое с значением при его наличии.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task ExecuteOnValueAsync(Func<T, Task> action)
	{
		if (HasValue)
		{
			await action(Value);
		}
	}
}
