using ParsersControl.Core.ParserLinks.Models;

namespace WebHostApplication.Modules.ParsersControl;

/// <summary>
/// Ответ с информацией о ссылке парсера.
/// </summary>
public sealed class ParserLinkResponseModel
{
	/// <summary>
	/// Идентификатор ссылки парсера.
	/// </summary>
	public required Guid Id { get; init; }

	/// <summary>
	/// Название ссылки парсера.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// URL ссылки парсера.
	/// </summary>
	public required string Url { get; init; }

	/// <summary>
	/// Количество распарсенных элементов по ссылке парсера.
	/// </summary>
	public required int ParsedCount { get; init; }

	/// <summary>
	/// Время работы ссылки парсера в секундах.
	/// </summary>
	public required long WorkTime { get; init; }

	/// <summary>
	/// Статус активности ссылки парсера.
	/// </summary>
	public required bool IsActive { get; init; }

	/// <summary>
	/// Часы работы ссылки парсера.
	/// </summary>
	public required int Hours { get; init; }

	/// <summary>
	/// Минуты работы ссылки парсера.
	/// </summary>
	public required int Minutes { get; init; }

	/// <summary>
	/// Секунды работы ссылки парсера.
	/// </summary>
	public required int Seconds { get; init; }

	/// <summary>
	/// Преобразование из доменной модели ссылки парсера в ответ.
	/// </summary>
	/// <param name="link">Доменная модель ссылки парсера.</param>
	/// <returns>Ответ с информацией о ссылке парсера.</returns>
	public static ParserLinkResponseModel ConvertFrom(SubscribedParserLink link) =>
		new()
		{
			Id = link.Id.Value,
			Name = link.UrlInfo.Name,
			Url = link.UrlInfo.Url,
			ParsedCount = link.Statistics.ParsedCount.Value,
			WorkTime = link.Statistics.WorkTime.TotalElapsedSeconds,
			IsActive = link.Active,
			Hours = link.Statistics.WorkTime.Hours,
			Minutes = link.Statistics.WorkTime.Minutes,
			Seconds = link.Statistics.WorkTime.Seconds,
		};

	/// <summary>
	/// Преобразование из коллекции доменных моделей ссылок парсера в ответы.
	/// </summary>
	/// <param name="links">Коллекция доменных моделей ссылок парсера.</param>
	/// <returns>Коллекция ответов с информацией о ссылках парсера.</returns>
	public static IEnumerable<ParserLinkResponseModel> ConvertFrom(IEnumerable<SubscribedParserLink> links) =>
		[.. links.Select(ConvertFrom)];
}
