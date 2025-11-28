using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUrlChangedListener;

public sealed class PipeLineParserLinkUrlChangedListener : IParserLinkUrlChangedListener
{
    private readonly Queue<IParserLinkUrlChangedListener> _listeners;

    public PipeLineParserLinkUrlChangedListener Add(IParserLinkUrlChangedListener listener)
    {
        _listeners.Enqueue(listener);
        return this;
    }

    public PipeLineParserLinkUrlChangedListener()
    {
        _listeners = [];
    }

    public PipeLineParserLinkUrlChangedListener(IEnumerable<IParserLinkUrlChangedListener> listeners)
    {
        _listeners = new Queue<IParserLinkUrlChangedListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserLinkUrlChangedListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure) return result.Error;
        }

        return Unit.Value;
    }
}