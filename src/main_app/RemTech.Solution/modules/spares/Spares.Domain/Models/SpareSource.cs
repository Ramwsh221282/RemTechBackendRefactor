using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Источник информации о запчасти.
/// </summary>
public sealed record SpareSource
{
	/// <summary>
	/// Создаёт экземпляр источника.
	/// </summary>
	/// <param name="url">Ссылка на источник.</param>
	private SpareSource(string url)
	{
		Url = url;
	}

	/// <summary>
	/// Ссылка на источник.
	/// </summary>
	public string Url { get; }

	/// <summary>
	/// Создаёт источник из строки.
	/// </summary>
	/// <param name="url">Ссылка на источник.</param>
	/// <returns>Результат создания источника.</returns>
	public static Result<SpareSource> Create(string url)
	{
		return string.IsNullOrWhiteSpace(url) ? Error.Validation("Ссылка на источник пустая.") : new SpareSource(url);
	}
}
