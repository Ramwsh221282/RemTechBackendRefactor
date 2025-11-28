using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserWorkTurning;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStateManagement.EventListeners;

public sealed class OnStatefulParserStateChangedPipeline : IOnStatefulParserStateChangedEventListener
{
    private readonly Queue<IOnStatefulParserStateChangedEventListener> _listeners;

    public OnStatefulParserStateChangedPipeline(IEnumerable<IOnStatefulParserStateChangedEventListener> listeners)
    {
        _listeners = new Queue<IOnStatefulParserStateChangedEventListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(RegisteredParser parser, ParserWorkTurner turner, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IOnStatefulParserStateChangedEventListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(parser, turner, ct);
            if (result.IsFailure) return result;
        }
        
        return Unit.Value;
    }
}