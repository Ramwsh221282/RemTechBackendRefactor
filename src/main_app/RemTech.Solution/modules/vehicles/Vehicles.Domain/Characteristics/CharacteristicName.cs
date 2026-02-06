using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics;

/// <summary>
/// Имя характеристики.
/// </summary>
public sealed record CharacteristicName
{
	private const int MAX_LENGTH = 128;

	private CharacteristicName(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение имени характеристики.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт имя характеристики из строки.
	/// </summary>
	/// <param name="value">Строковое значение имени характеристики.</param>
	/// <returns>Результат создания имени характеристики.</returns>
	public static Result<CharacteristicName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return Error.Validation("Имя характеристики не может быть пустым.");
		}

		return value.Length > MAX_LENGTH
			? Error.Validation($"Имя характеристики не может быть больше {MAX_LENGTH} символов.")
			: new CharacteristicName(value);
	}
}
