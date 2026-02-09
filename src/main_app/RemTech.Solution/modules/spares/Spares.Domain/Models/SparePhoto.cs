using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Фото запчасти.
/// </summary>
public sealed record SparePhoto
{
	private SparePhoto(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение фото (ссылка или путь).
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт фото из строки.
	/// </summary>
	/// <param name="value">Строковое значение фото.</param>
	/// <returns>Результат создания фото.</returns>
	public static Result<SparePhoto> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("Фото запчасти не может быть пустым.")
			: Result.Success(new SparePhoto(value));
	}
}
