using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetLinkParsedAmount;

public sealed record SetLinkParsedAmountCommand(Guid ParserId, Guid LinkId, int Amount) : ICommand;