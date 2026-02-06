namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

/// <summary>
/// Класс, представляющий уникальную последовательность элементов.
/// </summary>
/// <typeparam name="T">Тип элементов последовательности.</typeparam>
public sealed class UniqueSequence<T>
{
	private readonly HashSet<T> _items;

	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="UniqueSequence{T}"/>.
	/// </summary>
	/// <param name="items">Коллекция уникальных элементов.</param>
	internal UniqueSequence(HashSet<T> items)
	{
		_items = items;
	}

	/// <summary>
	/// Создает экземпляр <see cref="UniqueSequence{T}"/> из коллекции элементов.
	/// </summary>
	/// <param name="items">Коллекция элементов.</param>
	/// <returns>Результат с уникальной последовательностью или ошибкой.</returns>
	public static Result<UniqueSequence<T>> Create(IEnumerable<T> items)
	{
		HashSet<T> set = [];

		foreach (T item in items)
		{
			if (!set.Add(item))
			{
				return Error.Validation("Коллекция не уникальна.");
			}
		}

		return new UniqueSequence<T>(set);
	}

	/// <summary>
	/// Создает экземпляр <see cref="UniqueSequence{T}"/> из коллекции элементов.
	/// </summary>
	/// <param name="items">Коллекция элементов.</param>
	/// <param name="onError">Сообщение об ошибке в случае неуникальности элементов.</param>
	/// <returns>Результат с уникальной последовательностью или ошибкой.</returns>
	public static Result<UniqueSequence<T>> Create(IEnumerable<T> items, string onError)
	{
		Result<UniqueSequence<T>> sequence = Create(items);
		return sequence.IsFailure ? Error.Validation(onError) : sequence;
	}

	/// <summary>
	/// Создает экземпляр <see cref="UniqueSequence{T}"/> из коллекции элементов с использованием ключевого селектора.
	/// </summary>
	/// <typeparam name="TSource">Тип ключа для определения уникальности элементов.</typeparam>
	/// <param name="items">Коллекция элементов.</param>
	/// <param name="keySelector">Функция для выбора ключа из элемента.</param>
	/// <param name="comparer">Компаратор для сравнения ключей.</param>
	/// <returns>Результат с уникальной последовательностью или ошибкой.</returns>
	public static Result<UniqueSequence<T>> Create<TSource>(
		IEnumerable<T> items,
		Func<T, TSource> keySelector,
		IEqualityComparer<TSource>? comparer = null
	)
	{
		comparer ??= EqualityComparer<TSource>.Default;
		HashSet<TSource> seenKeys = new(comparer);
		HashSet<T> result = [];

		foreach (T item in items)
		{
			TSource? key = keySelector(item);
			{
				if (!seenKeys.Add(key))
				{
					return Error.Validation("Коллекция не уникальна.");
				}
			}

			result.Add(item);
		}

		return new UniqueSequence<T>(result);
	}

	/// <summary>
	/// Создает экземпляр <see cref="UniqueSequence{T}"/> из коллекции элементов с использованием ключевого селектора.
	/// </summary>
	/// <typeparam name="TSource">Тип ключа для определения уникальности элементов.</typeparam>
	/// <param name="items">Коллекция элементов.</param>
	/// <param name="keySelector">Функция для выбора ключа из элемента.</param>
	/// <param name="onError">Сообщение об ошибке в случае неуникальности элементов.</param>
	/// <param name="comparer">Компаратор для сравнения ключей.</param>
	/// <returns>Результат с уникальной последовательностью или ошибкой.</returns>
	public static Result<UniqueSequence<T>> Create<TSource>(
		IEnumerable<T> items,
		Func<T, TSource> keySelector,
		string onError,
		IEqualityComparer<TSource>? comparer = null
	)
	{
		Result<UniqueSequence<T>> sequence = Create(items, keySelector, comparer);
		return sequence.IsFailure ? Error.Validation(onError) : sequence;
	}

	/// <summary>
	/// Преобразует элементы уникальной последовательности в другой тип.
	/// </summary>
	/// <typeparam name="U">Тип элементов результирующей последовательности.</typeparam>
	/// <param name="selector">Функция для преобразования элементов.</param>
	/// <returns>Последовательность преобразованных элементов.</returns>
	public IEnumerable<U> Map<U>(Func<T, U> selector)
	{
		return _items.Select(selector);
	}

	/// <summary>
	/// Преобразует элементы уникальной последовательности в массив другого типа.
	/// </summary>
	/// <typeparam name="U">Тип элементов результирующего массива.</typeparam>
	/// <param name="factory">Функция для создания элементов массива.</param>
	/// <returns>Массив преобразованных элементов.</returns>
	public U[] MapToArray<U>(Func<T, U> factory)
	{
		int count = _items.Count;
		U[] array = new U[count];
		int current = 0;
		foreach (T item in _items)
		{
			array[current] = factory(item);
			current++;
		}

		return array;
	}
}
