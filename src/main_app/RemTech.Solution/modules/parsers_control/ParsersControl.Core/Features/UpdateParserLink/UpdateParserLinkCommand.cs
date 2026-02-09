using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.UpdateParserLink;

/// <summary>
/// Команда обновления ссылки парсера.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="LinkId">Идентификатор ссылки.</param>
/// <param name="Name">Имя ссылки.</param>
/// <param name="Url">URL ссылки.</param>
public sealed record UpdateParserLinkCommand(Guid ParserId, Guid LinkId, string? Name, string? Url) : ICommand;
