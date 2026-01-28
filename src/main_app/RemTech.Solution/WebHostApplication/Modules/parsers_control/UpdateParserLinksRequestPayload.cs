namespace WebHostApplication.Modules.parsers_control;

/// <summary>
/// Запрос на обновление ссылок парсера.
/// </summary>
/// <param name="LinkId">Идентификатор ссылки парсера.</param>
/// <param name="Activity">Активность ссылки парсера.</param>
/// <param name="Name">Название ссылки парсера.</param>
/// <param name="Url">URL ссылки парсера.</param>
public sealed record UpdateParserLinksRequestPayload(
    Guid LinkId,
    bool? Activity = null,
    string? Name = null,
    string? Url = null
);
