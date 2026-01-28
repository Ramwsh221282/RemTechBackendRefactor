using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Тип запчасти.
/// </summary>
public sealed record SpareType
{
	private SpareType(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение типа запчасти.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт тип запчасти из строки.
	/// </summary>
	/// <param name="value">Строковое представление типа запчасти.</param>
	/// <returns>Результат создания типа запчасти.</returns>
	public static Result<SpareType> Create(string value) =>
		string.IsNullOrWhiteSpace(value) ? Error.Validation("Тип запчасти не может быть пустым") : new SpareType(value);
}
