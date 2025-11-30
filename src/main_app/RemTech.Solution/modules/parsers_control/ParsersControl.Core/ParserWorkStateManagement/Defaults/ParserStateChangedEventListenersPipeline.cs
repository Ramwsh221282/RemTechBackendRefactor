using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkStateManagement.Defaults;

public sealed class ParserStateChangedEventListenersPipeline : IParserStateChangedEventListener
{
    private readonly Queue<IParserStateChangedEventListener> _listeners;

    public ParserStateChangedEventListenersPipeline(IEnumerable<IParserStateChangedEventListener> listeners)
    {
        _listeners = new Queue<IParserStateChangedEventListener>(listeners);
    }

    public async Task<Result<Unit>> React(ParserWorkTurnerState state, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserStateChangedEventListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(state, ct);
            if (result.IsFailure) return result.Error;
        }
        
        return Unit.Value;
    }
}