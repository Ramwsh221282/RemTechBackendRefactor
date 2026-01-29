namespace WebHostApplication.Modules.ParsersControl;

/// <summary>
/// Запрос на добавление ссылки к парсеру.
/// </summary>
/// <param name="Name">Название ссылки для добавления к парсеру.</param>
/// <param name="Url">URL ссылки для добавления к парсеру.</param>
public sealed record AddLinkToParserRequestBody(string Name, string Url);
