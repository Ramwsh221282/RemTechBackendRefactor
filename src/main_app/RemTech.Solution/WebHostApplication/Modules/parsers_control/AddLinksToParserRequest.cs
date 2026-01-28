namespace WebHostApplication.Modules.parsers_control;

/// <summary>
/// Запрос на добавление ссылок к парсеру.
/// </summary>
/// <param name="Links">Ссылки для добавления к парсеру.</param>
public sealed record AddLinksToParserRequest(IEnumerable<AddLinkToParserRequestBody> Links);
