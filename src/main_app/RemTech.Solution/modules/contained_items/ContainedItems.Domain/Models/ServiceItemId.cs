using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed record ServiceItemId
{
	private const int MaxLength = 255;

	private ServiceItemId(string value) => Value = value;

	public string Value { get; }

	public static Result<ServiceItemId> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Идентификатор сохраняемого элемента не может быть пустым.");
		return value.Length > MaxLength
			? Error.Validation($"Идентификатор сохраняемого элемента не может превышать {MaxLength} символов.")
			: new ServiceItemId(value);
	}
}
