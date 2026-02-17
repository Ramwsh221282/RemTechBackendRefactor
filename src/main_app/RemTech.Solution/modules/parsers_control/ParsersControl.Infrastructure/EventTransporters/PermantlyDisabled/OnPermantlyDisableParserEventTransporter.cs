using ParsersControl.Core.Features.PermantlyDisableParsing;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Infrastructure.EventTransporters.PermantlyDisabled;

public sealed class OnPermantlyDisableParserEventTransporter : IEventTransporter<PermantlyDisableParsingCommand, SubscribedParser>
{
    private readonly PermantlyDisabledParserEventEmitter _emitter;

    public OnPermantlyDisableParserEventTransporter(PermantlyDisabledParserEventEmitter emitter)
    {
        _emitter = emitter;
    }

    public async Task Transport(SubscribedParser result, CancellationToken ct = default)
    {
        await _emitter.Emit(result, ct);
    }
}
