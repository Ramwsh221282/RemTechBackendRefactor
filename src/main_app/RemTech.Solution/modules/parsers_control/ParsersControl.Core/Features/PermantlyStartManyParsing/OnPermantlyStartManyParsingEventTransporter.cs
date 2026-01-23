using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

public sealed class OnPermantlyStartManyParsingEventTransporter(IOnParserStartedListener listener)
    : IEventTransporter<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>>
{
    public async Task Transport(IEnumerable<SubscribedParser> result, CancellationToken ct = new CancellationToken())
    {
        foreach (SubscribedParser parser in result)
            await listener.Handle(parser, ct);
    }
}
