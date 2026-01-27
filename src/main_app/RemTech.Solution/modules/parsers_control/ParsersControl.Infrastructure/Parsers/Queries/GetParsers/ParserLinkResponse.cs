using ParsersControl.Core.ParserLinks.Models;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

/// <summary>
/// Ответ с информацией о ссылке на парсер.
/// </summary>
/// <param name="Id">Идентификатор ссылки на парсер.</param>
/// <param name="IsActive">Статус активности ссылки на парсер.</param>
/// <param name="UrlName">Имя URL ссылки на парсер.</param>
/// <param name="UrlValue">Значение URL ссылки на парсер.</param>
public sealed record ParserLinkResponse(Guid Id, bool IsActive, string UrlName, string UrlValue)
{
	/// <summary>
	///   Создаёт ответ с информацией о ссылке на парсер из модели ссылки на подписанный парсер.
	/// </summary>
	/// <param name="link">Модель ссылки на подписанный парсер.</param>
	/// <returns>Ответ с информацией о ссылке на парсер.</returns>
	public static ParserLinkResponse Create(SubscribedParserLink link) =>
		new(link.Id.Value, link.Active, link.UrlInfo.Name, link.UrlInfo.Url);
}
