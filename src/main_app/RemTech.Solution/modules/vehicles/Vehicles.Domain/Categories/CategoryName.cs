using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Categories;

/// <summary>
/// Имя категории.
/// </summary>
public sealed record CategoryName
{
	private const int MAX_LENGTH = 128;

	private CategoryName(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение имени категории.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт имя категории из строки.
	/// </summary>
	/// <param name="value">Строковое значение имени категории.</param>
	/// <returns>Результат создания имени категории.</returns>
	public static Result<CategoryName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Имя категории не может быть пустым.");
		return value.Length > MAX_LENGTH
			? Error.Validation($"Имя категории не может быть больше {MAX_LENGTH} символов.")
			: new CategoryName(value);
	}
}
