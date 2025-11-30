using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnUpdated;

public sealed class NpgSqlOnParserStatisticsUpdatedEventListener(
    IParserStatisticsStorage storage)
    : IParserStatisticsUpdatedEventListener
{
    public async Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default)
    {
        ParserStatistic statistic = new(data);
        await storage.Update(statistic, ct);
        return Unit.Value;
    }
}