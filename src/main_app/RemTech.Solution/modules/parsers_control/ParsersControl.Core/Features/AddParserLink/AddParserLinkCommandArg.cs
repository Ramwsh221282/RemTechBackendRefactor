namespace ParsersControl.Core.Features.AddParserLink;

/// <summary>
/// Аргумент команды добавления ссылки на парсер.
/// </summary>
/// <param name="LinkUrl">URL ссылки на парсер.</param>
/// <param name="LinkName">Название ссылки на парсер.</param>
public sealed record AddParserLinkCommandArg(string LinkUrl, string LinkName);
