using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnCreated;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnParserRegistered;

public sealed class RegisterEmptyStatisticsOnParserRegisteredListener(
    IEnumerable<IParserStatisticsCreatedEventListener> listeners) 
    : IOnParserRegisteredEventListener
{
    public async Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
    {
        OnParserStatisticsCreatedEventListenerPipeline pipeline = new(listeners);
        ParserStatistic statistic = new(new ParserStatisticData(data.Id, 0, 0));
        ParserStatistic observed = statistic.AddListener(pipeline);
        Result<ParserStatistic> processing = await observed.Register(ct);
        return processing.IsFailure ? processing.Error : Unit.Value;
    }
}