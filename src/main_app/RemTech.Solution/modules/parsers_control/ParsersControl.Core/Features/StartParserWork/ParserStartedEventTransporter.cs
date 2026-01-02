using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.StartParserWork;

public sealed class ParserStartedEventTransporter(IOnParserStartedListener listener) : IEventTransporter<StartParserCommand, SubscribedParser>
{
    public async Task Transport(SubscribedParser result, CancellationToken ct = new CancellationToken())
    {
        await listener.Handle(result, ct);
    }
}