using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnCreated;

public sealed class OnParserStatisticsCreatedEventListenerPipeline : IParserStatisticsCreatedEventListener
{
    private readonly Queue<IParserStatisticsCreatedEventListener> _listeners;

    public OnParserStatisticsCreatedEventListenerPipeline()
    {
        _listeners = [];
    }

    public OnParserStatisticsCreatedEventListenerPipeline(IEnumerable<IParserStatisticsCreatedEventListener> listeners)
    {
        _listeners = new  Queue<IParserStatisticsCreatedEventListener>(listeners);
    }
    
    public async Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default)
    {
        while (_listeners.Count > 0)
        {
            IParserStatisticsCreatedEventListener listener = _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure) return result.Error;
        }
        
        return  Result.Success(Unit.Value);
    }
}