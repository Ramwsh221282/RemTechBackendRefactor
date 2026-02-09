using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Описание запчасти.
/// </summary>
public sealed record SpareTextInfo
{
	/// <summary>
	/// Создаёт экземпляр описания запчасти.
	/// </summary>
	/// <param name="value">Текстовое описание.</param>
	private SpareTextInfo(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Текстовое описание запчасти.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт описание запчасти из строки.
	/// </summary>
	/// <param name="value">Текстовое описание.</param>
	/// <returns>Результат создания описания.</returns>
	public static Result<SpareTextInfo> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("Описание запчасти не может быть пустым.")
			: Result.Success(new SpareTextInfo(value));
	}
}
