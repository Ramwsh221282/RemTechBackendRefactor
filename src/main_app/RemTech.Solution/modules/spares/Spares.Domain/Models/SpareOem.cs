using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// OEM-номер запчасти.
/// </summary>
public sealed record SpareOem
{
	private SpareOem(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение OEM-номера.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт OEM-номер из строки.
	/// </summary>
	/// <param name="value">Строковое значение OEM-номера.</param>
	/// <returns>Результат создания OEM-номера.</returns>
	public static Result<SpareOem> Create(string value) =>
		string.IsNullOrWhiteSpace(value)
			? Error.Validation("OEM-номер запчасти не может быть пустым.")
			: Result.Success(new SpareOem(value));
}
