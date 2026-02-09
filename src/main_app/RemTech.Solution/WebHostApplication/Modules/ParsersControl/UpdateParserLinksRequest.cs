namespace WebHostApplication.Modules.ParsersControl;

/// <summary>
///  Запрос на обновление ссылок парсера.
/// </summary>
/// <param name="Links">Список ссылок для обновления.</param>
public sealed record UpdateParserLinksRequest(IEnumerable<UpdateParserLinksRequestPayload> Links);
