using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations;

public sealed record LocationName
{
	private const int MaxLength = 255;

	private LocationName(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<LocationName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Имя локации не может быть пустым.");
		return value.Length > MaxLength
			? Error.Validation($"Имя локации не может быть больше {MaxLength} символов.")
			: new LocationName(value);
	}
}
