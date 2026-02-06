using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Адрес запчасти.
/// </summary>
public sealed record SpareAddress
{
	private SpareAddress(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение адреса.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт адрес запчасти.
	/// </summary>
	/// <param name="value">Значение адреса.</param>
	/// <returns>Результат создания адреса запчасти.</returns>
	public static Result<SpareAddress> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("Адрес не может быть пустым")
			: new SpareAddress(value);
	}
}
