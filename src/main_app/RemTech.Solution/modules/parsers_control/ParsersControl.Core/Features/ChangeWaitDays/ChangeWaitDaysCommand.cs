using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.ChangeWaitDays;

/// <summary>
/// Команда изменения количества дней ожидания перед следующим запуском парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
/// <param name="WaitDays">Новое количество дней ожидания.</param>
public sealed record ChangeWaitDaysCommand(Guid Id, int WaitDays) : ICommand;
