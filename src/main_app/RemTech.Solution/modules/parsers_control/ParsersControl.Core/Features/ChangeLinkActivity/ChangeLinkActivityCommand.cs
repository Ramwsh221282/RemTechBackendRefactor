using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.ChangeLinkActivity;

/// <summary>
/// Команда изменения активности ссылки на парсер.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="LinkId">Идентификатор ссылки на парсер.</param>
/// <param name="IsActive">Новое значение активности ссылки.</param>
public sealed record ChangeLinkActivityCommand(Guid ParserId, Guid LinkId, bool IsActive) : ICommand;
