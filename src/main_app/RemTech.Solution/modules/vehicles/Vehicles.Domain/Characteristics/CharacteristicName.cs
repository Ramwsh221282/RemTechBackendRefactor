using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics;

public sealed record CharacteristicName
{
	private const int MaxLength = 128;
	public string Value { get; }

	private CharacteristicName(string value)
	{
		Value = value;
	}

	public static Result<CharacteristicName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Имя характеристики не может быть пустым.");
		if (value.Length > MaxLength)
			return Error.Validation($"Имя характеристики не может быть больше {MaxLength} символов.");
		return new CharacteristicName(value);
	}
}
