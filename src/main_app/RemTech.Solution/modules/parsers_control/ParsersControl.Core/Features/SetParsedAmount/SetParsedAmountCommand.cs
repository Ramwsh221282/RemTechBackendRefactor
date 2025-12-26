using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetParsedAmount;

public sealed record SetParsedAmountCommand(Guid Id, int Amount) : ICommand;