using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkParserAttachedListener;

public sealed class PipeLineParserLinkAttachedListener : IParserLinkParserAttached
{
    private readonly Queue<IParserLinkParserAttached> _listeners = [];

    public PipeLineParserLinkAttachedListener AddListener(IParserLinkParserAttached listener)
    {
        _listeners.Enqueue(listener);
        return this;
    }

    public PipeLineParserLinkAttachedListener(IEnumerable<IParserLinkParserAttached> listeners)
    {
        _listeners = new Queue<IParserLinkParserAttached>(listeners);
    }

    public PipeLineParserLinkAttachedListener()
    {
        _listeners = [];
    }
    
    public async Task<Result<Unit>> React(
        Guid parserId, 
        ParserLinkData data, 
        CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserLinkParserAttached listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(parserId, data, ct);
            if (result.IsFailure) return result.Error;
        }

        return Unit.Value;
    }
}