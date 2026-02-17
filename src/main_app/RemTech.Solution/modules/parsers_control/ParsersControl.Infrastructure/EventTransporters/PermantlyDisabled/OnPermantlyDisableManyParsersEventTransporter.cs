using ParsersControl.Core.Features.PermantlyDisableManyParsing;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Infrastructure.EventTransporters.PermantlyDisabled;

public sealed class OnPermantlyDisableManyParsersEventTransporter : IEventTransporter<PermantlyDisableManyParsingCommand, IEnumerable<SubscribedParser>>
{    
    private readonly PermantlyDisabledParserEventEmitter _emitter;

    public OnPermantlyDisableManyParsersEventTransporter(PermantlyDisabledParserEventEmitter emitter)
    {        
        _emitter = emitter;
    }

    public async Task Transport(IEnumerable<SubscribedParser> result, CancellationToken ct = default)
    {        
        foreach (SubscribedParser parser in result)
        {
            await _emitter.Emit(parser, ct);
        }
    }    
}
