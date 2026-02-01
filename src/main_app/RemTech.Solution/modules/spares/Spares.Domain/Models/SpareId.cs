using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Идентификатор запчасти.
/// </summary>
public sealed record SpareId
{
	private SpareId(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение идентификатора.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт идентификатор из строки.
	/// </summary>
	/// <param name="value">Строковое значение идентификатора.</param>
	/// <returns>Результат создания идентификатора.</returns>
	public static Result<SpareId> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("Идентификатор запчасти не может быть пустым.")
			: Result.Success(new SpareId(value));
	}
}
