using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

/// <summary>
/// Идентификатор элемента сервиса.
/// </summary>
public sealed record ServiceItemId
{
	private const int MaxLength = 255;

	private ServiceItemId(string value) => Value = value;

	/// <summary>
	/// Значение идентификатора элемента сервиса.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает идентификатор элемента сервиса из заданного значения.
	/// </summary>
	/// <param name="value">Значение идентификатора элемента сервиса.</param>
	/// <returns>Результат создания идентификатора элемента сервиса.</returns>
	public static Result<ServiceItemId> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Идентификатор сохраняемого элемента не может быть пустым.");
		return value.Length > MaxLength
			? Error.Validation($"Идентификатор сохраняемого элемента не может превышать {MaxLength} символов.")
			: new ServiceItemId(value);
	}
}
