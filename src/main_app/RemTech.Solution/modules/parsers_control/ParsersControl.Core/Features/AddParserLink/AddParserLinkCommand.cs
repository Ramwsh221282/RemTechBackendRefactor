using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.AddParserLink;

/// <summary>
/// Команда добавления ссылки на парсер.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="Links">Коллекция ссылок для добавления.</param>
public sealed record AddParserLinkCommand(Guid ParserId, IEnumerable<AddParserLinkCommandArg> Links) : ICommand;
