using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations;

/// <summary>
/// Имя локации.
/// </summary>
public sealed record LocationName
{
	/// <summary>
	/// Максимальная длина имени локации.
	/// </summary>
	private const int MAX_LENGTH = 255;

	private LocationName(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение имени локации.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт имя локации из строки.
	/// </summary>
	/// <param name="value">Строковое значение имени локации.</param>
	/// <returns>Результат создания имени локации.</returns>
	public static Result<LocationName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Имя локации не может быть пустым.");
		return value.Length > MAX_LENGTH
			? Error.Validation($"Имя локации не может быть больше {MAX_LENGTH} символов.")
			: new LocationName(value);
	}
}
