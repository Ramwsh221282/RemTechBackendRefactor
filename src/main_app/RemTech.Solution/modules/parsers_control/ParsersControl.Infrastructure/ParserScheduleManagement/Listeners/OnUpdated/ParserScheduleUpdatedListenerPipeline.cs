using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnUpdated;

public sealed class ParserScheduleUpdatedListenerPipeline : IParserScheduleUpdatedEventListener
{
    private readonly Queue<IParserScheduleUpdatedEventListener> _listeners;

    public ParserScheduleUpdatedListenerPipeline(IEnumerable<IParserScheduleUpdatedEventListener> listeners)
    {
        _listeners = new Queue<IParserScheduleUpdatedEventListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserScheduleUpdatedEventListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure) return result.Error;
        }
        
        return Unit.Value;
    }
}