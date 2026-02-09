using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetWorkTime;

/// <summary>
/// Команда установки рабочего времени.
/// </summary>
/// <param name="Id">Идентификатор элемента.</param>
/// <param name="TotalElapsedSeconds">Общее количество затраченных секунд.</param>
public sealed record SetWorkTimeCommand(Guid Id, long TotalElapsedSeconds) : ICommand;
