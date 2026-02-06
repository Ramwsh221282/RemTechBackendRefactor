namespace ParsersControl.Core.Features.UpdateParserLinks;

/// <summary>
/// Параметры команды обновления ссылки парсера.
/// </summary>
/// <param name="LinkId">Идентификатор ссылки парсера.</param>
/// <param name="Activity">Признак активности ссылки парсера.</param>
/// <param name="Name">Имя ссылки парсера.</param>
/// <param name="Url">URL ссылки парсера.</param>
public sealed record UpdateParserLinksCommandInfo(
	Guid LinkId,
	bool? Activity = null,
	string? Name = null,
	string? Url = null
);
