using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkRenamedListener;

public sealed class PipeLineParserLinkRenamedListener : IParserLinkRenamedListener
{
    private readonly Queue<IParserLinkRenamedListener> _listeners;

    public PipeLineParserLinkRenamedListener()
    {
        _listeners = [];
    }

    public PipeLineParserLinkRenamedListener(IEnumerable<IParserLinkRenamedListener> listeners)
    {
        _listeners = new Queue<IParserLinkRenamedListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserLinkRenamedListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure) return result.Error;
        }

        return Unit.Value;
    }
}