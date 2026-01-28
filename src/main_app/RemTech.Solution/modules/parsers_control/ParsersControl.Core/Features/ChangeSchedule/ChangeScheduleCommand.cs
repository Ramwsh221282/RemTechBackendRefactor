using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.ChangeSchedule;

/// <summary>
/// Команда изменения расписания парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
/// <param name="WaitDays">Количество дней ожидания перед следующим запуском.</param>
/// <param name="NextRun">Дата и время следующего запуска парсера.</param>
public sealed record ChangeScheduleCommand(Guid Id, int WaitDays, DateTime NextRun) : ICommand;
