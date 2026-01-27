using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.DeleteLinkFromParser;

/// <summary>
/// Команда удаления ссылки из парсера.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="LinkId">Идентификатор ссылки на парсер.</param>
public sealed record DeleteLinkFromParserCommand(Guid ParserId, Guid LinkId) : ICommand;
