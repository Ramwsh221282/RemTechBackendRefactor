using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

public sealed class OnPermantlyStartParsingEventTransporter(IOnParserStartedListener listener)
	: IEventTransporter<PermantlyStartParsingCommand, SubscribedParser>
{
	public async Task Transport(SubscribedParser result, CancellationToken ct = default) =>
		await listener.Handle(result, ct);
}
