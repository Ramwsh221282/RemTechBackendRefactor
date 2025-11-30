using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnCreated;

public sealed class ParserScheduleCreatedEventListenerPipeline : IParserScheduleCreatedEventListener
{
    private readonly Queue<IParserScheduleCreatedEventListener> _listeners;

    public ParserScheduleCreatedEventListenerPipeline(IEnumerable<IParserScheduleCreatedEventListener> listeners)
    {
        _listeners = new Queue<IParserScheduleCreatedEventListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserScheduleCreatedEventListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure) return Unit.Value;
        }
        
        return Unit.Value;
    }
}