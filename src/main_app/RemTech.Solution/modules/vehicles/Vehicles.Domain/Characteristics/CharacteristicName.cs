using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics;

public sealed record CharacteristicName
{
	private const int MaxLength = 128;

	private CharacteristicName(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<CharacteristicName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Имя характеристики не может быть пустым.");
		return value.Length > MaxLength
			? Error.Validation($"Имя характеристики не может быть больше {MaxLength} символов.")
			: new CharacteristicName(value);
	}
}
