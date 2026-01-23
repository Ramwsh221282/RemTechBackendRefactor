using System.Reflection;

namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public class Optional
{
	public bool HasValue { get; }
	public bool NoValue => !HasValue;

	protected Optional(bool hasValue)
	{
		HasValue = hasValue;
	}

	public U Map<U>(Func<U> onHasValue, Func<U> onNoneValue) => HasValue ? onHasValue() : onNoneValue();

	public U Map<U>(U onHasValue, U onNoneValue) => HasValue ? onHasValue : onNoneValue;

	public static Optional<T> Some<T>(T value) => new(value);

	public static Optional<T> FromNullable<T>(T? value) => value is null ? None<T>() : Some(value);

	public static Optional<DateTime> FromNullable(DateTime? value) =>
		value.HasValue ? Some(value.Value) : None<DateTime>();

	public static Optional<T> None<T>() => new();

	public static bool AllValuesExist(IEnumerable<Optional> optionals) => optionals.All(optional => optional.HasValue);

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
}

public sealed class Optional<T> : Optional
{
	public T Value =>
		HasValue
			? field!
			: throw new InvalidOperationException($"Нельзя получить доступ к пустому значению {nameof(Optional<>)}");

	internal Optional(T value)
		: base(true)
	{
		Value = value;
	}

	internal Optional()
		: base(false)
	{
		Value = default;
	}

	public void ExecuteOnValue(Action action)
	{
		if (HasValue)
			action();
	}

	public void ExecuteOnValue(Action<T> action)
	{
		if (HasValue)
			action(Value);
	}

	public async Task ExecuteOnValueAsync(Func<Task> action)
	{
		if (HasValue)
			await action();
	}

	public async Task ExecuteOnValueAsync(Func<T, Task> action)
	{
		if (HasValue)
			await action(Value);
	}

	public static Optional<T> Some(T value) => new(value);

	public static Optional<T> None() => new();

	public static implicit operator Optional<T>(T value)
	{
		return Some(value);
	}
}
