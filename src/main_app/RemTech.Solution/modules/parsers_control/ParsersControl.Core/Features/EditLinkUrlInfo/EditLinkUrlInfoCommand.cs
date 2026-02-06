using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.EditLinkUrlInfo;

/// <summary>
/// Команда редактирования информации о ссылке и URL парсера.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="LinkId">Идентификатор ссылки.</param>
/// <param name="NewName">Новое имя ссылки.</param>
/// <param name="NewUrl">Новый URL ссылки.</param>
public sealed record EditLinkUrlInfoCommand(Guid ParserId, Guid LinkId, string? NewName, string? NewUrl) : ICommand;
