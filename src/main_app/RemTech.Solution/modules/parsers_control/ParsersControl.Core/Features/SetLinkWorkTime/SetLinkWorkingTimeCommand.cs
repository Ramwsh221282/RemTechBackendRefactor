using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetLinkWorkTime;

/// <summary>
/// Команда установки рабочего времени ссылки.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="LinkId">Идентификатор ссылки.</param>
/// <param name="TotalElapsedSeconds">Общее количество затраченных секунд.</param>
public sealed record SetLinkWorkingTimeCommand(Guid ParserId, Guid LinkId, long TotalElapsedSeconds) : ICommand;
