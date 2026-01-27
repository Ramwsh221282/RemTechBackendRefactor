using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetParsedAmount;

/// <summary>
/// Команда установки количества распарсенных элементов.
/// </summary>
/// <param name="Id">Идентификатор элемента.</param>
/// <param name="Amount">Количество распарсенных элементов.</param>
public sealed record SetParsedAmountCommand(Guid Id, int Amount) : ICommand;
