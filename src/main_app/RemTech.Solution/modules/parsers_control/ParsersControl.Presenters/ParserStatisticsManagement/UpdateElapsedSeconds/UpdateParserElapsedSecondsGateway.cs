using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnUpdated;
using ParsersControl.Presenters.ParserStatisticsManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStatisticsManagement.UpdateElapsedSeconds;

public sealed class UpdateParserElapsedSecondsGateway(
    IParserStatisticsStorage storage,
    IEnumerable<IParserStatisticsUpdatedEventListener> listeners)
    : IGateway<UpdateParserElapsedSecondsRequest, ParserStatisticsUpdateResponse>
{
    public async Task<Result<ParserStatisticsUpdateResponse>> Execute(UpdateParserElapsedSecondsRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserStatisticsQuery query = new(Id: request.Id);
        ParserStatistic? statistic = await storage.Fetch(query, ct);
        if (statistic == null) return Error.NotFound("Парсер не найден");
        OnParserStatisticsUpdatedEventListenerPipeline pipeline = new(listeners);
        ParserStatistic observed = statistic.AddListener(pipeline);
        Result<ParserStatistic> update = await observed.UpdateElapsedSeconds(request.ElapsedSeconds, ct);
        return update.IsFailure ? update.Error : new ParserStatisticsUpdateResponse(update.Value);
    }
}