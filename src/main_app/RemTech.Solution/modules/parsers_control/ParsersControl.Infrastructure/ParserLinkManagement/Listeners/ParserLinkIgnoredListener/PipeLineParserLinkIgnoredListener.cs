using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkIgnoredListener;

public sealed class PipeLineParserLinkIgnoredListener : IParserLinkIgnoredListener
{
    private readonly Queue<IParserLinkIgnoredListener> _listeners;

    public PipeLineParserLinkIgnoredListener AddListener(IParserLinkIgnoredListener listener)
    {
        _listeners.Enqueue(listener);
        return this;
    }

    public PipeLineParserLinkIgnoredListener()
    {
        _listeners = [];
    }
    
    public PipeLineParserLinkIgnoredListener(IEnumerable<IParserLinkIgnoredListener> listeners)
    {
        _listeners = new Queue<IParserLinkIgnoredListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserLinkIgnoredListener listener = _listeners.Dequeue();
            Result<Unit> reacting = await listener.React(data, ct);
            if (reacting.IsFailure) return reacting.Error;
        }

        return Unit.Value;
    }
}