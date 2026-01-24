using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Core.Features.StartParserWork;

public sealed class ParserStartedEventTransporter(IOnParserStartedListener listener)
	: IEventTransporter<StartParserCommand, SubscribedParser>
{
	public Task Transport(SubscribedParser result, CancellationToken ct = default) => listener.Handle(result, ct);
}
