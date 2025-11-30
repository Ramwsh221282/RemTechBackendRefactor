using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnUpdated;

public sealed class OnParserStatisticsUpdatedEventListenerPipeline : IParserStatisticsUpdatedEventListener
{
    private readonly Queue<IParserStatisticsUpdatedEventListener> _listeners;
    
    public OnParserStatisticsUpdatedEventListenerPipeline()
    {
        _listeners = [];
    }

    public OnParserStatisticsUpdatedEventListenerPipeline(IEnumerable<IParserStatisticsUpdatedEventListener> listeners)
    {
        _listeners = new Queue<IParserStatisticsUpdatedEventListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserStatisticsUpdatedEventListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure) return result.Error;
        }
        
        return Result.Success(Unit.Value);
    }
}