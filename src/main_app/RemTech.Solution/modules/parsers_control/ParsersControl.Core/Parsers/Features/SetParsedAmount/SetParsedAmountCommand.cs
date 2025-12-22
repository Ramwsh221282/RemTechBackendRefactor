using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.SetParsedAmount;

public sealed record SetParsedAmountCommand(Guid Id, int Amount) : ICommand;