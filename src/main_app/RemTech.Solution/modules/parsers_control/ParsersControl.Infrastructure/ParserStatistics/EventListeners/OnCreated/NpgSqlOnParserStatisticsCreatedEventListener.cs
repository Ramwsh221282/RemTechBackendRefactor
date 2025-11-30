using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnCreated;

public sealed class NpgSqlOnParserStatisticsCreatedEventListener(
    IParserStatisticsStorage storage
    ) : IParserStatisticsCreatedEventListener
{
    public async Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default)
    {
        if (await HasStatisticsAlready(data, ct)) return Error.Conflict("У парсера уже есть статистика");
        ParserStatistic statistic = new(data);
        await storage.Persist(statistic, ct);
        return Unit.Value;
    }

    private async Task<bool> HasStatisticsAlready(ParserStatisticData data, CancellationToken ct)
    {
        ParserStatisticsQuery query = new(data.ParserId);
        ParserStatistic? statistic = await storage.Fetch(query, ct);
        return statistic != null;
    }
}