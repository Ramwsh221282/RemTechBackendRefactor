using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUnignoredListener;

public sealed class PipeLineParserLinkUnignoredListener : IParserLinkUnignoredListener
{
    private readonly Queue<IParserLinkUnignoredListener> _listeners;

    public PipeLineParserLinkUnignoredListener Add(IParserLinkUnignoredListener listener)
    {
        _listeners.Enqueue(listener);
        return this;
    }

    public PipeLineParserLinkUnignoredListener()
    {
        _listeners = [];
    }

    public PipeLineParserLinkUnignoredListener(IEnumerable<IParserLinkUnignoredListener> listeners)
    {
        _listeners = new Queue<IParserLinkUnignoredListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserLinkUnignoredListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure) return result.Error;
        }

        return Unit.Value;
    }
}